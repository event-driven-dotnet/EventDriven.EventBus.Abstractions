using System.Threading.Tasks;

namespace EventDriven.EventBus.Abstractions.Tests.Fakes
{
    public class FakeEventHandler3 : IntegrationEventHandler<FakeIntegrationEvent>
    {
        public FakeState State { get; }

        public FakeEventHandler3(FakeState state)
        {
            State = state;
        }

        public override Task HandleAsync(FakeIntegrationEvent @event)
        {
            // Mutate State Value
            State.Value++;
            return Task.CompletedTask;
        }
    }
}
