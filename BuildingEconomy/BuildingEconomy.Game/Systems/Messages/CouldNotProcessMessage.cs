namespace BuildingEconomy.Systems.Messages
{
    public class CouldNotProcessMessage
    {
        public static string EntityNotFound = "Entity not found";

        public CouldNotProcessMessage(object message, string reason)
        {
            Message = message;
            Reason = reason;
        }

        public object Message { get; }
        public string Reason { get; }
    }
}
