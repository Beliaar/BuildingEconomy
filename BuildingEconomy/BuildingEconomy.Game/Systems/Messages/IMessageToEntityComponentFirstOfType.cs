using System;

namespace BuildingEconomy.Systems.Messages
{
    public interface IMessageToEntityComponentFirstOfType : IMessageToEntityComponent
    {
        Type ComponentType { get; }
    }
}
