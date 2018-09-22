using Xenko.Engine;

namespace BuildingEconomy
{
    internal class BuildingEconomyApp
    {
        private static void Main(string[] args)
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}