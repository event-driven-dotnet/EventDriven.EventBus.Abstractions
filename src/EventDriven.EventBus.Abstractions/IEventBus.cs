using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventDriven.EventBus.Abstractions
{
    /// <summary>
    /// Provides a way for systems to communicate without knowing about each other.
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// List of topics with associated event handlers.
        /// </summary>
        Dictionary<string, List<IIntegrationEventHandler>> Topics { get; }

        /// <summary>
        /// Register a subscription with an event handler.
        /// </summary>
        /// <param name="handler">Subscription event handler.</param>
        /// <param name="topic">Subscription topic.</param>
        /// <param name="prefix">Dot delimited prefix, which can include version.</param>
        /// <param name="suffix">Dot delimited suffix, which can include version.</param>
        void Subscribe(
            IIntegrationEventHandler handler,
            string? topic = null,
            string? prefix = null,
            string? suffix = null);

        /// <summary>
        /// Unregister a subscription with an event handler.
        /// </summary>
        /// <param name="handler">Subscription event handler.</param>
        /// <param name="topic">Subscription topic.</param>
        /// <param name="prefix">Dot delimited prefix, which can include version.</param>
        /// <param name="suffix">Dot delimited suffix, which can include version.</param>
        void UnSubscribe(
            IIntegrationEventHandler handler,
            string? topic = null,
            string? prefix = null,
            string? suffix = null);

        /// <summary>
        /// Publish an event asynchronously.
        /// </summary>
        /// <typeparam name="TIntegrationEvent">Integration event type.</typeparam>
        /// <param name="event">Integration event.</param>
        /// <param name="topic">Publication topic.</param>
        /// <param name="prefix">Dot delimited prefix, which can include version.</param>
        /// <param name="suffix">Dot delimited suffix, which can include version.</param>
        /// <returns>Task that will complete when the operation has completed.</returns>
        Task PublishAsync<TIntegrationEvent>(
            TIntegrationEvent @event,
            string? topic = null,
            string? prefix = null,
            string? suffix = null)
            where TIntegrationEvent : IntegrationEvent;
    }
}
