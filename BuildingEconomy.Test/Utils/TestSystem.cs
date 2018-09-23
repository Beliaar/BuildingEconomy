using BuildingEconomy.Systems;
using Xenko.Engine;

namespace BuildingEconomy.Test.Utils
{
    public abstract class TestSystem : BasicSystem<TestSystem>
    {
        protected TestSystem(EntityManager entityManager) : base(entityManager)
        {
        }
    }
}
