using Akka.Actor;
using Xenko.Engine;

namespace BuildingEconomy.Systems.Actors
{
    public abstract class ComponentActor<T> : ReceiveActor where T : EntityComponent
    {
        protected ComponentActor(T component)
        {
            Component = component;
        }

        protected T Component { get; set; }
    }
}
