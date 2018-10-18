using Akka.Actor;
using BuildingEconomy.Components;
using BuildingEconomy.Systems.Actors;
using BuildingEconomy.Systems.Messages;
using BuildingEconomy.Systems.Transportable.Messages;
using TransportableComponent = BuildingEconomy.Components.Transportable;

namespace BuildingEconomy.Systems.Transportable.Actors
{
    public class TransportableStorageActor : ComponentActor<TransportableStorage>
    {
        public static string NotContainingTransportable = "I do not have that transportable.";
        public static string NotEnoughCapacity = "The storage has not enough capacity.";

        /// <inheritdoc />
        public TransportableStorageActor(TransportableStorage component) : base(component)
        {
            Become(Default);
        }

        public static Props Props(TransportableStorage component)
        {
            return Akka.Actor.Props.Create(() => new TransportableStorageActor(component));
        }

        private void Default()
        {
            Receive<GiveTransportableTo>(msg => HandleGiveTransportableTo(msg));
        }

        private void HandleGiveTransportableTo(GiveTransportableTo message)
        {
            if (!CheckGiveTransportableTo(message, out TransportableComponent transportable,
                out TransportableStorage targetStorage))
            {
                return;
            }

            if (!CheckCapacity(message, targetStorage))
            {
                return;
            }

            targetStorage.TransportableIds.Add(transportable.Id);
            transportable.TransporterId = targetStorage.Id;
            Component.TransportableIds.Remove(transportable.Id);
        }

        private bool CheckCapacity(GiveTransportableTo message, TransportableStorage targetStorage)
        {
            bool limitedCapacity = targetStorage.Capacity > 0;
            if (limitedCapacity && targetStorage.TransportableIds.Count >= targetStorage.Capacity)
            {
                Sender.Tell(new CouldNotProcessMessage(message, NotEnoughCapacity));
                return false;
            }

            return true;
        }

        private bool CheckGiveTransportableTo(GiveTransportableTo message, out TransportableComponent transportable,
            out TransportableStorage targetStorage)
        {
            transportable = null;
            targetStorage = null;
            if (!CheckTransportable(message))
            {
                return false;
            }

            transportable = message.Transportable;
            targetStorage = message.Target;
            return true;
        }

        private bool CheckTransportable(GiveTransportableTo message)
        {
            if (Component.TransportableIds.Contains(message.Transportable.Id))
            {
                return true;
            }

            Sender.Tell(new CouldNotProcessMessage(message, NotContainingTransportable));
            return false;
        }
    }
}
