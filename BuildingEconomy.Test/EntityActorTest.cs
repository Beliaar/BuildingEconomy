using Akka.Actor;
using Akka.TestKit.Xunit2;
using BuildingEconomy.Systems.Actors;
using BuildingEconomy.Systems.Interfaces;
using BuildingEconomy.Systems.Messages;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xenko.Engine;
using Xunit;

namespace BuildingEconomy.Test
{


    class TestActor : ReceiveActor
    {
        public TestActor(Guid guid)
        {
            Receive<string>((msg) => Sender.Tell(guid));
        }
    }

    [AllowMultipleComponents]
    public class TestComponent : EntityComponent
    {
    }


    public class EntityActorTest : TestKit
    {


        [Fact]
        public void TestMessageToEntityComponentFirstOfType()
        {
            var mockMessage = new Mock<IMessageToEntityComponentFirstOfType>();
            var mockComponentActorFactory = new Mock<IComponentActorFactory>();
            mockComponentActorFactory.Setup(f => f.GetOrCreateActorForComponent(It.IsAny<Guid>(), It.IsAny<IActorRefFactory>())).Returns((Guid guid, IActorRefFactory factory) =>
            {
                return factory.ActorOf(Props.Create(() => new TestActor(guid)), $"{guid}");
            }
            );

            var component = new TestComponent();
            var entity = new Entity
            {
                component,
                new TestComponent(),
            };

            mockMessage.Setup(m => m.EntityId).Returns(entity.Id);
            mockMessage.Setup(m => m.ComponentType).Returns(typeof(TestComponent));
            mockMessage.Setup(m => m.Message).Returns("Test");

            IActorRef entityActor = Sys.ActorOf(EntityActor.Props(entity, mockComponentActorFactory.Object));
            entityActor.Tell(mockMessage.Object);
            ExpectMsg<Guid>(guid => guid == component.Id);

            entity.RemoveAll<EntityComponent>();
            entity.Dispose();

            entity = new Entity
            {
                new TestComponent(),
                component,
            };

            entityActor = Sys.ActorOf(EntityActor.Props(entity, mockComponentActorFactory.Object));
            entityActor.Tell(mockMessage.Object);
            ExpectMsg<Guid>(guid => guid != component.Id);
        }
    }
}
