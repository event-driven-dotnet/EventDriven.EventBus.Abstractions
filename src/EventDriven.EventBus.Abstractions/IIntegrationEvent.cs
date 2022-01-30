using System;

namespace EventDriven.EventBus.Abstractions
{
    /// <summary>
    /// Event for communicating information between systems.
    /// </summary>
    public interface IIntegrationEvent
    {
        /// <summary>
        /// Unique event identifier.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Event creation date.
        /// </summary>
        DateTime CreationDate { get; set; }
    }
}
