using System;
using Akka.Actor;
using BuildingEconomy.Systems.Interfaces;
using Xenko.Engine;

namespace BuildingEconomy.Systems
{
    public abstract class ComponentActorFactory<T> : IComponentActorFactory where T : EntityComponent
    {
        private readonly IActorRefFactory actorRefFactory;

        protected ComponentActorFactory(IActorRefFactory actorRefFactory)
        {
            this.actorRefFactory = actorRefFactory;
        }

        public IActorRef GetOrCreateActorForComponent(EntityComponent component)
        {
            if (!(component is T asT))
            {
                throw new ArgumentException(
                    $"Wrong component type. Expected {typeof(T).FullName} got {component.GetType().Name}");
            }

            return GetOrCreateActorForComponent(asT);
        }

        public IActorRef GetOrCreateActorForComponent(T component)
        {
            string actorName = $"Component_{component.Id}";
            IActorRef componentActor;
            try
            {
                componentActor = actorRefFactory.ActorSelection($"user/{actorName}").ResolveOne(TimeSpan.FromSeconds(1))
                    .Result;
            }
            catch (AggregateException exception) when (exception.InnerException is ActorNotFoundException)
            {
                componentActor = ActorRefs.Nobody;
            }

            return !componentActor.IsNobody()
                ? componentActor
                : actorRefFactory.ActorOf(GetProps(component), actorName);
        }

        public abstract Props GetProps(T component);
    }
}
