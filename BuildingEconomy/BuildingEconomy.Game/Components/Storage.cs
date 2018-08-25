using System.Collections.Generic;
using Xenko.Engine;

namespace BuildingEconomy.Components
{
    /// <summary>
    /// Working data of a storage.
    /// </summary>
    public class Storage : EntityComponent
    {
        /// <summary>
        /// The list of stored items.
        /// </summary>
        public Dictionary<string, int> Items = new Dictionary<string, int>();

        /// <summary>
        /// Items requested to be moved to this storage.
        /// </summary>
        public Dictionary<string, int> RequestedItems = new Dictionary<string, int>();
    }
}
