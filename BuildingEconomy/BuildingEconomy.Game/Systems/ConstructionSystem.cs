using Akka.Actor;
using System.Collections.Generic;
using Xenko.Engine;

namespace BuildingEconomy.Systems
{
    public class ConstructionSystem : BasicSystem<ConstructionSystem>, Construction.Interfaces.IBuildingManager
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
