using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xenko.Engine;
using Xenko.Games;

namespace BuildingEconomy.Systems.Orders.Interfaces
{
    interface IOrder
    {
        bool IsValid(Entity entity);
        bool IsComplete { get; }
        void Update(Entity entity, GameTime updateTime);
    }
}
