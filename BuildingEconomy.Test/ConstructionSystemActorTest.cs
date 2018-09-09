using Akka.Actor;
using Akka.TestKit.Xunit2;
using BuildingEconomy.Components;
using BuildingEconomy.Systems.Construction;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xenko.Engine;
using Xenko.Games;
using Xunit;

namespace BuildingEconomy.Test
{
    public class ConstructionSystemActorTest : TestKit
    {

        private class ConstructionTestComponentActor : ConstructionSiteActor
        {
            public ConstructionTestComponentActor(ConstructionSite constructionSite, Systems.Construction.Building building) : base(constructionSite, building)
            {
            }

            public void TestBuilderNeeded()
            {
                Become(BuilderNeeded);
            }

            void TestWaitingForResources()
            {
                Become(WaitingForResources);
            }


            public void BuilderNeeded()
            {
                Receive<Systems.Messages.Update>((msg) => Context.Parent.Tell(new Systems.Construction.Messages.BuilderNeeded(Guid.Empty)));
            }

            public new void WaitingForResources()
            {
                Receive<Systems.Messages.Update>((msg) => Context.Parent.Tell(new Systems.Construction.Messages.WaitingForResources(Guid.Empty)));
            }

            public new void ConstructionFinished()
            {
                Receive<Systems.Messages.Update>((msg) => Context.Parent.Tell(new Systems.Construction.Messages.ConstructionFinished(Guid.Empty, "Test")));
            }

        }

        [Fact]
        public void TestHandleUpdate()
        {
            var actorRefFactoryMock = new Mock<IActorRefFactory>();
            actorRefFactoryMock.Setup(f => f.ActorOf(It.IsAny<Props>(), It.IsAny<string>())).Returns((Props props, string name) => ActorOfAsTestActorRef<SystemActor>(props, TestActor, name));

            var scene = new Scene();
            var mockServiceRegistry = new Mock<Xenko.Core.IServiceRegistry>();            
            var sceneInstance = new SceneInstance(mockServiceRegistry.Object, scene);
            var system = new Systems.ConstructionSystem(sceneInstance, actorRefFactoryMock.Object);
            var building = new Systems.Construction.Building()
            {
                Name = "Test",
                Stages =
                {
                    new Systems.Construction.Building.Stage()
                    {
                        NeededRessources = new Dictionary<string, int>
                        {
                            { "Wood", 2 }
                        },
                        Steps = 2,
                    },
                }
            };

            system.AddBuilding(building);
            var component = new ConstructionSite()
            {
                Building = "Test",                
            };
            var entity = new Entity
            {
                component,
            };

            scene.Entities.Add(entity);

            system.Actor.Tell(new Systems.Messages.Update(new GameTime(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5))));
            ExpectMsg<Systems.Construction.Messages.BuilderNeeded>();
            component.CurrentStage = 1;
            system.Actor.Tell(new Systems.Messages.Update(new GameTime(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5))));
            ExpectMsg<Systems.Construction.Messages.WaitingForResources>();

        }
    }
}
