﻿using Akka.TestKit.Xunit2;
using BuildingEconomy.Test.Utils;
using Moq;
using Xenko.Engine;
using Xunit;

namespace BuildingEconomy.Test
{
    public class BasicSystemTest : TestKit
    {

        [Fact]
        public void TestReferenceCount()
        {
            var scene = new Scene();
            var mockServiceRegistry = new Mock<Xenko.Core.IServiceRegistry>();
            var sceneInstance = new SceneInstance(mockServiceRegistry.Object, scene);
            var mockSystem = new Mock<TestSystem>(sceneInstance);
            Assert.Equal(0, mockSystem.Object.ReferenceCount);
            Assert.Equal(1, mockSystem.Object.AddReference());
            Assert.Equal(1, mockSystem.Object.ReferenceCount);
            Assert.Equal(0, mockSystem.Object.Release());
            Assert.Equal(0, mockSystem.Object.ReferenceCount);
        }
    }
}
