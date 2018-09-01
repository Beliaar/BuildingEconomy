﻿using System;

namespace BuildingEconomy.Systems.Construction.Messages
{
    public class AdvanceProgress : MessageToConstructionSite
    {
        /// <summary>
        /// Tell the site to advance the progress by one step.
        /// </summary>
        /// <param name="entityId"></param>
        public AdvanceProgress(Guid entityId) : base(entityId)
        {
        }
    }
}
