namespace BuildingEconomy.Components
{
    // Transportable storage with additional values for resource containers.
    internal class ContainerStorage : TransportableStorage
    {
        /// <summary>
        ///     How many total items can the stored, including items in containers.
        /// </summary>
        /// <remarks>
        ///     A value of 0 means unlimited.
        /// </remarks>
        public uint TotalCapacity;
    }
}