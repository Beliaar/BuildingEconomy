﻿using Akka.Actor;
using Akka.TestKit.Xunit2;
using BuildingEconomy.Systems;
using BuildingEconomy.Systems.Interfaces;
using Moq;
using System;
using Xenko.Engine;
using Xunit;

namespace BuildingEconomy.Test
{
    public class ComponentActorFactoryTest : TestKit
    {
        private class FactoryTestActor : ReceiveActor
        {
        }

        [Fact]
        public void TestGetOrCreateType()
        {
            var mockComponentActorFactory = new Mock<ComponentActorFactory<Utils.TestComponent>>(Sys);
            mockComponentActorFactory.Setup(f => f.GetProps(It.IsAny<Utils.TestComponent>())).Returns(Props.Create(() => new FactoryTestActor()));
            var mockActorContext = new Mock<IActorContext>();
            var mockComponent = new Mock<EntityComponent>();

            IComponentActorFactory componentActorFactory = mockComponentActorFactory.Object;

            var testComponentCorrect = new Utils.TestComponent();

            IActorRef actorRef = componentActorFactory.GetOrCreateActorForComponent(testComponentCorrect);

            Assert.NotNull(actorRef);
            Assert.Equal(actorRef, componentActorFactory.GetOrCreateActorForComponent(testComponentCorrect));
            Assert.Throws<ArgumentException>(() => componentActorFactory.GetOrCreateActorForComponent(mockComponent.Object));

        }
    }
}
