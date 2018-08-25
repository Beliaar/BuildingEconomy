using Xenko.Engine;
using Xenko.Games;

namespace BuildingEconomy.Systems
{
    internal abstract class BasicSystem : IGameSystemBase
    {
        protected Game Game { get; }
        public int ReferenceCount { get; protected set; }

        public BasicSystem(Game game)
        {
            Game = game;
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
        public abstract void Step();
    }
}
