using System;
using BuildingEconomy.Systems.Orders.Interfaces;

namespace BuildingEconomy.Systems.Messages
{
    public class InvalidOrder
    {
        public InvalidOrder(Guid entityId, IOrder order)
        {
            EntityId = entityId;
            Order = order;
        }

        public Guid EntityId { get; }
        public IOrder Order { get; }
    }
}