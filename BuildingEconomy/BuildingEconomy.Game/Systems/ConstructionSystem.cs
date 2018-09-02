﻿using Akka.Actor;
using System.Collections.Generic;
using System.Linq;
using Xenko.Engine;

namespace BuildingEconomy.Systems
{
    public class ConstructionSystem : BasicSystem<ConstructionSystem>
    {
        public override string Name => "Construction";
        public override IActorRef Actor => actor;

        private readonly Dictionary<string, Construction.Building> buildings = new Dictionary<string, Construction.Building>();
        private readonly IActorRef actor;

        public ConstructionSystem(EntityManager entityManager, IActorRefFactory actorRefFactory) : base(entityManager)
        {
            actor = actorRefFactory.ActorOf(Construction.SystemActor.Props(this), "ConstructionSystem");
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
                Steps = 4,
            });
            warehouse.Stages.Add(new Construction.Building.Stage
            {
                NeededRessources = new Dictionary<string, int>
                {
                    {"Wood", 2 },
                    {"Stone", 4 }
                },
                Steps = 6,
            });
            warehouse.Stages.Add(new Construction.Building.Stage
            {
                NeededRessources = new Dictionary<string, int>
                {
                    {"Wood", 2 },
                    {"Stone", 3 }
                },
                Steps = 5,
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

        public Construction.Building GetBuilding(string name)
        {
            if (buildings.ContainsKey(name))
            {
                return buildings[name];
            }
            return null;
        }
    }
}
