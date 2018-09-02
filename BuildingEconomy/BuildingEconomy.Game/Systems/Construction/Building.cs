using System.Collections.Generic;

namespace BuildingEconomy.Systems.Construction
{
    /// <summary>
    /// Construction data for a building.
    /// </summary>
    public class Building
    {
        public class Stage
        {
            public Dictionary<string, int> NeededRessources = new Dictionary<string, int>();
            public int Steps;
        }

        public string Name { get; set; }
        public List<Stage> Stages { get; } = new List<Stage>();
    }
}
