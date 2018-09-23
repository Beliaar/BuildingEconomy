using Akka.Actor;
using Akka.TestKit.Xunit2;
using BuildingEconomy.Systems.Actors;
using BuildingEconomy.Systems.Interfaces;
using BuildingEconomy.Systems.Messages;
using BuildingEconomy.Systems.Orders.Interfaces;
using Moq;
using System;
using System.Linq;
using Xenko.Engine;
using Xenko.Games;
using Xunit;
// ReSharper disable TooManyDeclarations
// ReSharper disable MethodTooLong

namespace BuildingEconomy.Test
{
    public class EntityActorTest : TestKit
    {
        private class EntityTestActor : ReceiveActor
        {
            public EntityTestActor(Guid guid)
            {
                Receive<string>((msg) => Sender.Tell(guid));
                Receive<Update>((msg) => Sender.Tell(guid));
            }
        }

        public class TestMessage
        {
        }

        public class TestComponent : EntityComponent
        {
        }

        public class TestComponentMessage : MessageToEntityComponent<TestComponent>
        {
            public TestComponentMessage(Guid entityId, object message) : base(entityId, message)
            {
            }
        }

        public class ComponentActor : ReceiveActor
        {
            public ComponentActor()
            {
                Receive<Guid>((msg) => Sender.Tell(msg));
            }
        }

        [Fact]
        public void TestMessageToEntityComponentFirstOfType()
        {
            var mockMessage = new Mock<IMessageToEntityComponentFirstOfType>();
            var mockComponentActorFactory = new Mock<IComponentActorFactory>();
            mockComponentActorFactory.Setup(f => f.GetOrCreateActorForComponent(It.IsAny<EntityComponent>())).Returns((EntityComponent component) =>
            {
                return Sys.ActorOf(Props.Create(() => new EntityTestActor(component.Id)), $"{component.Id}");
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
            mockComponentActorFactory.Setup(f => f.GetOrCreateActorForComponent(It.IsAny<EntityComponent>())).Returns((EntityComponent component) =>
                {
                    return Sys.ActorOf(Props.Create(() => new EntityTestActor(component.Id)), $"{component.Id}");
                }
            );

            var entity = new Entity
            {
                new Utils.TestComponent(),
                new Utils.TestComponent(),
                new Utils.TestComponent(),
            };

            IActorRef entityActor = Sys.ActorOf(EntityActor.Props(entity, mockComponentActorFactory.Object));
            entityActor.Tell(new Update(new GameTime()));
            ExpectMsgAllOf(entity.Select(c => c.Id).ToArray());
            ExpectNoMsg(500);
        }

        [Fact]
        public void TestOrders()
        {

            var mockOrder = new Mock<IOrder>();
            mockOrder.Setup(o => o.IsValid(It.IsAny<Entity>())).Returns(false);
            var entity = new Entity();
            var mockComponentActorFactory = new Mock<IComponentActorFactory>();
            mockComponentActorFactory.Setup(f => f.GetOrCreateActorForComponent(It.IsAny<EntityComponent>())).Returns((EntityComponent component) => ActorRefs.Nobody);

            IActorRef entityActor = ActorOfAsTestActorRef<EntityActor>(EntityActor.Props(entity, mockComponentActorFactory.Object), TestActor);

            entityActor.Tell(new EnqueueOrder(entity.Id, mockOrder.Object));
            entityActor.Tell(new Update(new GameTime()));
            ExpectMsg<InvalidOrder>();
            mockOrder.Reset();
            mockOrder.Setup(o => o.IsValid(It.IsAny<Entity>())).Returns(true);
            mockOrder.Setup(o => o.IsComplete(It.IsAny<Entity>())).Returns(false);
            entityActor.Tell(new EnqueueOrder(entity.Id, mockOrder.Object));
            entityActor.Tell(new Update(new GameTime()));
            ExpectNoMsg();
            mockOrder.Verify(o => o.IsComplete(It.IsAny<Entity>()), Times.AtMostOnce);
            mockOrder.Verify(o => o.IsValid(It.IsAny<Entity>()), Times.Once);
            mockOrder.Verify(o => o.Update(It.IsAny<Entity>(), It.IsAny<GameTime>()), Times.Once);
            mockOrder.Invocations.Clear();
            mockOrder.Setup(o => o.IsComplete(It.IsAny<Entity>())).Returns(true);
            entityActor.Tell(new Update(new GameTime()));
            ExpectNoMsg(100);
            mockOrder.Verify(o => o.IsComplete(It.IsAny<Entity>()), Times.Once);
            mockOrder.Verify(o => o.IsValid(It.IsAny<Entity>()), Times.AtMostOnce);
            mockOrder.Verify(o => o.Update(It.IsAny<Entity>(), It.IsAny<GameTime>()), Times.Never);
        }

        [Fact]
        public void TestStop()
        {
            var mockOrder = new Mock<IOrder>();
            mockOrder.Setup(o => o.IsValid(It.IsAny<Entity>())).Returns(true);
            mockOrder.Setup(o => o.IsComplete(It.IsAny<Entity>())).Returns(false);
            mockOrder.Setup(o => o.Update(It.IsAny<Entity>(), It.IsAny<GameTime>()))
                .Callback(() => TestActor.Tell("Failed"));
            var entity = new Entity();
            var mockComponentActorFactory = new Mock<IComponentActorFactory>();
            mockComponentActorFactory.Setup(f => f.GetOrCreateActorForComponent(It.IsAny<EntityComponent>())).Returns((EntityComponent component) => ActorRefs.Nobody);

            IActorRef entityActor = ActorOfAsTestActorRef<EntityActor>(EntityActor.Props(entity, mockComponentActorFactory.Object), TestActor);

            entityActor.Tell(new EnqueueOrder(entity.Id, mockOrder.Object));
            entityActor.Tell(new Update(new GameTime()));
            ExpectMsg("Failed");
            entityActor.Tell(new Stop(entity.Id));
            entityActor.Tell(new Update(new GameTime()));
            ExpectNoMsg(500);
        }

        [Fact]
        public void TestAnyMessage()
        {
            var entity = new Entity();
            var mockComponentActorFactory = new Mock<IComponentActorFactory>();

            IActorRef entityActor = ActorOfAsTestActorRef<EntityActor>(EntityActor.Props(entity, mockComponentActorFactory.Object), TestActor);
            // ReSharper disable once SuggestVarOrType_SimpleTypes
            entityActor.Tell(new TestMessage());
            ExpectMsg<TestMessage>();
        }

        [Fact]
        public void TestMessageToComponent()
        {
            var entity = new Entity
            {
                new TestComponent(),
            };
            var mockComponentActorFactory = new Mock<IComponentActorFactory>();
            mockComponentActorFactory.Setup(f => f.GetOrCreateActorForComponent(It.IsAny<EntityComponent>())).Returns((EntityComponent component) => Sys.ActorOf(Props.Create(() => new ComponentActor())));

            IActorRef entityActor = ActorOfAsTestActorRef<EntityActor>(EntityActor.Props(entity, mockComponentActorFactory.Object), TestActor);
            // ReSharper disable once SuggestVarOrType_SimpleTypes
            var message = Guid.NewGuid();
            entityActor.Tell(new TestComponentMessage(entity.Id, message));
            ExpectMsg(message);
        }
    }
}
