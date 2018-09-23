using Akka.Actor;
using BuildingEconomy.Components;
using BuildingEconomy.Systems.Construction.Messages;
using BuildingEconomy.Systems.Messages;
using System.Linq;
using Xenko.Engine;

namespace BuildingEconomy.Systems.Construction
{
    public class SystemActor : BasicSystemActor<ConstructionSystem>
    {
        public SystemActor(ConstructionSystem system) : base(system)
        {
        }

        protected override void Default()
        {
            base.Default();
            Receive<BuilderNeeded>(msg =>
            {
                // TODO: Set orders for builders.
                Context.Parent.Tell(msg);
            });
            Receive<ConstructionFinished>(msg => HandleConstructionFinished(msg));
            Receive<WaitingForResources>(msg => Context.Parent.Tell(msg));
        }

        public static Props Props(ConstructionSystem system)
        {
            return Akka.Actor.Props.Create(() => new SystemActor(system));
        }

        protected void HandleConstructionFinished(ConstructionFinished message)
        {
            Entity entity = System.EntityManager.SingleOrDefault(e => e.Id == message.EntityId);
            if (entity is null)
            {
                // TODO: Log error/warning.
                Sender.Tell(new CouldNotProcessMessage(message, CouldNotProcessMessage.EntityNotFound));
                return;
            }
            entity.RemoveAll<ConstructionSite>();
            var building = new Components.Building
            {
                Name = message.Building
            };
            entity.Add(building);
            // TODO: Add building specific components.
        }
    }
}