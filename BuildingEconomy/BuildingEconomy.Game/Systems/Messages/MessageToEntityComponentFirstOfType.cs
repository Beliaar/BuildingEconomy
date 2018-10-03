using System;
using Xenko.Engine;

namespace BuildingEconomy.Systems.Messages
{
    public abstract class MessageToEntityComponentFirstOfType<T> : IMessageToEntityComponentFirstOfType
        where T : EntityComponent
    {
        protected MessageToEntityComponentFirstOfType(Guid entityId, object message)
        {
            EntityId = entityId;
            Message = message;
        }

        public Guid EntityId { get; }
        public Type ComponentType => typeof(T);

        public object Message { get; }
    }
}
