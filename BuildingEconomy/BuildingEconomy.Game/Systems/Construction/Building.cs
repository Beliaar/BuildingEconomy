using System.Collections.Generic;

namespace BuildingEconomy.Systems.Construction
{
    /// <summary>
    ///     Construction data for a building.
    /// </summary>
    public class Building
    {
        public string Name { get; set; }
        public List<Stage> Stages { get; } = new List<Stage>();

        public class Stage
        {
            public Dictionary<string, int> NeededResources = new Dictionary<string, int>();
            public int Steps;
        }
    }
}