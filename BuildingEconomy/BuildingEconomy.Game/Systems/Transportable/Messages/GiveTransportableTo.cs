using BuildingEconomy.Components;

namespace BuildingEconomy.Systems.Transportable.Messages
{
    public class GiveTransportableTo
    {
        public GiveTransportableTo(Components.Transportable transportable, TransportableStorage target)
        {
            Transportable = transportable;
            Target = target;
        }

        public Components.Transportable Transportable { get; }
        public TransportableStorage Target { get; }
    }
}
