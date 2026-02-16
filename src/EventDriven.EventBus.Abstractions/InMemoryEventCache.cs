using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AsyncKeyedLock;

namespace EventDriven.EventBus.Abstractions;

/// <summary>
/// Event cache to enable idempotency.
/// </summary>
public class InMemoryEventCache : IEventCache
{
    private readonly AsyncNonKeyedLocker _syncRoot = new();

    /// <summary>
    /// Event cache options.
    /// </summary>
    protected readonly EventCacheOptions EventCacheOptions;

    /// <summary>
    /// Thread-safe event cache.
    /// </summary>
    protected Dictionary<string, EventHandling> Cache { get; } = new();

    /// <summary>
    /// Cleanup timer.
    /// </summary>
    protected Timer? CleanupTimer { get; }

    /// <summary>
    /// Errors cleanup timer.
    /// </summary>
    protected Timer? ErrorsCleanupTimer { get; }

    /// <summary>
    /// Cancellation token.
    /// </summary>
    public CancellationToken CancellationToken { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="eventCacheOptions">Event cache options.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public InMemoryEventCache(
        EventCacheOptions eventCacheOptions,
        CancellationToken cancellationToken = default)
    {
        EventCacheOptions = eventCacheOptions;
        CancellationToken = cancellationToken;
        if (EventCacheOptions.EnableEventCacheCleanup)
        {
            CleanupTimer = new Timer(TimerCallback, null, TimeSpan.Zero, EventCacheOptions.EventCacheCleanupInterval);
            ErrorsCleanupTimer = new Timer(ErrorsTimerCallback, null, TimeSpan.Zero, EventCacheOptions.EventErrorsCacheCleanupInterval);
        }
        return;
        async void TimerCallback(object? state) => await CleanupEventCacheAsync();
        async void ErrorsTimerCallback(object? state) => await CleanupEventCacheErrorsAsync();
    }

    /// <summary>
    /// Clean up event cache.
    /// </summary>
    /// <returns>Task that will complete when the operation has completed.</returns>
    protected virtual async Task CleanupEventCacheAsync()
    {
        using var _ = await _syncRoot.LockAsync(CancellationToken);
        // End timer and exit if cache cleanup disabled or cancellation pending
        if (!EventCacheOptions.EnableEventCacheCleanup || CancellationToken.IsCancellationRequested)
        {
            if (CleanupTimer != null) await CleanupTimer.DisposeAsync();
            return;
        }

        // Remove expired events without errors
        var expired = Cache
            .Where(kvp =>
                DateTime.UtcNow > kvp.Value.EventHandledTime + kvp.Value.EventHandledTimeout
                && !kvp.Value.Handlers.Any(h => h.Value.HasError));
        foreach (var keyValuePair in expired)
            Cache.Remove(keyValuePair.Key);
    }

    /// <summary>
    /// Clean up event cache errors.
    /// </summary>
    /// <returns>Task that will complete when the operation has completed.</returns>
    protected virtual async Task CleanupEventCacheErrorsAsync()
    {
        using var _ = await _syncRoot.LockAsync(CancellationToken);
        // End timer and exit if cache cleanup disabled or cancellation pending
        if (!EventCacheOptions.EnableEventCacheCleanup || CancellationToken.IsCancellationRequested)
        {
            if (ErrorsCleanupTimer != null) await ErrorsCleanupTimer.DisposeAsync();
            return;
        }

        // Remove expired events with errors
        var expiredWithErrors = Cache
            .Where(kvp =>
                DateTime.UtcNow > kvp.Value.EventHandledTime + kvp.Value.EventHandledTimeout
                && kvp.Value.Handlers.Any(h => h.Value.HasError));
        foreach (var keyValuePair in expiredWithErrors)
            Cache.Remove(keyValuePair.Key);
    }

    /// <inheritdoc />
    public Task<bool> HasBeenHandledAsync(IntegrationEvent @event, string handlerTypeName)
    {
        // Return false if not enabled
        if (!EventCacheOptions.EnableEventCache) return Task.FromResult(false);

        // Return true if event exists, is not expired, and handler has no error
        var exists = Cache.TryGetValue(@event.Id, out var handling);
        var expired = handling != null &&
                      DateTime.UtcNow > handling.EventHandledTime + handling.EventHandledTimeout;
        var hasError = handling != null &&
                       handling.Handlers.ContainsKey(handlerTypeName) &&
                       handling.Handlers[handlerTypeName].HasError;
        var hasBeenHandled = exists && !(expired || hasError);
        return Task.FromResult(hasBeenHandled);
    }

    /// <inheritdoc />
    public Task AddEventAsync(IntegrationEvent @event,
        string? handlerTypeName = null, string? errorMessage = null)
    {
        // Return if cache not enabled
        if (!EventCacheOptions.EnableEventCache) return Task.CompletedTask;

        // Remove existing event
        Cache.Remove(@event.Id);

        // Add new event
        var handling = new EventHandling
        {
            EventId = @event.Id,
            IntegrationEvent = @event,
            EventHandledTime = DateTime.UtcNow,
            EventHandledTimeout = EventCacheOptions.EventCacheTimeout
        };
        if (!string.IsNullOrWhiteSpace(handlerTypeName))
        {
            handling.Handlers.Add(handlerTypeName, new HandlerInfo
            {
                HasError = !string.IsNullOrWhiteSpace(errorMessage),
                ErrorMessage = !string.IsNullOrWhiteSpace(errorMessage)
                    ? errorMessage : null
            });
        }
        Cache.Add(@event.Id, handling);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task UpdateEventAsync(IntegrationEvent @event, string? handlerTypeName = null, string? errorMessage = null)
    {
        // Return if cache not enabled
        if (!EventCacheOptions.EnableEventCache) return Task.CompletedTask;

        // Remove existing event
        Cache.Remove(@event.Id);

        // Add new event
        var handling = new EventHandling
        {
            EventId = @event.Id,
            IntegrationEvent = @event,
            EventHandledTime = DateTime.UtcNow,
            EventHandledTimeout = EventCacheOptions.EventCacheTimeout
        };
        if (!string.IsNullOrWhiteSpace(handlerTypeName))
        {
            handling.Handlers.Add(handlerTypeName, new HandlerInfo
            {
                HasError = !string.IsNullOrWhiteSpace(errorMessage),
                ErrorMessage = !string.IsNullOrWhiteSpace(errorMessage)
                    ? errorMessage : null
            });
        }
        Cache[@event.Id] = handling;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<bool> HasBeenHandledPersistEventAsync(IntegrationEvent @event, string? handlerTypeName = null)
    {
        var hasBeenHandled = await HasBeenHandledAsync(@event, handlerTypeName!);
        if (!hasBeenHandled)
        {
            if (Cache.ContainsKey(@event.Id))
                await UpdateEventAsync(@event, handlerTypeName);
            else
                await AddEventAsync(@event, handlerTypeName);
        }

        return hasBeenHandled;
    }
}