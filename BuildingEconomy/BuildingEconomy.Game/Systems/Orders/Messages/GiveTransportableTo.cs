using System;
using BuildingEconomy.Components;

namespace BuildingEconomy.Systems.Orders.Messages
{
    public class GiveTransportableTo
    {
        public GiveTransportableTo(Guid transportableId, TransportableStorage target)
        {
            TransportableId = transportableId;
            Target = target;
        }

        public Guid TransportableId { get; }
        public TransportableStorage Target { get; }
    }
}