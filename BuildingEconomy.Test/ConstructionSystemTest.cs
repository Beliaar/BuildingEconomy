using Akka.Actor;
using Moq;
using Xenko.Engine;
using Xunit;

namespace BuildingEconomy.Test
{
    public class ConstructionSystemTest
    {
        [Fact]
        public void TestInit()
        {
            var actorRefFactoryMock = new Mock<IActorRefFactory>();
            //actorRefFactoryMock.Setup(f => f.ActorOf(It.IsAny<Props>(), It.IsAny<string>())).Returns((Props props, string name) => ActorRefs.Nobody);
            var scene = new Scene();
            var mockServiceRegistry = new Mock<Xenko.Core.IServiceRegistry>();
            var sceneInstance = new SceneInstance(mockServiceRegistry.Object, scene);
            var system = new Systems.ConstructionSystem(sceneInstance, actorRefFactoryMock.Object);
            Assert.Equal("Construction", system.Name);
            system.Initialize();
            var building = new Systems.Construction.Building()
            {
                Name = "Test",
            };

            system.AddBuilding(building);

            Assert.NotNull(system.GetBuilding(building.Name));
        }
    }
}
