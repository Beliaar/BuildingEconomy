using System;

namespace BuildingEconomy.Systems.Construction.Messages
{
    internal class AdvanceProgress : MessageToConstructionSite
    {
        public AdvanceProgress(Guid entityId) : base(entityId)
        {
        }
    }
}
