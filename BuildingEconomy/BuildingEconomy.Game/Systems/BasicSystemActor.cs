using Akka.Actor;
using BuildingEconomy.Systems.Messages;

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

        protected virtual void HandleStep(Update message) { }
    }
}
