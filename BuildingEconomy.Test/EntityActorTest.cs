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


    class TestActor : ReceiveActor
    {
        public TestActor(Guid guid)
        {
            Receive<string>((msg) => Sender.Tell(guid));
            Receive<Update>((msg) => Sender.Tell(guid));
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
            mockComponentActorFactory.Setup(f => f.GetOrCreateActorForComponent(It.IsAny<EntityComponent>(), It.IsAny<IActorContext>())).Returns((EntityComponent component, IActorRefFactory factory) =>
            {
                return factory.ActorOf(Props.Create(() => new TestActor(component.Id)), $"{component.Id}");
            }
            );

            var testComponent = new TestComponent();
            var entity = new Entity
            {
                testComponent,
                new TestComponent(),
            };

            mockMessage.Setup(m => m.EntityId).Returns(entity.Id);
            mockMessage.Setup(m => m.ComponentType).Returns(typeof(TestComponent));
            mockMessage.Setup(m => m.Message).Returns("Test");

            IActorRef entityActor = Sys.ActorOf(EntityActor.Props(entity, mockComponentActorFactory.Object));
            entityActor.Tell(mockMessage.Object);
            ExpectMsg(testComponent.Id);

            entity.RemoveAll<EntityComponent>();
            entity.Dispose();

            entity = new Entity
            {
                new TestComponent(),
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
                    return factory.ActorOf(Props.Create(() => new TestActor(component.Id)), $"{component.Id}");
                }
            );

            var entity = new Entity
            {
                new TestComponent(),
                new TestComponent(),
                new TestComponent(),
            };

            IActorRef entityActor = Sys.ActorOf(EntityActor.Props(entity, mockComponentActorFactory.Object));
            entityActor.Tell(new Update(new Xenko.Games.GameTime()));
            ExpectMsgAllOf(entity.Select(c => c.Id).ToArray());
            ExpectNoMsg(TimeSpan.FromMilliseconds(1000));
        }

    }
}
