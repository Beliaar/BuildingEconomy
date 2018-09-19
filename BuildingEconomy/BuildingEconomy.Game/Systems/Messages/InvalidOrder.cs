using System;

namespace BuildingEconomy.Systems.Messages
{
    public class InvalidOrder
    {
        public InvalidOrder(Guid entityId, Orders.Interfaces.IOrder order)
        {
            EntityId = entityId;
            Order = order;
        }

        public Guid EntityId { get; }
        public Orders.Interfaces.IOrder Order { get; }
    }
}
