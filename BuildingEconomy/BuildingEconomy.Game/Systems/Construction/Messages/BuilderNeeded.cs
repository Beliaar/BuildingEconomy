using System;

namespace BuildingEconomy.Systems.Construction.Messages
{
    /// <summary>
    ///     Message that the site needs builders.
    /// </summary>
    public class BuilderNeeded
    {
        public BuilderNeeded(Guid componentId)
        {
            ComponentId = componentId;
        }

        public Guid ComponentId { get; }
    }
}