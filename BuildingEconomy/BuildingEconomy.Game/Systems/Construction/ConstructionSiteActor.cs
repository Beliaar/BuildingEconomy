using System;
using System.Collections.Generic;
using Akka.Actor;
using BuildingEconomy.Components;
using BuildingEconomy.Systems.Actors;
using BuildingEconomy.Systems.Construction.Messages;
using BuildingEconomy.Systems.Messages;
using Xenko.Engine;
using Xenko.Games;

namespace BuildingEconomy.Systems.Construction
{
    /// <summary>
    ///     Akka Actor for handling messages from and to construction sites.
    /// </summary>
    public class ConstructionSiteActor : ComponentActor<ConstructionSite>
    {
        public static double SecondsBetweenUpdate = 5.0;
        protected double ElapsedTime;

        public ConstructionSiteActor(ConstructionSite constructionSite, Building building) : base(constructionSite)
        {
            Building = building;
            Become(Preparing);
        }

        public ConstructionSite ConstructionSite => Component;
        public Building Building { get; }

        public static Props Props(ConstructionSite constructionSite, Building building)
        {
            return Akka.Actor.Props.Create(() => new ConstructionSiteActor(constructionSite, building));
        }

        protected void Preparing()
        {
            ElapsedTime = 0.0f;
            ConstructionSite.CurrentStage = 0;
            Receive<Update>(msg => HandleUpdatePreparing(msg));
            Receive<AdvanceProgress>(msg => HandleAdvanceProgressPreparing(msg));
        }

        protected void Constructing()
        {
            ElapsedTime = 0.0f;
            Receive<Update>(msg => HandleUpdateConstructing(msg));
            Receive<AdvanceProgress>(msg => HandleAdvanceProgressConstructing(msg));
        }

        protected void ConstructionFinished()
        {
            Receive<Update>(msg =>
            {
                DoIfTimePassed(msg.UpdateTime,
                    () => { Sender.Tell(new ConstructionFinished(ConstructionSite.Entity.Id, Building.Name)); });
            });
        }

        protected void WaitingForResources()
        {
            ElapsedTime = 0.0f;
            Receive<Update>(msg => HandleUpdateWaitingForResources(msg));
        }

        protected void DoIfTimePassed(GameTime gameTime, Action action)
        {
            ElapsedTime += gameTime.Elapsed.TotalSeconds;
            if (ElapsedTime >= SecondsBetweenUpdate)
            {
                action();
                ElapsedTime = 0.0;
            }
        }

        protected void HandleUpdatePreparing(Update message)
        {
            if (ConstructionSite.CurrentStageProgress >= 1.0f)
            {
                ConstructionSite.CurrentStage = 1;
                ConstructionSite.CurrentStageProgress = 0f;
                ConstructionSite.Entity.Add(new Storage());
                Become(WaitingForResources);
                Self.Forward(message);
            }
            else
            {
                IActorRef sender = Sender;
                DoIfTimePassed(message.UpdateTime, () => sender.Tell(new BuilderNeeded(ConstructionSite.Id)));
            }
        }

        protected void HandleUpdateWaitingForResources(Update message)
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
                    sender.Tell(new WaitingForResources(ConstructionSite.Id));
                });
            }
        }

        protected void SetNeededResources()
        {
            var storage = ConstructionSite.Entity.Get<Storage>();
            var neededResources = new Dictionary<string, int>();
            int nextStageIndex = ConstructionSite.CurrentStage - 1;
            var hasResourcesForCurrentStage = true;
            var hasResourcesForAllStages = false;
            while (hasResourcesForCurrentStage && !hasResourcesForAllStages)
            {
                if (nextStageIndex < Building.Stages.Count)
                {
                    Building.Stage stage = Building.Stages[nextStageIndex];
                    foreach (KeyValuePair<string, int> resource in stage.NeededResources)
                    {
                        if (!neededResources.ContainsKey(resource.Key))
                        {
                            neededResources[resource.Key] = 0;
                        }

                        if (!storage.Items.ContainsKey(resource.Key) || storage.Items[resource.Key] <
                            resource.Value + neededResources[resource.Key])
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

        public void HandleAdvanceProgressPreparing(AdvanceProgress message)
        {
            // This will probably be things like leveling the ground and such later. Everything that needs to be done before the actual building starts and that does not need items.
            ConstructionSite.CurrentStageProgress += 0.25f;
        }

        /// <summary>
        ///     Add a step to the progress of the construction site. Checks if the site is allowed to progress first.
        /// </summary>
        /// <param name="message"></param>
        public void HandleAdvanceProgressConstructing(AdvanceProgress message)
        {
            Building.Stage stage = Building.Stages[ConstructionSite.CurrentStage - 1];
            ConstructionSite.CurrentStageProgress += 1.0f / stage.Steps;
        }

        public void HandleUpdateConstructing(Update message)
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
                    sender.Tell(new BuilderNeeded(ConstructionSite.Id));
                });
            }
        }

        private bool HasNeededResourcesForStage(Building.Stage stage)
        {
            Entity entity = ConstructionSite.Entity;
            var storage = entity.Components.Get<Storage>();
            foreach (string resource in stage.NeededResources.Keys)
                // Check if all resources are at or higher than the needed level.
            {
                if (!storage.Items.ContainsKey(resource) || storage.Items[resource] < stage.NeededResources[resource])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
