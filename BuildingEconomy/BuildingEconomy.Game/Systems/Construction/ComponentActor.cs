using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using Xenko.Engine;
using Xenko.Games;

namespace BuildingEconomy.Systems.Construction
{

    /// <summary>
    /// Akka Actor for handling messages from and to construction sites.
    /// </summary>
    public class ComponentActor : ReceiveActor
    {
        public static double SecondsBetweenUpdate = 5.0;

        public Components.ConstructionSite ConstructionSite { get; }
        public Building Building { get; }
        protected double elapsedTime;

        public ComponentActor(Components.ConstructionSite constructionSite, Building building)
        {
            ConstructionSite = constructionSite;
            Building = building;
            Become(Preparing);
        }

        public static Props Props(Components.ConstructionSite constructionSite, Building building)
        {
            return Akka.Actor.Props.Create(() => new ComponentActor(constructionSite, building));
        }

        protected void Preparing()
        {
            elapsedTime = 0.0f;
            ConstructionSite.CurrentStage = -1;
            Receive<Systems.Messages.Update>(msg => HandleUpdatePreparing(msg));
            Receive<Messages.AdvanceProgress>(msg => HandleAdvanceProgressPreparing(msg));
        }

        protected void Constructing()
        {

        }

        protected void WaitingForResources()
        {
            elapsedTime = 0.0f;
            Receive<Systems.Messages.Update>(msg => HandleUpdateWaitingforResources(msg));
        }

        protected void DoIfTimePassed(GameTime gameTime, Action action)
        {
            elapsedTime += gameTime.Elapsed.TotalSeconds;
            if (elapsedTime >= SecondsBetweenUpdate)
            {
                action();
                elapsedTime = 0.0;
            }
        }

        protected void HandleUpdatePreparing(Systems.Messages.Update message)
        {
            IActorRef sender = Sender;
            DoIfTimePassed(message.UpdateTime, () => sender.Tell(new Messages.BuilderNeeded(ConstructionSite.Id)));
            if (ConstructionSite.CurrentStageProgress >= 1.0f)
            {
                ConstructionSite.CurrentStage = 0;
                ConstructionSite.Entity.Add(new Components.Storage());
                Become(WaitingForResources);
            }
        }

        protected void HandleUpdateWaitingforResources(Systems.Messages.Update message)
        {
            IActorRef sender = Sender;
            DoIfTimePassed(message.UpdateTime, () => sender.Tell(new Messages.WaitingForResources(ConstructionSite.Id)));
        }

        protected void SetNeededResources()
        {
            Components.Storage storage = ConstructionSite.Entity.Get<Components.Storage>();
            var neededResources = new Dictionary<string, int>();
            int nextStageIndex = ConstructionSite.CurrentStage - 1;
            bool hasResourcesForCurrentStage = true;
            bool hasResourcesForAllStages = false;
            while (hasResourcesForCurrentStage && !hasResourcesForAllStages)
            {
                if (nextStageIndex < Building.Stages.Count)
                {
                    Building.Stage stage = Building.Stages[nextStageIndex - 1];
                    foreach (KeyValuePair<string, int> resource in stage.NeededRessources)
                    {
                        if (!neededResources.ContainsKey(resource.Key))
                        {
                            neededResources[resource.Key] = 0;
                        }
                        if (!storage.Items.ContainsKey(resource.Key) || storage.Items[resource.Key] < (resource.Value + neededResources[resource.Key]))
                        {
                            hasResourcesForCurrentStage = false;
                        }
                        neededResources[resource.Key] += resource.Value;
                    }
                    nextStageIndex++;
                }
                else
                {
                    hasResourcesForAllStages = true;
                }
            }
            storage.RequestedItems.Clear();
            storage.RequestedItems.Concat(neededResources).ToDictionary(p => p.Key, p => p.Value);

        }

        public void HandleAdvanceProgressPreparing(Messages.AdvanceProgress message)
        {
            // This will probably be things like leveling the ground and such later. Everything that needs to be done before the actual building starts and that does not need items.
            ConstructionSite.CurrentStageProgress += 0.25f;
        }

        /// <summary>
        /// Add a step to the progress of the construction site. Checks if the site is allowed to progress first.
        /// </summary>
        /// <param name="construction"></param>
        public void HandleAdvanceProgress(Messages.AdvanceProgress message)
        {
            Entity entity = ConstructionSite.Entity;
            Building.Stage stage = Building.Stages[ConstructionSite.CurrentStage - 1];
            Components.Storage storage = entity.Components.Get<Components.Storage>();
            if (storage is null)
            {
                return;
            }
            foreach (string resource in stage.NeededRessources.Keys)
            {
                // Check if all resources are at or higher than the needed level.
                if (!storage.Items.ContainsKey(resource) || storage.Items[resource] < stage.NeededRessources[resource])
                {
                    return;
                }
            }
            ConstructionSite.CurrentStageProgress += stage.StepProgress;
        }
    }
}
