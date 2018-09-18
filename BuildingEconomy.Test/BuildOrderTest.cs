using Akka.Actor;
using Akka.TestKit.Xunit2;
using BuildingEconomy.Systems;
using BuildingEconomy.Systems.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xenko.Engine;
using Xunit;

namespace BuildingEconomy.Test
{
    public class BuildOrderTest : TestKit
    {
        class EntityTestActor : ReceiveActor
        {
            public EntityTestActor()
            {
                ReceiveAny(msg => Sender.Tell(msg));
            }
        }

        [Fact]
        public void TestValid()
        {
            // TODO: Add test after a builder component was added.
        }

        [Fact]
        public void TestComplete()
        {
            var mockComponentActorFactory = new Mock<ComponentActorFactory<Components.ConstructionSite>>(Sys);
            var constructionSite = new Components.ConstructionSite();
            var buildOrder = new Systems.Orders.Build(constructionSite, mockComponentActorFactory.Object);
            Assert.False(buildOrder.IsComplete(null));
            constructionSite.CurrentStageProgress = 1.0f;
            Assert.True(buildOrder.IsComplete(null));
        }

        [Fact]
        void TestUpdate()
        {
            var entity = new Entity();
            var mockComponentActorFactory = new Mock<ComponentActorFactory<Components.ConstructionSite>>(Sys);
            mockComponentActorFactory.Setup(f => f.GetProps(It.IsAny<Components.ConstructionSite>())).Returns(Props.Create(() => new EntityTestActor()));
            var constructionSite = new Components.ConstructionSite();
            var buildOrder = new Systems.Orders.Build(constructionSite, mockComponentActorFactory.Object);
            buildOrder.Update(entity, new Xenko.Games.GameTime());
            ExpectMsg<Systems.Construction.Messages.AdvanceProgress>();
        }
    }
}
