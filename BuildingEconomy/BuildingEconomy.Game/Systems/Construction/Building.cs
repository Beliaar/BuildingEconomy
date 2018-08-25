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
            public float StepProgress;
        }

        public string Name;
        public List<Stage> Stages = new List<Stage>();
    }
}
