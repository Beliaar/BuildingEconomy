using Akka.Actor;

namespace BuildingEconomy.Systems.Construction
{
    public class ConstructionSiteActorFactory : ComponentActorFactory<Components.ConstructionSite>
    {
        private readonly Interfaces.IBuildingManager buildingManager;

        public ConstructionSiteActorFactory(Interfaces.IBuildingManager buildingManager, IActorContext factoryContext) : base(factoryContext)
        {
            this.buildingManager = buildingManager;
        }

        public override Props GetProps(Components.ConstructionSite component)
        {
            return ConstructionSiteActor.Props(component, buildingManager.GetBuilding(component.Building));
        }
    }
}
