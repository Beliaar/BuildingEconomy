using Xenko.Engine;

namespace BuildingEconomy.Components
{
    /// <inheritdoc />
    /// <summary>
    ///     Component that can be transported.
    /// </summary>
    public class Transportable : EntityComponent
    {
        public TransportableStorage Transporter { get; set; }
    }
}
