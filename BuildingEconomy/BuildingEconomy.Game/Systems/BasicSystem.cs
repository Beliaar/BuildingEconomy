using Akka.Actor;
using BuildingEconomy.Systems.Messages;
using Xenko.Engine;
using Xenko.Games;

namespace BuildingEconomy.Systems
{
    internal abstract class BasicSystem : ReceiveActor, IGameSystemBase
    {
        protected SceneInstance Scene { get; }
        public int ReferenceCount { get; protected set; }

        public BasicSystem(SceneInstance scene)
        {
            Scene = scene;
            Receive<Update>((message) => HandleStep(message));
        }

        public abstract string Name { get; }

        public int AddReference()
        {
            return ++ReferenceCount;
        }

        public int Release()
        {
            return --ReferenceCount;
        }

        public abstract void Initialize();
        public abstract void HandleStep(Update message);
    }
}
