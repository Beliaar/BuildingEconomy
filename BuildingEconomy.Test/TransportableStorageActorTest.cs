using System;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using BuildingEconomy.Components;
using BuildingEconomy.Systems.Transportable.Actors;
using BuildingEconomy.Systems.Transportable.Messages;
using Xunit;

namespace BuildingEconomy.Test
{
    public class TransportableStorageActorTest : TestKit
    {
        [Fact]
        public void GiveTransportableTo()
        {
            var transportable = new Transportable();
            var transportableStorage = new TransportableStorage
            {
                TransportableIds = {transportable.Id}
            };
            IActorRef componentActor = Sys.ActorOf(TransportableStorageActor.Props(transportableStorage),
                "TestTransportableStorageActor");
            var targetStorage = new TransportableStorage();
            var giveTransportableTo = new GiveTransportableTo(transportable, targetStorage);
            componentActor.Tell(giveTransportableTo);
            ExpectNoMsg(100);
            Assert.Equal(targetStorage.Id, transportable.TransporterId);
            Assert.Contains(transportable.Id, targetStorage.TransportableIds);
            Assert.DoesNotContain(transportable.Id, transportableStorage.TransportableIds);
        }

        [Fact]
        public void InvalidTargetStorage()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void InvalidTransportable()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void NotContainingTransportable()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void NotEnoughCapacity()
        {
            throw new NotImplementedException();
        }
    }
}
