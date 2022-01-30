using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventDriven.EventBus.Abstractions.Tests.Fakes
{
    public class FakeMessageBroker
    {
        public Dictionary<string, List<IIntegrationEventHandler>> Topics { get; } = new();

        public virtual void Subscribe(
            IIntegrationEventHandler handler,
            string topic = null,
            string prefix = null)
        {
            var topicName = string.IsNullOrWhiteSpace(topic) ? handler.Topic : topic;
            topicName = string.IsNullOrWhiteSpace(prefix) ? topicName : $"{prefix}.{topicName}";
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
            where TIntegrationEvent : IIntegrationEvent
        {
            var handlers = Topics[topic];
            if (handlers != null)
            {
                foreach (var handler in handlers)
                {
                    handler.HandleAsync(@event);
                }
            }
            return Task.CompletedTask;
        }
    }
}
