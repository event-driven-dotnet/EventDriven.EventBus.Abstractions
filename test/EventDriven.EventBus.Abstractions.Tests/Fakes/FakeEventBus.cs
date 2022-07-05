using System.Threading.Tasks;

namespace EventDriven.EventBus.Abstractions.Tests.Fakes
{
    public class FakeEventBus : EventBus
    {
        protected FakeMessageBroker MessageBroker { get; }

        public FakeEventBus(FakeMessageBroker messageBroker)
        {
            MessageBroker = messageBroker;
        }

        public override async Task PublishAsync<TIntegrationEvent>(
            TIntegrationEvent @event,
            string? topic = null,
            string? prefix = null,
            string? suffix = null)
        {
            var topicName = GetTopicName(@event.GetType(), topic, prefix, suffix);
            await MessageBroker.PublishEventAsync(@event, topicName);
        }
    }
}
