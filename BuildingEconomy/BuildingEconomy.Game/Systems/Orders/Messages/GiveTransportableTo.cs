using BuildingEconomy.Components;

namespace BuildingEconomy.Systems.Orders.Messages
{
    public class GiveTransportableTo
    {
        public GiveTransportableTo(Transportable transportable, TransportableStorage target)
        {
            Transportable = transportable;
            Target = target;
        }

        public Transportable Transportable { get; }
        public TransportableStorage Target { get; }
    }
}
