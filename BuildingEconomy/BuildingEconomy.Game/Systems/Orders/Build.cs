using Akka.Actor;
using BuildingEconomy.Components;
using BuildingEconomy.Systems.Construction.Messages;
using BuildingEconomy.Systems.Orders.Interfaces;
using Xenko.Engine;
using Xenko.Games;

namespace BuildingEconomy.Systems.Orders
{
    public class Build : IOrder
    {
        private readonly ComponentActorFactory<ConstructionSite> componentActorFactory;
        private readonly ConstructionSite target;

        public Build(ConstructionSite target, ComponentActorFactory<ConstructionSite> componentActorFactory)
        {
            this.target = target;
            this.componentActorFactory = componentActorFactory;
        }

        public bool IsComplete(Entity entity)
        {
            return target.CurrentStageProgress >= 1.0f;
        }

        public bool IsValid(Entity entity)
        {
            return entity.Get<Builder>() != null;
        }

        public void Update(Entity entity, GameTime updateTime)
        {
            if (IsComplete(entity) || !IsValid(entity))
            {
                return;
            }

            // TODO: Move to entity if not near.
            IActorRef targetActor = componentActorFactory.GetOrCreateActorForComponent(target);

            targetActor.Tell(new AdvanceProgress());
        }
    }
}
