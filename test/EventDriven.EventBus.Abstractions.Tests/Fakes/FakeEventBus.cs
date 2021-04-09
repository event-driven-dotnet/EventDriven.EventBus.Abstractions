using System.Threading.Tasks;

namespace EventDriven.EventBus.Abstractions.Tests.Fakes
{
    public class FakeEventBus : EventBus
    {
        private readonly FakeMessageBroker _messageBroker;

        public FakeEventBus(FakeMessageBroker messageBroker)
        {
            _messageBroker = messageBroker;
        }

        public override async Task PublishAsync<TIntegrationEvent>(
            TIntegrationEvent @event,
            string topic = null,
            string prefix = null)
        {
            var topicName = topic ?? @event.GetType().Name;
            await _messageBroker.PublishEventAsync(@event, topicName, prefix);
        }
    }
}
