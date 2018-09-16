using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingEconomy.Systems.Messages
{
    /// <summary>
    /// Stops the current order and removes all queued orders of the entity.
    /// </summary>
    public class Stop : IMessageToEntity
    {
        public Stop(Guid entityId)
        {
            EntityId = entityId;
        }

        public Guid EntityId { get; }
    }
}
