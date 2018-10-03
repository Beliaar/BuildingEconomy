using Akka.Actor;
using BuildingEconomy.Components;
using BuildingEconomy.Systems.Construction.Interfaces;

namespace BuildingEconomy.Systems.Construction
{
    public class ConstructionSiteActorFactory : ComponentActorFactory<ConstructionSite>
    {
        private readonly IBuildingManager buildingManager;

        public ConstructionSiteActorFactory(IBuildingManager buildingManager, IActorRefFactory factoryContext) : base(
            factoryContext)
        {
            this.buildingManager = buildingManager;
        }

        public override Props GetProps(ConstructionSite component)
        {
            return ConstructionSiteActor.Props(component, buildingManager.GetBuilding(component.Building));
        }
    }
}
