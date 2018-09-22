using System;
using Akka.Actor;

namespace BuildingEconomy.Systems.Interfaces
{
    public interface IEntityActorFactory
    {
        IActorRef GetOrCreateActorForEntity(Guid entityId);
    }
}