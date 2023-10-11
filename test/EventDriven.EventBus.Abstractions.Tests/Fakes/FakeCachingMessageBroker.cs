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
        
        public override async Task PublishEventAsync<TIntegrationEvent>(
            TIntegrationEvent @event,
            string topic, bool hasError = false)
        {
            var handlers = Topics[topic];
            foreach (var handler in handlers)
            {
                var handlerTypeName = handler.GetType().Name;
                var errorMessage = hasError ? "Fake Error" : null;
                if (await EventCache.HasBeenHandledAsync(@event, handlerTypeName)) continue;
                if (!hasError) await handler.HandleAsync(@event);
                await EventCache.AddEventAsync(@event, handlerTypeName, errorMessage);
            }
        }
    }
}
