using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xenko.Engine;

namespace BuildingEconomy.Systems.Interfaces
{
    public interface IComponentActorFactory
    {
        IActorRef GetOrCreateActorForComponent(EntityComponent component);
    }
}
