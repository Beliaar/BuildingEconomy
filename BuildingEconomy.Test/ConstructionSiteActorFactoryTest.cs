using Akka.TestKit.Xunit2;
using BuildingEconomy.Components;
using BuildingEconomy.Systems.Construction;
using BuildingEconomy.Systems.Construction.Interfaces;
using Moq;
using System.Collections.Generic;
using Xunit;
using Building = BuildingEconomy.Systems.Construction.Building;

namespace BuildingEconomy.Test
{
    public class ConstructionSiteActorFactoryTest : TestKit
    {

        [Fact]
        public void TestGetActor()
        {
            var building = new Building()
            {
                Name = "Test",
            };
            var buildings = new Dictionary<string, Building>()
            {
                {"Test", building},
            };

            var buildingManagerMock = new Mock<IBuildingManager>();
            buildingManagerMock.Setup(bm => bm.GetBuilding(It.IsAny<string>())).Returns((string name) => buildings[name]);

            var factory = new ConstructionSiteActorFactory(buildingManagerMock.Object, Sys);

            var testSite = new ConstructionSite
            {
                Building = building.Name,
            };

            Assert.NotNull(factory.GetOrCreateActorForComponent(testSite));

        }
    }
}
