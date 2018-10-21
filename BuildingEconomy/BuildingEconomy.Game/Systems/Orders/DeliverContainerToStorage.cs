using System;
using BuildingEconomy.Components;
using BuildingEconomy.Systems.Orders.Interfaces;
using Xenko.Engine;
using Xenko.Games;

namespace BuildingEconomy.Systems.Orders
{
    public class DeliverContainerToStorage : IOrder
    {
        private readonly ComponentActorFactory<TransportableStorage> componentActorFactory;
        private readonly ResourceContainer container;
        private readonly Entity target;

        public DeliverContainerToStorage(ResourceContainer container, Entity target,
            ComponentActorFactory<TransportableStorage> componentActorFactory)
        {
            this.container = container;
            this.target = target;
            this.componentActorFactory = componentActorFactory;
        }

        public bool IsComplete(Entity entity)
        {
            throw new NotImplementedException();
        }

        public bool IsValid(Entity entity)
        {
            return entity.Components.Get<TransportableStorage>() != null;
        }

        public void Update(Entity entity, GameTime updateTime)
        {
            throw new NotImplementedException();
        }
    }
}
