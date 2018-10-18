using BuildingEconomy.Components;
using JetBrains.Annotations;

namespace BuildingEconomy.Systems.Transportable.Messages
{
    public class GiveTransportableTo
    {
        public GiveTransportableTo([NotNull] Components.Transportable transportable,
            [NotNull] TransportableStorage target)
        {
            Transportable = transportable;
            Target = target;
        }

        public Components.Transportable Transportable { get; }
        public TransportableStorage Target { get; }
    }
}
