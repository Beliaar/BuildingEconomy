using Akka.Actor;
using Xenko.Engine;
using Xenko.Games;

namespace BuildingEconomy.Systems
{
    public abstract class BasicSystem<T> : IGameSystemBase where T : BasicSystem<T>
    {
        protected BasicSystem(EntityManager entityManager)
        {
            EntityManager = entityManager;
        }

        public EntityManager EntityManager { get; }
        public abstract IActorRef Actor { get; }
        public int ReferenceCount { get; protected set; }

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
    }
}
