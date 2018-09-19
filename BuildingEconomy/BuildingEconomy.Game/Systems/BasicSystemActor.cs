using Akka.Actor;
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


        public virtual void HandleStep(Messages.Update message)
        {
        }

    }
}
