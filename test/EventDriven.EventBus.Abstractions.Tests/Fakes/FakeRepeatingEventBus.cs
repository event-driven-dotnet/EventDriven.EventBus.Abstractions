using System.Threading.Tasks;

namespace EventDriven.EventBus.Abstractions.Tests.Fakes
{
    public class FakeRepeatingEventBus : FakeEventBus
    {
        private readonly EventCacheOptions _eventBusOptions;
        private readonly int _iterations;
        private readonly bool _expire;

        public FakeRepeatingEventBus(FakeMessageBroker messageBroker,
            EventCacheOptions eventBusOptions, int iterations, bool expire, bool hasError) :
            base(messageBroker, hasError)
        {
            _eventBusOptions = eventBusOptions;
            _iterations = iterations;
            _expire = expire;
        }

        public override async Task PublishAsync<TIntegrationEvent>(
            TIntegrationEvent @event,
            string? topic = null,
            string? prefix = null,
            string? suffix = null)
        {
            var topicName = GetTopicName(@event.GetType(), topic, prefix, suffix);
            for (int i = 0; i < _iterations; i++)
            {
                await MessageBroker.PublishEventAsync(@event, topicName, HasError);
                if (_expire) await Task.Delay(_eventBusOptions.EventCacheTimeout * 1.5);
            }
        }
    }
}
