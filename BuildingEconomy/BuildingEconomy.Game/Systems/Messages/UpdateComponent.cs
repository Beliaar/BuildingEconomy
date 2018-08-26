using System;
using Xenko.Games;

namespace BuildingEconomy.Systems.Messages
{
    internal class UpdateComponent : Update
    {
        public Guid ComponentId { get; }

        public UpdateComponent(GameTime updateTime, Guid componentId) : base(updateTime)
        {
            ComponentId = componentId;
        }
    }
}
