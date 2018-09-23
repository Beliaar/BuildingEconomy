using Akka.Actor;
using BuildingEconomy.Systems;
using BuildingEconomy.Systems.Messages;

namespace BuildingEconomy.Test.Utils
{
    public class TestSystemActor : BasicSystemActor<TestSystem>
    {

        public static Props Props(TestSystem system)
        {
            return Akka.Actor.Props.Create(() => new TestSystemActor(system));
        }

        public TestSystemActor(TestSystem system) : base(system)
        {
        }

        protected override void HandleStep(Update message)
        {
            base.HandleStep(message);
            Sender.Tell(new TestUpdateRespondMessage());
        }
    }
}
