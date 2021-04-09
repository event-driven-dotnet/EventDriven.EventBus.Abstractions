using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventDriven.EventBus.Abstractions.Tests.Fakes
{
    public class FakeMessageBroker
    {
        public Dictionary<string, List<IIntegrationEventHandler>> Topics { get; } = new();

        public void Subscribe(IIntegrationEventHandler handler, string topic = null, string prefix = null)
        {
            var topicName = topic ?? handler.Topic;
            if (!string.IsNullOrWhiteSpace(prefix)) topicName = $"{prefix}.{topicName}";
            if (Topics.TryGetValue(topicName, out var handlers))
            {
                handlers.Add(handler);
            }
            else
            {
                Topics.Add(topicName, new List<IIntegrationEventHandler> { handler });
            }
        }

        public Task PublishEventAsync<TIntegrationEvent>(TIntegrationEvent @event, string topic, string prefix)
            where TIntegrationEvent : IIntegrationEvent
        {
            var topicName = topic;
            if (!string.IsNullOrWhiteSpace(prefix)) topicName = $"{prefix}.{topicName}";
            var handlers = Topics[topicName];
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
