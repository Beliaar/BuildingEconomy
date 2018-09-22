using Xenko.Engine;

namespace BuildingEconomy.Components
{
    /// <summary>
    ///     Working data for a construction site.
    /// </summary>
    public class ConstructionSite : EntityComponent
    {
        /// <summary>
        ///     The name of the building being constructed.
        /// </summary>
        public string Building { get; set; }

        /// <summary>
        ///     The index of the current Stage.
        /// </summary>
        public int CurrentStage { get; set; }

        /// <summary>
        ///     The progress of the current stage.
        /// </summary>
        public float CurrentStageProgress { get; set; }
    }
}