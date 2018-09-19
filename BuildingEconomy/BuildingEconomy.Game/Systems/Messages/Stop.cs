using System;

namespace BuildingEconomy.Systems.Messages
{
    /// <summary>
    /// Stops the current order and removes all queued orders of the entity.
    /// </summary>
    public class Stop : IMessageToEntity
    {
        public Stop(Guid entityId)
        {
            EntityId = entityId;
        }

        public Guid EntityId { get; }
    }
}
