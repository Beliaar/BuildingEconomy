using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using BuildingEconomy.Components;
using BuildingEconomy.Systems;
using BuildingEconomy.Systems.Messages;
using BuildingEconomy.Systems.Orders;
using BuildingEconomy.Systems.Transportable.Messages;
using Moq;
using Xenko.Engine;
using Xenko.Games;
using Xunit;

// These are tests.
// ReSharper disable TooManyDeclarations
// ReSharper disable MethodTooLong 

namespace BuildingEconomy.Test
{
    public class DeliverContainerToStorageOrder : TestKit
    {
        private class EntityTestActor : ReceiveActor
        {
            public EntityTestActor()
            {
                ReceiveAny(msg => Sender.Tell(msg));
            }
        }

        [Fact]
        public void MoveToTarget()
        {
            // TODO: Add a message to move an entity to another.
            throw new NotImplementedException();
        }

        [Fact]
        public void UpdateGiveToTarget()
        {
            #region Setup

            var mockComponentActorFactory = new Mock<ComponentActorFactory<TransportableStorage>>(Sys);
            mockComponentActorFactory.Setup(f => f.GetProps(It.IsAny<TransportableStorage>()))
                .Returns(Props.Create(() => new EntityTestActor()));
            var resourceContainer = new ResourceContainer();
            var orderTargetTransportable = new TransportableStorage();
            var orderTargetEntity = new Entity {orderTargetTransportable};
            var deliverOrder =
                new DeliverContainerToStorage(resourceContainer, orderTargetEntity,
                    mockComponentActorFactory.Object);
            var transporterEntity = new Entity
            {
                new TransportableStorage
                {
                    TransportableIds =
                    {
                        resourceContainer.Id
                    }
                }
            };

            #endregion

            deliverOrder.Update(transporterEntity, new GameTime());
            var messageToEntityComponent = ExpectMsg<MessageToEntityComponentFirstOfType<TransportableStorage>>();
            Assert.Equal(transporterEntity.Id, messageToEntityComponent.EntityId);
            var giveTransportableTo = Assert.IsType<GiveTransportableTo>(messageToEntityComponent.Message);
            Assert.Equal(resourceContainer.Id, giveTransportableTo.Transportable.Id);
            Assert.Equal(orderTargetTransportable.Id, giveTransportableTo.Target.Id);
        }

        [Fact]
        public void UpdateMoveToSource()
        {
            // TODO: Add a message to move an entity to another.
            throw new NotImplementedException();
        }

        [Fact]
        public void UpdateTakeFromSource()
        {
            #region Setup

            var mockComponentActorFactory = new Mock<ComponentActorFactory<TransportableStorage>>(Sys);
            mockComponentActorFactory.Setup(f => f.GetProps(It.IsAny<TransportableStorage>()))
                .Returns(Props.Create(() => new EntityTestActor()));
            var sourceEntity = new Entity();
            var resourceContainer = new ResourceContainer();

            // TODO: Entity might not be needed
            var containerEntity = new Entity
            {
                resourceContainer,
                new Transportable
                {
                    TransporterId = sourceEntity.Id
                }
            };
            var orderTargetTransportable = new TransportableStorage();
            var orderTargetEntity = new Entity {orderTargetTransportable};
            var deliverOrder =
                new DeliverContainerToStorage(resourceContainer, orderTargetEntity,
                    mockComponentActorFactory.Object);
            var transporterStorage = new TransportableStorage();
            var transporterEntity = new Entity {transporterStorage};

            #endregion

            deliverOrder.Update(transporterEntity, new GameTime());
            var messageToEntityComponent = ExpectMsg<MessageToEntityComponentFirstOfType<TransportableStorage>>();
            Assert.Equal(sourceEntity.Id, messageToEntityComponent.EntityId);
            var giveTransportableTo = Assert.IsType<GiveTransportableTo>(messageToEntityComponent.Message);
            Assert.Equal(resourceContainer.Id, giveTransportableTo.Transportable.Id);
            Assert.Equal(transporterStorage.Id, giveTransportableTo.Target.Id);
        }

        [Fact]
        public void Valid()
        {
            var mockComponentActorFactory = new Mock<ComponentActorFactory<TransportableStorage>>(Sys);
            var resourceContainer = new ResourceContainer();
            var targetEntity = new Entity();
            var deliverOrder =
                new DeliverContainerToStorage(resourceContainer, targetEntity,
                    mockComponentActorFactory.Object);
            var entity = new Entity();
            Assert.False(deliverOrder.IsValid(entity));
            entity.Add(new TransportableStorage());
            Assert.True(deliverOrder.IsValid(entity));
        }
    }
}
