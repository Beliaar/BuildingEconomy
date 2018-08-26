using System;

namespace BuildingEconomy.Systems.Messages
{
    public abstract class MessageToEntity
    {
        public Guid EntityId { get; }

        protected MessageToEntity(Guid entityId)
        {
            EntityId = entityId;
        }
    }
}
