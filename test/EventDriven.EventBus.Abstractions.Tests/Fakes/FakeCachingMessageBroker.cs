using System.Threading.Tasks;

namespace EventDriven.EventBus.Abstractions.Tests.Fakes
{
    public class FakeCachingMessageBroker : FakeMessageBroker
    {
        private readonly EventBusOptions _options;

        public IEventBus EventBus { get; set; }

        public FakeCachingMessageBroker(
            EventBusOptions options)
        {
            _options = options;
        }
        
        public override Task PublishEventAsync<TIntegrationEvent>(
            TIntegrationEvent @event,
            string topic)
        {
            var handlers = Topics[topic];
            if (handlers == null) return Task.CompletedTask;
            foreach (var handler in handlers)
            {
                if (_options.EnableEventCache == false
                    || EventBus.EventCache.TryAdd(@event))
                    handler.HandleAsync(@event);
            }
            return Task.CompletedTask;
        }
    }
}
