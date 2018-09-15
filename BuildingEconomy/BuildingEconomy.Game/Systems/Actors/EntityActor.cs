﻿using Akka.Actor;
using BuildingEconomy.Systems.Interfaces;
using BuildingEconomy.Systems.Messages;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Xenko.Engine;
using BuildingEconomy.Utils;

namespace BuildingEconomy.Systems.Actors
{
    public class EntityActor : ReceiveActor
    {
        private readonly Entity entity;
        private readonly IComponentActorFactory componentActorFactory;
        private Queue<Orders.Interfaces.IOrder> orderQueue = new Queue<Orders.Interfaces.IOrder>();
        private Orders.Interfaces.IOrder currentOrder = null;

        public EntityActor(Entity entity, IComponentActorFactory componentActorFactory)
        {
            this.entity = entity;
            this.componentActorFactory = componentActorFactory;
            Receive<Update>(msg => HandleUpdate(msg));
            Receive<IMessageToEntityComponentFirstOfType>(msg => HandleMessageToEntityComponent(msg));

            //Should be last IMessageToEntity* since akka.NET takes the first match in the order they were added.
            Receive<IMessageToEntity>(msg => HandleMessageToEntity(msg));
            ReceiveAny(msg => Context.Parent.Forward(msg));
        }

        public static Props Props(Entity entity, IComponentActorFactory componentActorFactory)
        {
            return Akka.Actor.Props.Create(() => new EntityActor(entity, componentActorFactory));
        }

        private void HandleMessageToEntity(IMessageToEntity message)
        {
        }
               
        private void HandleMessageToEntityComponent(IMessageToEntityComponentFirstOfType message)
        {
            EntityComponent entityComponent = entity.FirstOrDefault(c => c.GetType() == message.ComponentType);
            componentActorFactory.GetOrCreateActorForComponent(entityComponent, Context)?.Tell(message.Message, Sender);
        }

        private void HandleUpdate(Update message)
        {
            if (currentOrder != null || orderQueue.TryDequeue(out currentOrder))
            {
                if (currentOrder.IsComplete && !(orderQueue.TryDequeue(out currentOrder)))
                {
                    currentOrder = null;
                }
                else
                {
                    currentOrder.Update(entity, message.UpdateTime);
                }
            }
            foreach (EntityComponent entityComponent in entity)
            {
                componentActorFactory.GetOrCreateActorForComponent(entityComponent, Context)?.Forward(message);
            }
        }

    }
}
