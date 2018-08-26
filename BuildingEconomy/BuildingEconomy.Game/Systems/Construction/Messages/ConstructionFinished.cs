using System;

namespace BuildingEconomy.Systems.Construction.Messages
{
    internal class ConstructionFinished
    {
        public Guid EntityId { get; }
        public string Building { get; }

        public ConstructionFinished(Guid entityId, string building)
        {
            EntityId = entityId;
            Building = building;
        }
    }
}
