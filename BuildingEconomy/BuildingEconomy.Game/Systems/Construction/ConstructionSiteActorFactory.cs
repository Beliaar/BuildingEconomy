using Akka.Actor;
using BuildingEconomy.Systems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xenko.Engine;

namespace BuildingEconomy.Systems.Construction
{
    class ConstructionSiteActorFactory : ComponentActorFactory<Components.ConstructionSite>
    {
        readonly Interfaces.IBuildingManager buildingManager;

        public ConstructionSiteActorFactory(Interfaces.IBuildingManager buildingManager)
        {
            this.buildingManager = buildingManager;
        }

        public override Props GetProps(Components.ConstructionSite component)
        {
            return ConstructionSiteActor.Props(component, buildingManager.GetBuilding(component.Building));
        }
    }
}
