using Akka.Actor;
using BuildingEconomy.Systems.Construction;
using BuildingEconomy.Systems.Construction.Interfaces;
using System.Collections.Generic;
using Xenko.Engine;

namespace BuildingEconomy.Systems
{
    public class ConstructionSystem : BasicSystem<ConstructionSystem>, IBuildingManager
    {
        private readonly Dictionary<string, Building> buildings = new Dictionary<string, Building>();

        public ConstructionSystem(EntityManager entityManager, IActorRefFactory actorRefFactory) : base(entityManager)
        {
            Actor = actorRefFactory.ActorOf(SystemActor.Props(this), "ConstructionSystem");
        }

        public override string Name => "Construction";
        public override IActorRef Actor { get; }

        public Building GetBuilding(string name)
        {
            if (buildings.ContainsKey(name))
            {
                return buildings[name];
            }

            return null;
        }

        public override void Initialize()
        {
        }

        public void AddBuilding(Building building)
        {
            if (!buildings.ContainsKey(building.Name))
            {
                buildings.Add(building.Name, building);
            }
        }
    }
}