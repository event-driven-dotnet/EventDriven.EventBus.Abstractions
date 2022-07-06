using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventDriven.EventBus.Abstractions.Tests.Fakes
{
    public class FakeMessageBroker
    {
        public Dictionary<string, List<IIntegrationEventHandler>> Topics { get; } = new();

        public virtual void Subscribe(
            IIntegrationEventHandler handler,
            string? topic = null,
            string? prefix = null,
            string? suffix = null)
        {
            var topicName = string.IsNullOrWhiteSpace(topic) ? handler.Topic : topic;
            topicName = string.IsNullOrWhiteSpace(prefix) ? topicName : $"{prefix}.{topicName}";
            topicName = string.IsNullOrWhiteSpace(suffix) ? topicName : $"{topicName}.{suffix}";
            if (Topics.TryGetValue(topicName, out var handlers))
            {
                handlers.Add(handler);
            }
            else
            {
                Topics.Add(topicName, new List<IIntegrationEventHandler> { handler });
            }
        }

        public virtual Task PublishEventAsync<TIntegrationEvent>(
            TIntegrationEvent @event,
            string topic)
            where TIntegrationEvent : IntegrationEvent
        {
            var handlers = Topics[topic];
            foreach (var handler in handlers)
            {
                handler.HandleAsync(@event);
            }
            return Task.CompletedTask;
        }
    }
}
