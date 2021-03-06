﻿using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using BuildingEconomy.Systems.Interfaces;
using BuildingEconomy.Systems.Messages;
using BuildingEconomy.Systems.Orders.Interfaces;
using BuildingEconomy.Utils;
using Xenko.Engine;
using Xenko.Games;

namespace BuildingEconomy.Systems.Actors
{
    public class EntityActor : ReceiveActor
    {
        private readonly IComponentActorFactory componentActorFactory;
        private readonly Entity entity;
        private readonly Queue<IOrder> orderQueue = new Queue<IOrder>();
        private IOrder currentOrder;

        public EntityActor(Entity entity, IComponentActorFactory componentActorFactory)
        {
            this.entity = entity;
            this.componentActorFactory = componentActorFactory;
            Receive<Update>(msg => HandleUpdate(msg));
            Receive<IMessageToEntityComponentFirstOfType>(msg => HandleMessageToEntityComponent(msg));
            Receive<EnqueueOrder>(msg => orderQueue.Enqueue(msg.Order));
            Receive<Stop>(msg =>
                {
                    orderQueue.Clear();
                    currentOrder = null;
                }
            );
            ReceiveAny(msg => Context.Parent.Forward(msg));
        }

        public static Props Props(Entity entity, IComponentActorFactory componentActorFactory)
        {
            return Akka.Actor.Props.Create(() => new EntityActor(entity, componentActorFactory));
        }

        private void HandleMessageToEntityComponent(IMessageToEntityComponentFirstOfType message)
        {
            EntityComponent entityComponent = entity.FirstOrDefault(c => c.GetType() == message.ComponentType);
            componentActorFactory.GetOrCreateActorForComponent(entityComponent)?.Tell(message.Message, Sender);
        }

        private void HandleUpdate(Update message)
        {
            UpdateOrder(message.UpdateTime);
            foreach (EntityComponent entityComponent in entity)
            {
                componentActorFactory.GetOrCreateActorForComponent(entityComponent)?.Forward(message);
            }
        }

        private void UpdateOrder(GameTime updateTime)
        {
            if (currentOrder?.IsComplete(entity) ?? false)
            {
                currentOrder = null;
            }

            if (currentOrder is null)
            {
                if (!orderQueue.TryDequeue(out currentOrder))
                {
                    currentOrder = null;
                }
            }

            if (!currentOrder?.IsValid(entity) ?? false)
            {
                Context.Parent.Tell(new InvalidOrder(entity.Id, currentOrder));
                currentOrder = null;
            }

            currentOrder?.Update(entity, updateTime);
        }
    }
}
