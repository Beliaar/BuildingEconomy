using Akka.Actor;
using Akka.TestKit.Xunit2;
using Moq;
using System;
using Xunit;
using Xenko.Engine;
using BuildingEconomy.Systems.Messages;
using System.Collections.Generic;
using BuildingEconomy.Systems.Construction;
using BuildingEconomy.Systems.Construction.Messages;

namespace BuildingEconomy.Test
{
    public class ConstructionSystemTest : TestKit
    {

 
        [Fact]
        public void TestComponentActor()
        {
            var building = new Building()
            {
                Name = "Test",
                Stages =
                {
                    new Building.Stage()
                    {
                        NeededRessources = new Dictionary<string, int>
                        {
                            { "Wood", 2 }
                        },
                        Steps = 1,
                    },
                    new Building.Stage()
                    {
                        NeededRessources = new Dictionary<string, int>
                        {
                            { "Wood", 2 },
                            { "Stone", 1 }
                        },
                        Steps = 3,
                    }
                }
            };

            var siteComponent = new Components.ConstructionSite()
            {
                Building = "Test",
            };

            var testEntity = new Entity("Test")
            {
                siteComponent
            };

            var minTimeBetweenUpdate = TimeSpan.FromSeconds(ComponentActor.SecondsBetweenUpdate);

            var componentActor = Sys.ActorOf(ComponentActor.Props(siteComponent, building), "TestSiteActor");
            componentActor.Tell(new Update(new Xenko.Games.GameTime()));
            ExpectNoMsg(1000);
            Assert.Equal(0, siteComponent.CurrentStage);
            Assert.Equal(0f, siteComponent.CurrentStageProgress);
            componentActor.Tell(new Update(new Xenko.Games.GameTime(minTimeBetweenUpdate, minTimeBetweenUpdate)));
            ExpectMsg<BuilderNeeded>();
            componentActor.Tell(new AdvanceProgress(testEntity.Id));
            ExpectNoMsg(10);
            Assert.Equal(0.25f, siteComponent.CurrentStageProgress);
            componentActor.Tell(new AdvanceProgress(testEntity.Id));
            componentActor.Tell(new AdvanceProgress(testEntity.Id));
            componentActor.Tell(new AdvanceProgress(testEntity.Id));
            ExpectNoMsg(10);
            Assert.Equal(1.0f, siteComponent.CurrentStageProgress);
            componentActor.Tell(new Update(new Xenko.Games.GameTime(minTimeBetweenUpdate, minTimeBetweenUpdate)));
            ExpectMsg<WaitingForResources>();
            Assert.Equal(1, siteComponent.CurrentStage);
            Assert.Equal(0f, siteComponent.CurrentStageProgress);
            Components.Storage storage = testEntity.Get<Components.Storage>();
            var curStage = building.Stages[siteComponent.CurrentStage - 1];
            foreach (KeyValuePair<string, int> pair in curStage.NeededRessources)
            {
                Assert.True(storage.RequestedItems[pair.Key] == pair.Value);
                storage.Items[pair.Key] = pair.Value;
            }
            componentActor.Tell(new Update(new Xenko.Games.GameTime(minTimeBetweenUpdate, minTimeBetweenUpdate)));
            ExpectNoMsg(1000);
        }
    }
}
