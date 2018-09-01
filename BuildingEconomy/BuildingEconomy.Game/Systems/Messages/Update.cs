using Xenko.Games;

namespace BuildingEconomy.Systems.Messages
{
    public class Update
    {
        public GameTime UpdateTime { get; }

        public Update(GameTime updateTime)
        {
            UpdateTime = updateTime;
        }
    }
}
