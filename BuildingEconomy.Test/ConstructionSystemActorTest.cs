using Akka.Actor;
using Akka.TestKit.Xunit2;
using BuildingEconomy.Components;
using BuildingEconomy.Systems.Construction;
using Moq;
using System;
using System.Collections.Generic;
using Xenko.Engine;
using Xunit;

namespace BuildingEconomy.Test
{
    public class ConstructionSystemActorTest : TestKit
    {
        [Fact]
        public void TestBuilderNeeded()
        {
            var scene = new Scene();
            var mockServiceRegistry = new Mock<Xenko.Core.IServiceRegistry>();
            var sceneInstance = new SceneInstance(mockServiceRegistry.Object, scene);
            var actorRefFactoryMock = new Mock<IActorRefFactory>();
            actorRefFactoryMock.Setup(f => f.ActorOf(It.IsAny<Props>(), It.IsAny<string>())).Returns((Props props, string name) => ActorOfAsTestActorRef<SystemActor>(props, TestActor, name));
            var system = new Systems.ConstructionSystem(sceneInstance, actorRefFactoryMock.Object);
            var guid = new Guid();
            system.Actor.Tell(new Systems.Construction.Messages.BuilderNeeded(guid));
            ExpectMsg<Systems.Construction.Messages.BuilderNeeded>(bn => bn.ComponentId == guid);
        }

        [Fact]
        public void TestWaitingForResources()
        {
            var scene = new Scene();
            var mockServiceRegistry = new Mock<Xenko.Core.IServiceRegistry>();
            var sceneInstance = new SceneInstance(mockServiceRegistry.Object, scene);
            var actorRefFactoryMock = new Mock<IActorRefFactory>();
            actorRefFactoryMock.Setup(f => f.ActorOf(It.IsAny<Props>(), It.IsAny<string>())).Returns((Props props, string name) => ActorOfAsTestActorRef<SystemActor>(props, TestActor, name));
            var system = new Systems.ConstructionSystem(sceneInstance, actorRefFactoryMock.Object);
            var guid = new Guid();
            system.Actor.Tell(new Systems.Construction.Messages.WaitingForResources(guid));
            ExpectMsg<Systems.Construction.Messages.WaitingForResources>(bn => bn.ComponentId == guid);
        }

        [Fact]
        public void TestMessagesFromComponent()
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

            Assert.NotNull(entity.Get<ConstructionSite>());
            Assert.Null(entity.Get<Components.Building>());
            system.Actor.Tell(new Systems.Construction.Messages.ConstructionFinished(entity.Id, component.Building));
            ExpectNoMsg(500);
            Assert.Null(entity.Get<ConstructionSite>());
            Components.Building entBuilding = entity.Get<Components.Building>();
            Assert.NotNull(building);
            Assert.Equal(component.Building, entBuilding.Name);
        }
    }
}
