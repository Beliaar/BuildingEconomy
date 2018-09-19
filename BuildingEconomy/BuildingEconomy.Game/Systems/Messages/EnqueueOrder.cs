using System;

namespace BuildingEconomy.Systems.Messages
{
    /// <summary>
    /// Adds an order to the order queue of the entity.
    /// </summary>
    public class EnqueueOrder : IMessageToEntity
    {

        public EnqueueOrder(Guid entityId, Orders.Interfaces.IOrder order)
        {
            EntityId = entityId;
            Order = order;
        }

        public Orders.Interfaces.IOrder Order { get; }

        public Guid EntityId { get; }
    }
}
