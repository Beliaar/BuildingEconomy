using Akka.Actor;
using Akka.TestKit.Xunit2;
using BuildingEconomy.Systems;
using System;
using Xenko.Engine;

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
        }

        public abstract class TestSystem : Systems.BasicSystem<TestSystem>
        {
            public TestSystem(EntityManager entityManager) : base(entityManager)
            {
            }
        }
    }
}
