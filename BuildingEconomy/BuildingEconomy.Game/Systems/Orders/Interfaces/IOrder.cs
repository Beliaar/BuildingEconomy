using Xenko.Engine;
using Xenko.Games;

namespace BuildingEconomy.Systems.Orders.Interfaces
{
    /// <summary>
    /// An order for an entity.
    /// </summary>
    public interface IOrder
    {
        /// <summary>
        /// Wether the given entity is able to perform the order in its CURRENT state.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool IsValid(Entity entity);
        /// <summary>
        /// Whether the order is complete.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool IsComplete(Entity entity);
        /// <summary>
        /// Performs actions to complete the order on the given entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateTime"></param>
        void Update(Entity entity, GameTime updateTime);
    }
}
