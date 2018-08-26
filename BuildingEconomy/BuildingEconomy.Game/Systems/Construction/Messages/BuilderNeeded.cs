using System;

namespace BuildingEconomy.Systems.Construction.Messages
{
    internal class BuilderNeeded
    {
        public Guid ComponentId { get; }

        public BuilderNeeded(Guid componentId)
        {
            ComponentId = componentId;
        }
    }
}
