using System.Collections.Concurrent;
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
        public ConcurrentDictionary<string, int> Items = new ConcurrentDictionary<string, int>();

        /// <summary>
        /// Items requested to be moved to this storage.
        /// </summary>
        public ConcurrentDictionary<string, int> RequestedItems = new ConcurrentDictionary<string, int>();
    }
}
