using System.Collections.Generic;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using BuildingEconomy.Components;
using BuildingEconomy.Systems.Messages;
using BuildingEconomy.Systems.Transportable.Actors;
using BuildingEconomy.Systems.Transportable.Messages;
using Xunit;

namespace BuildingEconomy.Test
{
    public class TransportableStorageActorTest : TestKit
    {
        private Transportable transportable;
        private TransportableStorage sourceStorage;
        private TransportableStorage targetStorage;

        [Fact]
        public void GiveTransportableTo()
        {
            GivenTransportableExists();
            GivenSourceStorageExists();
            GivenTransportableInSourceStorage();
            TellMessageToSourceStorage(new GiveTransportableTo(transportable, targetStorage));
            ExpectNoMsg(100);
            Assert.Equal(targetStorage.Id, transportable.TransporterId);
            Assert.Contains(transportable.Id, targetStorage.TransportableIds);
            Assert.DoesNotContain(transportable.Id, sourceStorage.TransportableIds);
        }

        [Fact]
        public void NotContainingTransportable()
        {
            GivenTransportableExists();
            GivenSourceStorageExists();
            GivenTransportableInSourceStorage();
            GivenTargetStorageExists();
            TellMessageToSourceStorage(new GiveTransportableTo(transportable, targetStorage));
            ExpectMsg<CouldNotProcessMessage>(message =>
                message.Reason == TransportableStorageActor.NotContainingTransportable);
        }

        [Fact]
        public void NotEnoughCapacity()
        {
            GivenTransportableExists();
            GivenSourceStorageExists();
            GivenTransportableInSourceStorage();
            GivenTargetStorageExists();
            GivenStorageHasCapacity(targetStorage, 1);
            GivenTransportablesInStorage(new List<Transportable> {CreateBareTransportable()}, targetStorage);
            TellMessageToSourceStorage(new GiveTransportableTo(transportable, targetStorage));
            ExpectMsg<CouldNotProcessMessage>(message =>
                message.Reason == TransportableStorageActor.NotEnoughCapacity);
        }

        private static TransportableStorage CreateEmptyStorage()
        {
            return new TransportableStorage();
        }

        private static Transportable CreateBareTransportable()
        {
            return new Transportable();
        }

        private void TellMessageToSourceStorage(GiveTransportableTo message)
        {
            SetupTransportableStorageActor(sourceStorage).Tell(message);
        }

        private void GivenTransportableInSourceStorage()
        {
            GivenTransportablesInStorage(new List<Transportable> {transportable}, sourceStorage);
        }

        private void GivenTargetStorageExists(uint capacity = 0)
        {
            targetStorage = CreateEmptyStorage();
        }

        private static void GivenStorageHasCapacity(TransportableStorage storage, uint capacity)
        {
            storage.Capacity = capacity;
        }

        private static void GivenTransportablesInStorage(IEnumerable<Transportable> transportables,
            TransportableStorage storage)
        {
            foreach (Transportable transportable in transportables)
            {
                storage.TransportableIds.Add(transportable.Id);
                transportable.TransporterId = storage.Id;
            }
        }

        private void GivenSourceStorageExists()
        {
            sourceStorage = CreateEmptyStorage();
        }

        private void GivenTransportableExists()
        {
            transportable = CreateBareTransportable();
        }

        private IActorRef SetupTransportableStorageActor(TransportableStorage storage)
        {
            return Sys.ActorOf(TransportableStorageActor.Props(storage));
        }
    }
}
