using System;

namespace BuildingEconomy.Systems.Construction.Messages
{
    internal class AdvanceProgress : Systems.Messages.MessageToEntity
    {
        public AdvanceProgress(Guid entityId) : base(entityId)
        {
        }
    }
}
