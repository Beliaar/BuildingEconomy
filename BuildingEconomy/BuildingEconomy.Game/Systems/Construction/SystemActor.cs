using Akka.Actor;
using System.Collections.Generic;
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
            Receive<Messages.BuilderNeeded>(msg =>
            {
                System.EntityManager.SelectMany(e => e.Where(c => c.Id == msg.ComponentId)).FirstOrDefault();
                Context.Parent.Tell(msg);
            });
            Receive<Messages.ConstructionFinished>(msg => HandleConstructionFinished(msg));
            Receive<Messages.WaitingForResources>(msg => Context.Parent.Tell(msg));
        }

        public static Props Props(ConstructionSystem system)
        {
            return Akka.Actor.Props.Create(() => new SystemActor(system));
        }


        public override void HandleStep(Systems.Messages.Update message)
        {
            IEnumerable<Components.ConstructionSite> constructionSites = System.EntityManager.SelectMany(e => e.Components.OfType<Components.ConstructionSite>());

            foreach (Components.ConstructionSite constructionSite in constructionSites)
            {
                ForwardMessageToComponent(message, constructionSite, Self);
            }
        }

        /// <summary>
        /// Returns the actor for that construction site
        /// </summary>
        /// <param name="constructionSite"></param>
        /// <returns></returns>
        protected override IActorRef GetOrCreateActor<C>(C component)
        {
            var constructionSite = component as Components.ConstructionSite;
            return constructionSite is null ?
                    ActorRefs.Nobody
                    :
                    GetOrCreateActor(
                    constructionSite,
                    ComponentActor.Props(
                        constructionSite,
                        System.GetBuilding(constructionSite.Building)
                        )
                );
        }

        public void HandleConstructionFinished(Messages.ConstructionFinished message)
        {
            Entity entity = System.EntityManager.SingleOrDefault(e => e.Id == message.EntityId);
            entity.RemoveAll<Components.ConstructionSite>();
            var building = new Components.Building
            {
                Name = message.Building
            };
            entity.Add(building);
            // TODO: Add building specific components.
        }


    }
}
