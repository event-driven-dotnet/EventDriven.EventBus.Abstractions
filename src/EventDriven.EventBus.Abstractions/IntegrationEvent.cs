using System;

namespace EventDriven.EventBus.Abstractions
{
    /// <inheritdoc cref="IIntegrationEvent" />
    public abstract record IntegrationEvent : IIntegrationEvent
    {
        ///<inheritdoc/>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        ///<inheritdoc/>
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    }
}
