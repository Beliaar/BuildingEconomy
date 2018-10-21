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
            GivenTargetStorageExists();
            TellMessageToStorage(new GiveTransportableTo(transportable, targetStorage), sourceStorage);
            ExpectNoMsg(100);
            Assert.Equal(targetStorage.Id, transportable.Transporter.Id);
            Assert.Contains(transportable, targetStorage.Transportables);
            Assert.DoesNotContain(transportable, sourceStorage.Transportables);
        }

        [Fact]
        public void NotContainingTransportable()
        {
            GivenTransportableExists();
            GivenSourceStorageExists();
            GivenTargetStorageExists();
            TellMessageToStorage(new GiveTransportableTo(transportable, targetStorage), sourceStorage);
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
            TellMessageToStorage(new GiveTransportableTo(transportable, targetStorage), sourceStorage);
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

        private void TellMessageToStorage(GiveTransportableTo message, TransportableStorage storage)
        {
            SetupTransportableStorageActor(storage).Tell(message);
        }

        private void GivenTransportableInSourceStorage()
        {
            GivenTransportablesInStorage(new List<Transportable> {transportable}, sourceStorage);
        }

        private void GivenTargetStorageExists()
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
                storage.Transportables.Add(transportable);
                transportable.Transporter = storage;
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
