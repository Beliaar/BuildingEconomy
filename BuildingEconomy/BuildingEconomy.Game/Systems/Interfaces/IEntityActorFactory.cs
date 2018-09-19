using Akka.Actor;
using System;

namespace BuildingEconomy.Systems.Interfaces
{
    public interface IEntityActorFactory
    {
        IActorRef GetOrCreateActorForEntity(Guid entityId);
    }
}
