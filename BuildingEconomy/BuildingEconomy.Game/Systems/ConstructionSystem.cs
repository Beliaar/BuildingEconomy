using Akka.Actor;
using System.Collections.Generic;
using System.Linq;
using Xenko.Engine;

namespace BuildingEconomy.Systems
{
    internal class ConstructionSystem : BasicSystem
    {
        public override string Name => "Construction";

        private readonly Dictionary<string, Construction.Building> buildings = new Dictionary<string, Construction.Building>();

        private readonly Construction.Actor constructionActor;

        public ConstructionSystem(SceneInstance scene) : base(scene)
        {
            Receive<Construction.Messages.BuilderNeeded>(msg =>
            {
                scene.SelectMany(e => e.Where(c => c.Id == msg.ComponentId)).FirstOrDefault();
            });
            Receive<Construction.Messages.MessageToConstructionSite>(msg => HandleMessageToConstructionSite(msg));
        }

        protected override void PreStart()
        {
            base.PreStart();
        }

        public override void Initialize()
        {
            var warehouse = new Construction.Building()
            {
                Name = "Warehouse"
            };

            warehouse.Stages.Add(new Construction.Building.Stage
            {
                NeededRessources = new Dictionary<string, int>
                {
                    {"Wood", 4 },
                },
                StepProgress = 0.25f,
            });
            warehouse.Stages.Add(new Construction.Building.Stage
            {
                NeededRessources = new Dictionary<string, int>
                {
                    {"Wood", 2 },
                    {"Stone", 4 }
                },
                StepProgress = 0.15f,
            });
            warehouse.Stages.Add(new Construction.Building.Stage
            {
                NeededRessources = new Dictionary<string, int>
                {
                    {"Wood", 2 },
                    {"Stone", 3 }
                },
                StepProgress = 0.2f,
            });
            AddBuilding(warehouse);
        }

        public void AddBuilding(Construction.Building building)
        {
            if (!buildings.ContainsKey(building.Name))
            {
                buildings.Add(building.Name, building);
            }
        }

        public void HandleMessageToConstructionSite(Construction.Messages.MessageToConstructionSite message)
        {
            Components.ConstructionSite constructionSite = Scene.SingleOrDefault(e => e.Id == message.EntityId)?.Get<Components.ConstructionSite>();
            if (constructionSite is null)
            {
                return;
            }
            ForwardMessageToSite(message, constructionSite);
        }

        /// <summary>
        /// Returns the actor for that construction site
        /// </summary>
        /// <param name="constructionSite"></param>
        /// <returns></returns>
        private IActorRef GetOrCreateActor(Components.ConstructionSite constructionSite)
        {
            string actorName = $"{constructionSite.Id}";
            IActorRef actor = Context.Child(actorName);
            if (actor == ActorRefs.Nobody)
            {
                Construction.Building building = buildings[constructionSite.Building];
                actor = Context.ActorOf(Construction.Actor.Props(constructionSite, building), actorName);
            }

            return actor;
        }

        public void HandleConstructionFinished(Construction.Messages.ConstructionFinished message)
        {
            Entity entity = Scene.SingleOrDefault(e => e.Id == message.EntityId);
            entity.RemoveAll<Components.ConstructionSite>();
            var building = new Components.Building
            {
                Name = message.Building
            };
            entity.Add(building);
            // TODO: Add building specific components.
        }

        public override void HandleStep(Messages.Update message)
        {
            IEnumerable<Components.ConstructionSite> constructionSites = Scene.SelectMany(si => si.Components.OfType<Components.ConstructionSite>());

            foreach (Components.ConstructionSite constructionSite in constructionSites)
            {
                ForwardMessageToSite(message, constructionSite);
            }
        }

        private void ForwardMessageToSite(object message, Components.ConstructionSite constructionSite)
        {
            GetOrCreateActor(constructionSite).Tell(message);
        }
    }
}
