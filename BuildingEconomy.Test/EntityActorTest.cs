using Akka.Actor;
using Akka.TestKit.Xunit2;
using BuildingEconomy.Systems.Actors;
using BuildingEconomy.Systems.Interfaces;
using BuildingEconomy.Systems.Messages;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xenko.Engine;
using Xunit;

namespace BuildingEconomy.Test
{
    public class EntityActorTest : TestKit
    {
        class EntityTestActor : ReceiveActor
        {
            public EntityTestActor(Guid guid)
            {
                Receive<string>((msg) => Sender.Tell(guid));
                Receive<Update>((msg) => Sender.Tell(guid));
            }
        }

        [Fact]
        public void TestMessageToEntityComponentFirstOfType()
        {
            var mockMessage = new Mock<IMessageToEntityComponentFirstOfType>();
            var mockComponentActorFactory = new Mock<IComponentActorFactory>();
            mockComponentActorFactory.Setup(f => f.GetOrCreateActorForComponent(It.IsAny<EntityComponent>(), It.IsAny<IActorContext>())).Returns((EntityComponent component, IActorRefFactory factory) =>
            {
                return factory.ActorOf(Props.Create(() => new EntityTestActor(component.Id)), $"{component.Id}");
            }
            );

            var testComponent = new Utils.TestComponent();
            var entity = new Entity
            {
                testComponent,
                new Utils.TestComponent(),
            };

            mockMessage.Setup(m => m.EntityId).Returns(entity.Id);
            mockMessage.Setup(m => m.ComponentType).Returns(typeof(Utils.TestComponent));
            mockMessage.Setup(m => m.Message).Returns("Test");

            IActorRef entityActor = Sys.ActorOf(EntityActor.Props(entity, mockComponentActorFactory.Object));
            entityActor.Tell(mockMessage.Object);
            ExpectMsg(testComponent.Id);

            entity.RemoveAll<EntityComponent>();
            entity.Dispose();

            entity = new Entity
            {
                new Utils.TestComponent(),
                testComponent,
            };

            entityActor = Sys.ActorOf(EntityActor.Props(entity, mockComponentActorFactory.Object));
            entityActor.Tell(mockMessage.Object);
            ExpectMsg<Guid>(guid => guid != testComponent.Id);
        }

        [Fact]
        public void TestUpdate()
        {
            var mockComponentActorFactory = new Mock<IComponentActorFactory>();
            mockComponentActorFactory.Setup(f => f.GetOrCreateActorForComponent(It.IsAny<EntityComponent>(), It.IsAny<IActorContext>())).Returns((EntityComponent component, IActorRefFactory factory) =>
                {
                    return factory.ActorOf(Props.Create(() => new EntityTestActor(component.Id)), $"{component.Id}");
                }
            );

            var entity = new Entity
            {
                new Utils.TestComponent(),
                new Utils.TestComponent(),
                new Utils.TestComponent(),
            };

            IActorRef entityActor = Sys.ActorOf(EntityActor.Props(entity, mockComponentActorFactory.Object));
            entityActor.Tell(new Update(new Xenko.Games.GameTime()));
            ExpectMsgAllOf(entity.Select(c => c.Id).ToArray());
            ExpectNoMsg(500);
        }

        [Fact]
        public void TestOrders()
        {

            var mockOrder = new Mock<Systems.Orders.Interfaces.IOrder>();
            mockOrder.Setup(o => o.IsValid(It.IsAny<Entity>())).Returns(false);
            var entity = new Entity();
            var mockComponentActorFactory = new Mock<IComponentActorFactory>();
            mockComponentActorFactory.Setup(f => f.GetOrCreateActorForComponent(It.IsAny<EntityComponent>(), It.IsAny<IActorContext>())).Returns((EntityComponent component, IActorRefFactory factory) =>
                {
                    return ActorRefs.Nobody;
                }
            );            

            IActorRef entityActor = ActorOfAsTestActorRef<EntityActor>(EntityActor.Props(entity, mockComponentActorFactory.Object), TestActor);

            entityActor.Tell(new EnqueueOrder(entity.Id, mockOrder.Object));
            entityActor.Tell(new Update(new Xenko.Games.GameTime()));
            ExpectMsg<InvalidOrder>();
            mockOrder.Reset();
            mockOrder.Setup(o => o.IsValid(It.IsAny<Entity>())).Returns(true);
            mockOrder.Setup(o => o.IsComplete(It.IsAny<Entity>())).Returns(false);
            entityActor.Tell(new EnqueueOrder(entity.Id, mockOrder.Object));
            entityActor.Tell(new Update(new Xenko.Games.GameTime()));
            ExpectNoMsg();
            mockOrder.Verify(o => o.IsComplete(It.IsAny<Entity>()), Times.AtMostOnce);
            mockOrder.Verify(o => o.IsValid(It.IsAny<Entity>()), Times.Once);
            mockOrder.Verify(o => o.Update(It.IsAny<Entity>(), It.IsAny<Xenko.Games.GameTime>()), Times.Once);
            mockOrder.Invocations.Clear();
            mockOrder.Setup(o => o.IsComplete(It.IsAny<Entity>())).Returns(true);
            entityActor.Tell(new Update(new Xenko.Games.GameTime()));
            ExpectNoMsg(100);
            mockOrder.Verify(o => o.IsComplete(It.IsAny<Entity>()), Times.Once);
            mockOrder.Verify(o => o.IsValid(It.IsAny<Entity>()), Times.Never);
            mockOrder.Verify(o => o.Update(It.IsAny<Entity>(), It.IsAny<Xenko.Games.GameTime>()), Times.Never);
        }
    }
}
