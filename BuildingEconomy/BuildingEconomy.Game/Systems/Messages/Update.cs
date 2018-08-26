using Xenko.Games;

namespace BuildingEconomy.Systems.Messages
{
    internal class Update
    {
        public GameTime UpdateTime { get; }

        public Update(GameTime updateTime)
        {
            UpdateTime = updateTime;
        }
    }
}
