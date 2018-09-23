using Akka.Actor;
using Akka.TestKit.Xunit2;
using BuildingEconomy.Systems.Messages;
using BuildingEconomy.Test.Utils;
using Moq;
using Xenko.Engine;
using Xenko.Games;
using Xunit;

namespace BuildingEconomy.Test
{
    public class BasisSystemActorTest : TestKit
    {
        [Fact]
        public void TestDefaultUpdate()
        {
            var scene = new Scene();
            var mockServiceRegistry = new Mock<Xenko.Core.IServiceRegistry>();
            var sceneInstance = new SceneInstance(mockServiceRegistry.Object, scene);
            var mockSystem = new Mock<TestSystem>(sceneInstance);
            IActorRef testSystemActor = Sys.ActorOf(TestSystemActor.Props(mockSystem.Object));
            testSystemActor.Tell(new Update(new GameTime()));
            ExpectMsg<TestUpdateRespondMessage>();
        }
    }
}
