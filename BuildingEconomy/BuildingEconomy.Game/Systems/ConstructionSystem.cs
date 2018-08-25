using System.Collections.Generic;
using System.Linq;
using Xenko.Engine;

namespace BuildingEconomy.Systems
{
    internal class ConstructionSystem : BasicSystem
    {
        public override string Name => "Construction";

        private readonly Dictionary<string, Construction.Building> buildings = new Dictionary<string, Construction.Building>();

        public ConstructionSystem(Game game) : base(game)
        {
        }

        public override void Initialize()
        {
            var warehouse = new Construction.Building()
            {
                Name = "Warehouse"
            };

            warehouse.Stages.Add(new Construction.Building.Stage
            {
                NeededRessources = new Dictionary<string, int>
                {
                    {"Wood", 4 },
                },
                StepProgress = 0.25f,
            });
            warehouse.Stages.Add(new Construction.Building.Stage
            {
                NeededRessources = new Dictionary<string, int>
                {
                    {"Wood", 2 },
                    {"Stone", 4 }
                },
                StepProgress = 0.15f,
            });
            warehouse.Stages.Add(new Construction.Building.Stage
            {
                NeededRessources = new Dictionary<string, int>
                {
                    {"Wood", 2 },
                    {"Stone", 3 }
                },
                StepProgress = 0.2f,
            });
            AddBuilding(warehouse);
        }

        public void AddBuilding(Construction.Building building)
        {
            if (!buildings.ContainsKey(building.Name))
            {
                buildings.Add(building.Name, building);
            }
        }

        public void ConstructionStep(Components.ConstructionSite construction)
        {
            Construction.Building building = buildings[construction.Building];
            Construction.Building.Stage stage = building.Stages[construction.CurrentStage - 1];
            Entity entity = construction.Entity;
            Components.Storage storage = entity.Components.Get<Components.Storage>();
            if (storage is null)
            {
                return;
            }
            foreach (string resource in stage.NeededRessources.Keys)
            {
                // Check if all resources are at or higher than the needed level.
                if (!storage.Items.ContainsKey(resource) || storage.Items[resource] < stage.NeededRessources[resource])
                {
                    return;
                }
            }

            construction.CurrentStageProgress += stage.StepProgress;

            if (construction.CurrentStageProgress >= 1.0f) // Stage is done.
            {

                foreach (string resource in stage.NeededRessources.Keys)
                {
                    storage.Items[resource] -= stage.NeededRessources[resource];
                }
                if (++construction.CurrentStage >= building.Stages.Count) // Construction is done.
                {
                    // TODO: Drop remaining resources, if any?
                    entity.Remove(construction);
                    var buildingComponent = new Components.Building
                    {
                        Name = building.Name,
                    };
                    entity.Add(buildingComponent);
                }
            }
        }

        public override void Step()
        {
            IEnumerable<Components.ConstructionSite> constructionSites = Game.SceneSystem.SceneInstance.SelectMany(si => si.Components.OfType<Components.ConstructionSite>());
            // TODO: Work with the sites.
        }
    }
}
