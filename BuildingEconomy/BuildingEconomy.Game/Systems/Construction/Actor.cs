using Akka.Actor;
using Xenko.Engine;

namespace BuildingEconomy.Systems.Construction
{
    internal class Actor : ReceiveActor
    {
        public static double SecondsBetweenUpdate = 5.0;

        public enum State
        {
            Preparing,
            WaitingForResources,
            Constructing,
            ConstructionDong
        }

        public Components.ConstructionSite ConstructionSite { get; }
        public Building Building { get; }
        public State ConstructionState { get; protected set; }
        protected double elapsedTime;

        public Actor(Components.ConstructionSite constructionSite, Building building)
        {
            ConstructionSite = constructionSite;
            Building = building;
            Become(Preparing);
        }

        public static Props Props(Components.ConstructionSite constructionSite, Building building)
        {
            return Akka.Actor.Props.Create(() => new Actor(constructionSite, building));
        }

        protected void Preparing()
        {
            elapsedTime = 0.0f;
            ConstructionState = State.Preparing;
            Receive<Systems.Messages.Update>(msg => HandleUpdatePreparing(msg));
            Receive<Messages.AdvanceProgress>(msg => HandleAdvanceProgressPreparing(msg));
        }

        protected void WaitingForResources()
        {
            ConstructionState = State.WaitingForResources;
        }

        protected void HandleUpdatePreparing(Systems.Messages.Update message)
        {
            elapsedTime += message.UpdateTime.Elapsed.TotalSeconds;
            if (elapsedTime > SecondsBetweenUpdate)
            {
                Context.Parent.Tell(new Messages.BuilderNeeded(ConstructionSite.Id));
                elapsedTime = 0.0;
            }
        }

        public void HandleAdvanceProgressPreparing(Messages.AdvanceProgress message)
        {

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
