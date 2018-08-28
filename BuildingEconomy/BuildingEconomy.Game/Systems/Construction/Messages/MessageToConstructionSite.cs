using System;

namespace BuildingEconomy.Systems.Construction.Messages
{
    public class MessageToConstructionSite : Systems.Messages.MessageToEntity
    {
        public MessageToConstructionSite(Guid entityId) : base(entityId)
        {
        }
    }
}
