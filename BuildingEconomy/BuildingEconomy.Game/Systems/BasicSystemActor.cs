using Akka.Actor;
using BuildingEconomy.Systems.Messages;
using Xenko.Engine;

namespace BuildingEconomy.Systems
{
    public abstract class BasicSystemActor<T> : ReceiveActor where T : BasicSystem<T>
    {
        protected BasicSystemActor(T system)
        {
            System = system;
            Become(Default);
        }

        public T System { get; }

        protected virtual void Default()
        {
            Receive<Update>(message => HandleStep(message));
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
            if (actor.Equals(ActorRefs.Nobody) && props != null)
            {
                actor = Context.ActorOf(props, actorName);
            }

            return actor;
        }


        public virtual void HandleStep(Update message)
        {
        }
    }
}