using System;

namespace BuildingEconomy.Systems.Messages
{
    public interface IMessageToEntity
    {
        Guid EntityId { get; }
    }
}