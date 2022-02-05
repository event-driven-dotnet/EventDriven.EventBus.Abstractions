using System.Threading.Tasks;

namespace EventDriven.EventBus.Abstractions.Tests.Fakes
{
    public class FakeCachingMessageBroker : FakeMessageBroker
    {
        public IEventCache EventCache { get; }

        public IEventBus? EventBus { get; set; }

        public FakeCachingMessageBroker(
            IEventCache eventCache)
        {
            EventCache = eventCache;
        }
        
        public override Task PublishEventAsync<TIntegrationEvent>(
            TIntegrationEvent @event,
            string topic)
        {
            var handlers = Topics[topic];
            foreach (var handler in handlers)
            {
                if (EventCache.TryAdd(@event))
                    handler.HandleAsync(@event);
            }
            return Task.CompletedTask;
        }
    }
}
