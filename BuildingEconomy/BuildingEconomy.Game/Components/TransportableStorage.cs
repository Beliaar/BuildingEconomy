using System;
using System.Collections.Generic;
using Xenko.Engine;

namespace BuildingEconomy.Components
{
    /// <inheritdoc />
    /// <summary>
    ///     A component that can store transportables.
    /// </summary>
    public class TransportableStorage : EntityComponent
    {
        /// <summary>
        ///     Capacity of the storage
        /// </summary>
        /// <remarks>
        ///     A value of 0 means unlimited.
        /// </remarks>
        public uint Capacity;

        /// <summary>
        ///     The ids of the transportables this storage contains.
        /// </summary>
        public List<Guid> TransportableIds;
    }
}