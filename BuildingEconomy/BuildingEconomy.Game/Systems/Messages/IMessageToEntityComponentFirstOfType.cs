using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingEconomy.Systems.Messages
{
    public interface IMessageToEntityComponentFirstOfType : IMessageToEntityComponent
    {
        Type ComponentType { get; }
    }
}
