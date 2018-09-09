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
    public class ComponentActorFactoryTest : TestKit
    {
        class FactoryTestActor : ReceiveActor
        {
        }

        [Fact]
        public void TestGetOrCreateType()
        {
            var mockComponentActorFactory = new Mock<ComponentActorFactory<TestComponent>>();
            mockComponentActorFactory.Setup(f => f.GetProps(It.IsAny<TestComponent>())).Returns(Props.Create(() => new FactoryTestActor()));
            var mockActorContext = new Mock<IActorContext>();
            var mockComponent = new Mock<EntityComponent>();

            IComponentActorFactory componentActorFactory = mockComponentActorFactory.Object;

            var testComponentCorrect = new TestComponent();


            Assert.Null(componentActorFactory.GetOrCreateActorForComponent(testComponentCorrect, mockActorContext.Object));
            Assert.Throws<ArgumentException>(() => componentActorFactory.GetOrCreateActorForComponent(mockComponent.Object, mockActorContext.Object));

        }
    }
}
