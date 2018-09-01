using System;

namespace BuildingEconomy.Systems.Construction.Messages
{
    /// <summary>
    /// Base for messages intended for construction sites.
    /// </summary>
    public abstract class MessageToConstructionSite : Systems.Messages.MessageToEntityComponent<Components.ConstructionSite>
    {
        public MessageToConstructionSite(Guid entityId) : base(entityId)
        {
        }
    }
}
