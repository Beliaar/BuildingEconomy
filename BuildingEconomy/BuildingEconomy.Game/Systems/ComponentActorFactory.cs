using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Xenko.Engine;

namespace BuildingEconomy.Systems
{
    public abstract class ComponentActorFactory<T> : Interfaces.IComponentActorFactory where T : EntityComponent
    {
        private readonly IActorRefFactory actorRefFactory;

        public ComponentActorFactory(IActorRefFactory actorRefFactory)
        {
            this.actorRefFactory = actorRefFactory;
        }

        public IActorRef GetOrCreateActorForComponent(T component)
        {
            string actorName = $"Component_{component.Id}";
            IActorRef componentActor;
            try
            {
                componentActor = actorRefFactory.ActorSelection($"user/{actorName}").ResolveOne(TimeSpan.FromSeconds(1)).Result;
            }
            catch (AggregateException exception) when (exception.InnerException is ActorNotFoundException)
            {
                componentActor = ActorRefs.Nobody;
            }
            
            return !componentActor.IsNobody() ? 
                componentActor 
                :
                actorRefFactory.ActorOf(GetProps(component), actorName);
        }

        public IActorRef GetOrCreateActorForComponent(EntityComponent component)
        {
            var asT = component as T;
            if (asT is null)
            {
                throw new ArgumentException($"Wrong component type. Expected {typeof(T).FullName} got {component.GetType().Name}");
            }
            return GetOrCreateActorForComponent(asT);
        }

        public abstract Props GetProps(T component);
    }
}
