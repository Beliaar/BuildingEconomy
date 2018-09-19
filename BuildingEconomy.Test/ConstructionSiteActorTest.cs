using Akka.Actor;
using Akka.TestKit.Xunit2;
using BuildingEconomy.Systems.Construction;
using BuildingEconomy.Systems.Construction.Messages;
using BuildingEconomy.Systems.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using Xenko.Engine;
using Xunit;

namespace BuildingEconomy.Test
{
    public class ConstructionComponentActorTest : TestKit
    {

        /// <summary>
        /// Tests if the site set ups correctly.
        /// </summary>
        [Fact]
        public void TestComponentActorInit()
        {
            var building = new Building()
            {
                Name = "Test",
                Stages =
                {
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

            var minTimeBetweenUpdate = TimeSpan.FromSeconds(ConstructionSiteActor.SecondsBetweenUpdate);

            IActorRef componentActor = Sys.ActorOf(ConstructionSiteActor.Props(siteComponent, building), "TestSiteActor");
            componentActor.Tell(new Update(new Xenko.Games.GameTime()));
            ExpectNoMsg(1000);
            Assert.Equal(0, siteComponent.CurrentStage);
            Assert.Equal(0f, siteComponent.CurrentStageProgress);
            componentActor.Tell(new Update(new Xenko.Games.GameTime(minTimeBetweenUpdate, minTimeBetweenUpdate)));
            ExpectMsg<BuilderNeeded>();
        }

        /// <summary>
        /// Test if the preparing stage completes successfully.
        /// </summary>
        [Fact]
        public void TestComponentActorPreparing()
        {
            var building = new Building()
            {
                Name = "Test",
                Stages =
                {
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

            var minTimeBetweenUpdate = TimeSpan.FromSeconds(ConstructionSiteActor.SecondsBetweenUpdate);

            IActorRef componentActor = Sys.ActorOf(ConstructionSiteActor.Props(siteComponent, building), "TestSiteActor");
            componentActor.Tell(new Update(new Xenko.Games.GameTime()));
            componentActor.Tell(new Update(new Xenko.Games.GameTime(minTimeBetweenUpdate, minTimeBetweenUpdate)));
            ExpectMsg<BuilderNeeded>();
            componentActor.Tell(new AdvanceProgress());
            ExpectNoMsg(100);
            Assert.Equal(0.25f, siteComponent.CurrentStageProgress);
            componentActor.Tell(new AdvanceProgress());
            componentActor.Tell(new AdvanceProgress());
            componentActor.Tell(new AdvanceProgress());
            ExpectNoMsg(100);
            Assert.Equal(1.0f, siteComponent.CurrentStageProgress);
        }

        /// <summary>
        /// Tests a complete construction with a single stage.
        /// </summary>
        [Fact]
        public void TestComponentActorConstructionSingleStage()
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
                        Steps = 2,
                    },
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

            var minTimeBetweenUpdate = TimeSpan.FromSeconds(ConstructionSiteActor.SecondsBetweenUpdate);

            IActorRef componentActor = Sys.ActorOf(ConstructionSiteActor.Props(siteComponent, building), "TestSiteActor");
            siteComponent.CurrentStageProgress = 1.0f;
            componentActor.Tell(new Update(new Xenko.Games.GameTime(minTimeBetweenUpdate, minTimeBetweenUpdate)));
            ExpectMsg<WaitingForResources>();
            Assert.Equal(1, siteComponent.CurrentStage);
            Assert.Equal(0f, siteComponent.CurrentStageProgress);
            Components.Storage storage = testEntity.Get<Components.Storage>();
            Building.Stage curStage = building.Stages[siteComponent.CurrentStage - 1];
            foreach (KeyValuePair<string, int> pair in curStage.NeededRessources)
            {
                Assert.Equal(pair.Value, storage.RequestedItems[pair.Key]);
                storage.Items[pair.Key] = pair.Value;
            }
            componentActor.Tell(new Update(new Xenko.Games.GameTime(minTimeBetweenUpdate, minTimeBetweenUpdate)));
            ExpectMsg<BuilderNeeded>();
            componentActor.Tell(new AdvanceProgress());
            ExpectNoMsg(100);
            Assert.NotEqual(0f, siteComponent.CurrentStageProgress);
            Assert.True(siteComponent.CurrentStageProgress < 1f);
            foreach (int _ in Enumerable.Range(0, curStage.Steps))
            {
                componentActor.Tell(new AdvanceProgress());
            }
            ExpectNoMsg(100);
            Assert.True(siteComponent.CurrentStageProgress >= 1.0f);
            componentActor.Tell(new Update(new Xenko.Games.GameTime(minTimeBetweenUpdate, minTimeBetweenUpdate)));
            ExpectMsg<ConstructionFinished>();
        }

        /// <summary>
        /// Tests a complete construction with a multiple stages.
        /// </summary>
        [Fact]
        public void TestComponentActorConstructionMultiStage()
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
                        Steps = 2,
                    },
                    new Building.Stage()
                    {
                        NeededRessources = new Dictionary<string, int>
                        {
                            { "Wood", 2 },
                            { "Stone", 1 }
                        },
                        Steps = 3,
                    },
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

            var minTimeBetweenUpdate = TimeSpan.FromSeconds(ConstructionSiteActor.SecondsBetweenUpdate);

            IActorRef componentActor = Sys.ActorOf(ConstructionSiteActor.Props(siteComponent, building), "TestSiteActor");
            siteComponent.CurrentStageProgress = 1.0f;
            componentActor.Tell(new Update(new Xenko.Games.GameTime(minTimeBetweenUpdate, minTimeBetweenUpdate)));
            ExpectMsg<WaitingForResources>();
            Components.Storage storage = testEntity.Get<Components.Storage>();
            Building.Stage curStage = building.Stages[siteComponent.CurrentStage - 1];
            foreach (KeyValuePair<string, int> pair in curStage.NeededRessources)
            {
                Assert.Equal(pair.Value, storage.RequestedItems[pair.Key]);
                storage.Items[pair.Key] = pair.Value;
            }
            componentActor.Tell(new Update(new Xenko.Games.GameTime(minTimeBetweenUpdate, minTimeBetweenUpdate)));
            ExpectMsg<BuilderNeeded>();
            foreach (int _ in Enumerable.Range(0, curStage.Steps))
            {
                componentActor.Tell(new AdvanceProgress());
            }
            componentActor.Tell(new Update(new Xenko.Games.GameTime(minTimeBetweenUpdate, minTimeBetweenUpdate)));
            ExpectMsg<WaitingForResources>();
            Assert.Equal(2, siteComponent.CurrentStage);
            Assert.Equal(0f, siteComponent.CurrentStageProgress);
            curStage = building.Stages[siteComponent.CurrentStage - 1];
            foreach (KeyValuePair<string, int> pair in storage.RequestedItems)
            {
                storage.Items[pair.Key] = pair.Value;
            }
            componentActor.Tell(new Update(new Xenko.Games.GameTime(minTimeBetweenUpdate, minTimeBetweenUpdate)));
            ExpectMsg<BuilderNeeded>();
            foreach (int _ in Enumerable.Range(0, curStage.Steps))
            {
                componentActor.Tell(new AdvanceProgress());
            }
            componentActor.Tell(new Update(new Xenko.Games.GameTime(minTimeBetweenUpdate, minTimeBetweenUpdate)));
            ExpectMsg<ConstructionFinished>();
        }

        /// <summary>
        /// Tests if the needed resources are correctly set for the next stage if the current ones are met.
        /// </summary>
        [Fact]
        public void TestComponentActorConstructionMultiStagePreDeliver()
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
                        Steps = 2,
                    },
                    new Building.Stage()
                    {
                        NeededRessources = new Dictionary<string, int>
                        {
                            { "Wood", 2 },
                            { "Stone", 1 }
                        },
                        Steps = 3,
                    },
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

            var minTimeBetweenUpdate = TimeSpan.FromSeconds(ConstructionSiteActor.SecondsBetweenUpdate);

            IActorRef componentActor = Sys.ActorOf(ConstructionSiteActor.Props(siteComponent, building), "TestSiteActor");
            siteComponent.CurrentStageProgress = 1.0f;
            componentActor.Tell(new Update(new Xenko.Games.GameTime(minTimeBetweenUpdate, minTimeBetweenUpdate)));
            ExpectMsg<WaitingForResources>();
            Components.Storage storage = testEntity.Get<Components.Storage>();
            Building.Stage curStage = building.Stages[siteComponent.CurrentStage - 1];
            curStage = building.Stages[siteComponent.CurrentStage - 1];
            foreach (KeyValuePair<string, int> pair in storage.RequestedItems)
            {
                storage.Items[pair.Key] = pair.Value;
            }
            componentActor.Tell(new Update(new Xenko.Games.GameTime(minTimeBetweenUpdate, minTimeBetweenUpdate)));
            ExpectMsg<BuilderNeeded>();
            foreach (KeyValuePair<string, int> pair in storage.RequestedItems)
            {
                if (storage.Items.ContainsKey(pair.Key))
                {
                    Assert.NotEqual(pair.Value, storage.Items[pair.Key]);
                }
                storage.Items[pair.Key] = pair.Value;
            }
            componentActor.Tell(new Update(new Xenko.Games.GameTime(minTimeBetweenUpdate, minTimeBetweenUpdate)));
            ExpectMsg<BuilderNeeded>();
            foreach (int _ in Enumerable.Range(0, curStage.Steps))
            {
                componentActor.Tell(new AdvanceProgress());
            }
            componentActor.Tell(new Update(new Xenko.Games.GameTime(minTimeBetweenUpdate, minTimeBetweenUpdate)));
            ExpectMsg<BuilderNeeded>();
            curStage = building.Stages[siteComponent.CurrentStage - 1];
            foreach (int _ in Enumerable.Range(0, curStage.Steps))
            {
                componentActor.Tell(new AdvanceProgress());
            }
            componentActor.Tell(new Update(new Xenko.Games.GameTime(minTimeBetweenUpdate, minTimeBetweenUpdate)));
            ExpectMsg<ConstructionFinished>();

        }
    }
}