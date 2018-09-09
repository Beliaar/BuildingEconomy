using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Xenko.Engine;

namespace BuildingEconomy.Systems
{
    abstract class ComponentActorFactory<T> : Interfaces.IComponentActorFactory where T : EntityComponent
    {
        public IActorRef GetOrCreateActorForComponent(T component, IActorContext context)
        {
            string actorName = $"Component_{component.Id}";
            IActorRef componentActor = context.Child(actorName);
            return !componentActor.IsNobody() ? 
                componentActor 
                :
                context.ActorOf(GetProps(component), actorName);
        }

        public IActorRef GetOrCreateActorForComponent(EntityComponent component, IActorContext context)
        {
            var asT = component as T;
            if (asT is null)
            {
                throw new ArgumentException($"Wrong component type. Expected {nameof(T)} got {component.GetType().Name}");
            }
            return GetOrCreateActorForComponent(asT, context);
        }

        public abstract Props GetProps(T component);
    }
}
