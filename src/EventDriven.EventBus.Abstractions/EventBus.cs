using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventDriven.EventBus.Abstractions
{
    ///<inheritdoc/>
    public abstract class EventBus : IEventBus
    {
        ///<inheritdoc/>
        public Dictionary<string, List<IIntegrationEventHandler>> Topics { get; } = new();

        ///<inheritdoc/>
        public virtual void Subscribe(
            IIntegrationEventHandler handler,
            string topic = null,
            string prefix = null)
        {
            var topicName = topic ?? handler.Topic;
            if (Topics.TryGetValue(topicName, out var handlers))
            {
                handlers.Add(handler);
            }
            else
            {
                Topics.Add(topicName, new List<IIntegrationEventHandler> { handler });
            }
        }

        ///<inheritdoc/>
        public virtual void UnSubscribe(
            IIntegrationEventHandler handler,
            string topic = null,
            string prefix = null)
        {
            var topicName = topic ?? handler.Topic;
            if (Topics.TryGetValue(topicName, out var handlers))
            {
                handlers.Remove(handler);
                if (handlers.Count == 0)
                {
                    Topics.Remove(topicName);
                }
            }
        }

        ///<inheritdoc/>
        public abstract Task PublishAsync<TIntegrationEvent>(
            TIntegrationEvent @event,
            string topic = null,
            string prefix = null)
            where TIntegrationEvent : IIntegrationEvent;
    }
}
