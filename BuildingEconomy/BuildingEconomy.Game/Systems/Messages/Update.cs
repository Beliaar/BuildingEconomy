using Xenko.Games;

namespace BuildingEconomy.Systems.Messages
{
    public class Update
    {
        public Update(GameTime updateTime)
        {
            UpdateTime = updateTime;
        }

        public GameTime UpdateTime { get; }
    }
}
