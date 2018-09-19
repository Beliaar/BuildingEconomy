using Akka.Actor;
using Xenko.Engine;
using Xenko.Games;

namespace BuildingEconomy.Systems
{
    public abstract class BasicSystem<T> : IGameSystemBase where T : BasicSystem<T>
    {
        public EntityManager EntityManager { get; }
        public int ReferenceCount { get; protected set; }
        public abstract IActorRef Actor { get; }

        public BasicSystem(EntityManager entityManager)
        {
            EntityManager = entityManager;
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
    }
}
