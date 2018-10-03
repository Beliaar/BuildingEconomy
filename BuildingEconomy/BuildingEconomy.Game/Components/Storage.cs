using System.Collections.Concurrent;

namespace BuildingEconomy.Components
{
    /// <inheritdoc />
    /// <summary>
    ///     Working data of a storage.
    /// </summary>
    public class Storage : ResourceContainer
    {
        /// <summary>
        ///     Items requested to be moved to this storage.
        /// </summary>
        public ConcurrentDictionary<string, int> RequestedItems { get; } = new ConcurrentDictionary<string, int>();
    }
}
