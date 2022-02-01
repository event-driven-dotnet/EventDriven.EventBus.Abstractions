using System.Threading.Tasks;

namespace EventDriven.EventBus.Abstractions.Tests.Fakes
{
    public class FakeEventHandler2 : IntegrationEventHandler<FakeIntegrationEvent>
    {
        public FakeState State { get; }

        public FakeEventHandler2(FakeState state)
        {
            State = state;
        }

        public override Task HandleAsync(FakeIntegrationEvent @event)
        {
            // Mutate State Date
            State.Date = @event.CreationDate;
            State.Value++;
            return Task.CompletedTask;
        }
    }
}
