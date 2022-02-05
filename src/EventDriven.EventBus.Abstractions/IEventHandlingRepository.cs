using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventDriven.EventBus.Abstractions;

/// <summary>
/// Event handing repository interface.
/// </summary>
public interface IEventHandlingRepository
{
    /// <summary>
    /// Remove expired events.
    /// </summary>
    /// <returns>
    /// Task that will complete when the operation has completed.
    /// Task contains number of items deleted.
    /// </returns>
    Task<int> RemoveExpiredEventsAsync();
}

/// <summary>
/// Event handing repository interface.
/// </summary>
/// <typeparam name="TIntegrationEvent">Integration event type.</typeparam>
public interface IEventHandlingRepository<TIntegrationEvent> : IEventHandlingRepository
    where TIntegrationEvent : IntegrationEvent
{
    /// <summary>
    /// Get expired integration events.
    /// </summary>
    /// <returns>
    /// Task that will complete when the operation has completed.
    /// Task contains an IEnumerable of EventWrapper of TIntegrationEvent.
    /// </returns>
    Task<IEnumerable<EventWrapper<TIntegrationEvent>>> GetExpiredEventsAsync();
}