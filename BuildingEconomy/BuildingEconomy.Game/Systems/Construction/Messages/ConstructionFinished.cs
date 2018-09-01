using System;

namespace BuildingEconomy.Systems.Construction.Messages
{
    /// <summary>
    /// Message that the site has finished its construction.
    /// </summary>
    public class ConstructionFinished
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
