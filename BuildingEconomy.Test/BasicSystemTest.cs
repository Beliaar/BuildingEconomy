using Akka.Actor;
using Akka.TestKit.Xunit2;
using BuildingEconomy.Systems;
using Moq;
using System;
using Xenko.Engine;
using Xunit;

namespace BuildingEconomy.Test
{
    public class BasicSystemTest : TestKit
    {
        public class TestComponent : EntityComponent
        {
        }

        public class TestMessage : Systems.Messages.MessageToEntityComponent<TestComponent>
        {
            public TestMessage(Guid entityId, object message) : base(entityId, message)
            {
            }
        }

        public class ComponentActor : ReceiveActor
        {
            public ComponentActor()
            {
                Receive<TestMessage>((msg) => Sender.Tell("Received"));
            }
        }

        public class TestSystemActor : BasicSystemActor<TestSystem>
        {

            public TestSystemActor(TestSystem system) : base(system)
            {
            }

            protected override IActorRef GetOrCreateActor<C>(C component)
            {
                return GetOrCreateActor(
                    component,
                    Props.Create(() => new ComponentActor())
                    );
            }
        }

        public abstract class TestSystem : Systems.BasicSystem<TestSystem>
        {
            public TestSystem(EntityManager entityManager) : base(entityManager)
            {
            }
        }

        [Fact]
        public void TestForwardToEntityComponent()
        {
            var scene = new Scene();
            var mockServiceRegistry = new Mock<Xenko.Core.IServiceRegistry>();
            var sceneInstance = new SceneInstance(mockServiceRegistry.Object, scene);
            var system = new Mock<TestSystem>(sceneInstance);
            IActorRef systemActor = Sys.ActorOf(Props.Create(() => new TestSystemActor(system.Object)));
            system.SetupGet(m => m.Actor).Returns(systemActor);

            var component = new TestComponent();
            var entity = new Entity
            {
                component,
            };

            scene.Entities.Add(entity);

            system.Object.Actor.Tell(new TestMessage(entity.Id, null));
            ExpectMsg<string>();
        }
    }
}
