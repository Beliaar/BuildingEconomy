using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingEconomy.Systems.Construction.Interfaces
{
    interface IBuildingManager
    {
        Building GetBuilding(string name);
    }
}
