using System;
using Xenko.Engine;

namespace BuildingEconomy.Systems.Messages
{
    public interface IMessageToEntityComponent
    {
        Guid EntityId { get; }
        Type ComponentType { get; }
    }

    public abstract class MessageToEntityComponent<T> : IMessageToEntityComponent where T : EntityComponent
    {
        public Guid EntityId { get; }
        public Type ComponentType => typeof(T);

        protected MessageToEntityComponent(Guid entityId)
        {
            EntityId = entityId;
        }
    }
}
