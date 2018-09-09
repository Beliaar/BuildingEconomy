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
    public class ConstructionSiteActor : Actors.ComponentActor<Components.ConstructionSite>
    {
        public static double SecondsBetweenUpdate = 5.0;

        public Components.ConstructionSite ConstructionSite => Component as Components.ConstructionSite;
        public Building Building { get; }
        protected double elapsedTime;

        public ConstructionSiteActor(Components.ConstructionSite constructionSite, Building building) : base(constructionSite)
        {
            Building = building;
            Become(Preparing);
        }

        public static Props Props(Components.ConstructionSite constructionSite, Building building)
        {
            return Akka.Actor.Props.Create(() => new ConstructionSiteActor(constructionSite, building));
        }

        protected void Preparing()
        {
            elapsedTime = 0.0f;
            ConstructionSite.CurrentStage = 0;
            Receive<Systems.Messages.Update>(msg => HandleUpdatePreparing(msg));
            Receive<Messages.AdvanceProgress>(msg => HandleAdvanceProgressPreparing(msg));
        }

        protected void Constructing()
        {
            elapsedTime = 0.0f;
            Receive<Systems.Messages.Update>(msg => HandleUpdateConstructing(msg));
            Receive<Messages.AdvanceProgress>(msg => HandleAdvanceProgressConstructing(msg));
        }

        protected void ConstructionFinished()
        {
            Receive<Systems.Messages.Update>(msg =>
            {
                DoIfTimePassed(msg.UpdateTime, () =>
                {
                    Sender.Tell(new Messages.ConstructionFinished(ConstructionSite.Entity.Id, Building.Name));
                });
            });
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
            if (ConstructionSite.CurrentStageProgress >= 1.0f)
            {
                ConstructionSite.CurrentStage = 1;
                ConstructionSite.CurrentStageProgress = 0f;
                ConstructionSite.Entity.Add(new Components.Storage());
                Become(WaitingForResources);
                Self.Forward(message);
            }
            else
            {
                IActorRef sender = Sender;
                DoIfTimePassed(message.UpdateTime, () => sender.Tell(new Messages.BuilderNeeded(ConstructionSite.Id)));
            }
        }

        protected void HandleUpdateWaitingforResources(Systems.Messages.Update message)
        {
            IActorRef sender = Sender;
            if (HasNeededResourcesForStage(Building.Stages[ConstructionSite.CurrentStage - 1]))
            {
                Become(Constructing);
                Self.Forward(message);
            }
            else
            {
                DoIfTimePassed(message.UpdateTime, () =>
                {
                    SetNeededResources();
                    sender.Tell(new Messages.WaitingForResources(ConstructionSite.Id));
                });
            }
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
                    Building.Stage stage = Building.Stages[nextStageIndex];
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
            foreach (KeyValuePair<string, int> pair in neededResources)
            {
                storage.RequestedItems[pair.Key] = pair.Value;
            }

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
        public void HandleAdvanceProgressConstructing(Messages.AdvanceProgress message)
        {
            Building.Stage stage = Building.Stages[ConstructionSite.CurrentStage - 1];
            ConstructionSite.CurrentStageProgress += 1.0f / stage.Steps;
        }

        public void HandleUpdateConstructing(Systems.Messages.Update message)
        {
            if (ConstructionSite.CurrentStageProgress >= 1.0f)
            {
                if (ConstructionSite.CurrentStage < Building.Stages.Count)
                {
                    ++ConstructionSite.CurrentStage;
                    ConstructionSite.CurrentStageProgress = 0.0f;
                    Become(WaitingForResources);
                    Self.Forward(message);
                }
                else
                {
                    Become(ConstructionFinished);
                    Self.Forward(message);
                }
            }
            else
            {
                IActorRef sender = Sender;
                DoIfTimePassed(message.UpdateTime, () =>
                {
                    SetNeededResources();
                    sender.Tell(new Messages.BuilderNeeded(ConstructionSite.Id));
                });
            }

        }

        private bool HasNeededResourcesForStage(Building.Stage stage)
        {
            Entity entity = ConstructionSite.Entity;
            Components.Storage storage = entity.Components.Get<Components.Storage>();
            foreach (string resource in stage.NeededRessources.Keys)
            {
                // Check if all resources are at or higher than the needed level.
                if (!storage.Items.ContainsKey(resource) || storage.Items[resource] < stage.NeededRessources[resource])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
