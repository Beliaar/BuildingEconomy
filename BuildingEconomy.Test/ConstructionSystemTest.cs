using Akka.Actor;
using Akka.TestKit.Xunit2;
using Moq;
using System;
using Xunit;
using Xenko.Engine;
using BuildingEconomy.Systems.Messages;
using System.Collections.Generic;

namespace BuildingEconomy.Test
{
    public class ConstructionSystemTest : TestKit
    {

        /// <summary>
        /// Test that the system forwards ConstructionSiteMessages to the Component
        /// </summary>
        [Fact]
        public void TestForwardToComponent()
        {
            /*
            var scene = new Scene();
            var mockServiceRegistry = new Mock<Xenko.Core.IServiceRegistry>();
            var sceneInstance = new SceneInstance(mockServiceRegistry.Object, scene);
            var system = new Mock<Systems.BasicSystemActor>(sceneInstance, Sys);

            system.AddBuilding(new Systems.Construction.Building()
            {
                Name = "Test",
                Stages =
                {
                    new Systems.Construction.Building.Stage()
                    {
                        NeededRessources = new System.Collections.Generic.Dictionary<string, int>
                        {
                            { "Wood", 2 }
                        },
                        StepProgress = 0.5f,
                    },
                    new Systems.Construction.Building.Stage()
                    {
                        NeededRessources = new System.Collections.Generic.Dictionary<string, int>
                        {
                            { "Wood", 2 },
                            { "Stone", 1 }
                        },
                        StepProgress = 0.4f, //just needs to be over 1.0 in 3 steps.
                    }
                }
            });

            var siteComponent = new Components.ConstructionSite()
            {
                Building = "Test",
            };

            string componentName = Systems.Construction.SystemActor.GetComponentName(siteComponent);

            IActorRef testActor = Sys.ActorOf(Props.Create(() => new ComponentActor()), componentName);

            var testEntity = new Entity("Test")
            {
                siteComponent
            };
            scene.Entities.Add(testEntity);
            system.Actor.Tell(new TestMessage(testEntity.Id));
            ExpectMsg<string>();
            return;
            system.Actor.Tell(new Update(new Xenko.Games.GameTime()));
            ExpectNoMsg(10);
            system.Actor.Tell(new Update(new Xenko.Games.GameTime(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5))));
            ExpectMsg<Systems.Construction.Messages.BuilderNeeded>(TimeSpan.FromSeconds(5000));
            Assert.Equal(-1, siteComponent.CurrentStage);
            Assert.Equal(0, siteComponent.CurrentStageProgress);            
            var message = new Systems.Construction.Messages.AdvanceProgress(testEntity.Id);
            
            system.Actor.Tell(message);
            ExpectNoMsg(10);
            Assert.Equal(0.25f, siteComponent.CurrentStageProgress);
            system.Actor.Tell(message);
            system.Actor.Tell(message);
            ExpectNoMsg(10);
            Assert.True(siteComponent.CurrentStageProgress < 1.0f);
            system.Actor.Tell(message);
            ExpectNoMsg(10);
            Assert.True(siteComponent.CurrentStageProgress >= 1.0f);
            Assert.Equal(-1, siteComponent.CurrentStage);
            system.Actor.Tell(new Update(new Xenko.Games.GameTime()));
            //ExpectMsg<Systems.Construction.Messages.>();
            Assert.Equal(0, siteComponent.CurrentStage);
            Assert.Equal(0, siteComponent.CurrentStageProgress);
            */
        }

        [Fact]
        public void TestComponentActor()
        {

        }
    }
}
