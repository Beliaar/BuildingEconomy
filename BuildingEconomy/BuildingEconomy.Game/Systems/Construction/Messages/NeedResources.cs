using System;

namespace BuildingEconomy.Systems.Construction.Messages
{
    /// <summary>
    ///     Message that the site is waiting for resources.
    /// </summary>
    public class WaitingForResources
    {
        public WaitingForResources(Guid componentId)
        {
            ComponentId = componentId;
        }

        public Guid ComponentId { get; }
    }
}
