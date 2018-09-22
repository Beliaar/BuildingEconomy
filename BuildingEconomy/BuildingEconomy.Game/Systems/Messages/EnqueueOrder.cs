using System;
using BuildingEconomy.Systems.Orders.Interfaces;

namespace BuildingEconomy.Systems.Messages
{
    /// <summary>
    ///     Adds an order to the order queue of the entity.
    /// </summary>
    public class EnqueueOrder : IMessageToEntity
    {
        public EnqueueOrder(Guid entityId, IOrder order)
        {
            EntityId = entityId;
            Order = order;
        }

        public IOrder Order { get; }

        public Guid EntityId { get; }
    }
}