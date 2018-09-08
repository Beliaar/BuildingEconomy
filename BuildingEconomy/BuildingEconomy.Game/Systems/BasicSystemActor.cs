using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xenko.Engine;

namespace BuildingEconomy.Systems
{
    public abstract class BasicSystemActor<T> : ReceiveActor where T : BasicSystem<T>
    {
        public T System { get; }

        public BasicSystemActor(T system)
        {
            System = system;
            Become(Default);
        }

        protected virtual void Default()
        {
            Receive<Messages.Update>((message) => HandleStep(message));
            Receive<Messages.IMessageToEntityComponentFirstOfType>(msg => HandleMessageToEntityComponent(msg));
        }

        public static string GetComponentName(EntityComponent component)
        {
            return $"Component_{component.Id}";
        }

        protected void ForwardMessageToComponent(object message, EntityComponent component, IActorRef replyTo = null)
        {
            GetOrCreateActor(component).Tell(message, replyTo);
        }


        protected IActorRef GetOrCreateActor(EntityComponent component, Props props = null)
        {
            string actorName = GetComponentName(component);
            IActorRef actor = Context.Child(actorName);
            if (actor == ActorRefs.Nobody && props != null)
            {
                actor = Context.ActorOf(props, actorName);
            }

            return actor;
        }

        /// <summary>
        /// Returns an actor for the given component.
        /// </summary>
        /// <param name="actorName"></param>
        /// <returns></returns>
        protected abstract IActorRef GetOrCreateActor<C>(C component) where C : EntityComponent;

        public void HandleMessageToEntityComponent(Messages.IMessageToEntityComponentFirstOfType message)
        {
            Entity entity = System.EntityManager.SingleOrDefault(e => e.Id == message.EntityId);
            EntityComponent component = entity?.FirstOrDefault(c => c.GetType() == message.ComponentType);
            if (component is null)
            {
                return;
            }
            ForwardMessageToComponent(message, component, Sender);

        }

        public virtual void HandleStep(Messages.Update message)
        {
        }

    }
}
