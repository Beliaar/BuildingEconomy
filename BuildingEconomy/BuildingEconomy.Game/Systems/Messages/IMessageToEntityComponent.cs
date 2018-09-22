namespace BuildingEconomy.Systems.Messages
{
    public interface IMessageToEntityComponent : IMessageToEntity
    {
        object Message { get; }
    }
}