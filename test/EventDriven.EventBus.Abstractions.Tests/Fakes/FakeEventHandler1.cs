using System.Threading.Tasks;

namespace EventDriven.EventBus.Abstractions.Tests.Fakes
{
    public class FakeEventHandler1 : IntegrationEventHandler<FakeIntegrationEvent>
    {
        public FakeState State { get; }

        public FakeEventHandler1(FakeState state)
        {
            State = state;
        }

        public override Task HandleAsync(FakeIntegrationEvent @event)
        {
            // Mutate State Data
            State.Data = @event.Data;
            State.Value++;
            return Task.CompletedTask;
        }
    }
}
