using System.Collections.Concurrent;
using Xenko.Engine;

namespace BuildingEconomy.Components
{
    /// <summary>
    ///     A container for storing of resources.
    /// </summary>
    public class ResourceContainer : EntityComponent
    {
        /// <summary>
        ///     How many resources the container can hold. 0 is unlimited.
        /// </summary>
        public uint Capacity;

        /// <summary>
        ///     The items the container has.
        /// </summary>
        public ConcurrentDictionary<string, int> Items = new ConcurrentDictionary<string, int>();
    }
}