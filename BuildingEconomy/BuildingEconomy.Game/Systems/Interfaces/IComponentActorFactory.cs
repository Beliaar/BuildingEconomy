using Akka.Actor;
using Xenko.Engine;

namespace BuildingEconomy.Systems.Interfaces
{
    public interface IComponentActorFactory
    {
        IActorRef GetOrCreateActorForComponent(EntityComponent component);
    }
}
