using System;

namespace EventDriven.EventBus.Abstractions;

/// <summary>
/// Event cache options.
/// </summary>
public class EventCacheOptions
{
    /// <summary>
    /// Application name.
    /// </summary>
    public string AppName { get; set; } = null!;

    /// <summary>
    /// True to enable event cache for idempotency. Defaults to true.
    /// </summary>
    public bool EnableEventCache { get; set; } = true;

    /// <summary>
    /// Event cache timeout. Defaults to 60 seconds.
    /// </summary>
    public TimeSpan EventCacheTimeout { get; set; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// True to enable event cache clean up. Defaults to false.
    /// </summary>
    public bool EnableEventCacheCleanup { get; set; }

    /// <summary>
    /// Event cache cleanup interval. Defaults to 5 minutes.
    /// </summary>
    public TimeSpan EventCacheCleanupInterval { get; set; } = TimeSpan.FromMinutes(5);
    
    /// <summary>
    /// Event errors cache cleanup interval. Defaults to 15 minutes.
    /// </summary>
    public TimeSpan EventErrorsCacheCleanupInterval { get; set; } = TimeSpan.FromMinutes(15);
}