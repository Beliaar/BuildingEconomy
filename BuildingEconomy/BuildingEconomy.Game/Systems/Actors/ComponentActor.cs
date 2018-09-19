using Akka.Actor;
using Xenko.Engine;

namespace BuildingEconomy.Systems.Actors
{
    public abstract class ComponentActor<T> : ReceiveActor where T : EntityComponent
    {
        protected T Component { get; set; }

        public ComponentActor(T component)
        {
            Component = component;
        }
    }
}
