using System;
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
            string? topic = null,
            string? prefix = null)
        {
            var topicName = GetTopicName(handler, topic, prefix);
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
            string? topic = null,
            string? prefix = null)
        {
            var topicName = GetTopicName(handler, topic, prefix);
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
            string? topic = null,
            string? prefix = null)
            where TIntegrationEvent : IntegrationEvent;

        /// <summary>
        /// Get topic name from event handler.
        /// </summary>
        /// <param name="handler">Subscription event handler.</param>
        /// <param name="topic">Subscription topic.</param>
        /// <param name="prefix">Dot delimited prefix, which can include version.</param>
        /// <returns>Fully qualified topic name.</returns>
        protected string GetTopicName(
            IIntegrationEventHandler handler,
            string? topic,
            string? prefix) => FormatTopicName(handler.Topic, topic, prefix);

        /// <summary>
        /// Get topic name from event handler.
        /// </summary>
        /// <param name="eventType">Integration event type.</param>
        /// <param name="topic">Subscription topic.</param>
        /// <param name="prefix">Dot delimited prefix, which can include version.</param>
        /// <returns>Fully qualified topic name.</returns>
        protected string GetTopicName(
            Type eventType,
            string? topic,
            string? prefix) => FormatTopicName(eventType.Name, topic, prefix);

        private string FormatTopicName(
            string implicitTopic,
            string? explicitTopic,
            string? prefix)
        {
            var topicName = string.IsNullOrWhiteSpace(explicitTopic) ? implicitTopic : explicitTopic;
            topicName = string.IsNullOrWhiteSpace(prefix) ? topicName : $"{prefix}.{topicName}";
            return topicName;
        }
    }
}
