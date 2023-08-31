using System;

namespace EventDriven.EventBus.Abstractions;

/// <summary>
/// Attribute for storing topic info for events.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class TopicInfoAttribute : Attribute
{
    /// <summary>
    /// Ctor for TopicInfoAttribute.
    /// </summary>
    /// <param name="topic">Event topic.</param>
    /// <param name="prefix">Dot delimited prefix, which can include version.</param>
    /// <param name="suffix">Dot delimited suffix, which can include version.</param>
    public TopicInfoAttribute(string? topic = null, string? prefix = null, string? suffix = null)
    {
        Topic = topic;
        Prefix = prefix;
        Suffix = suffix;
    }

    /// <summary>
    /// Topic name.
    /// </summary>
    public string? Topic { get; }

    /// <summary>
    /// Dot delimited prefix, which can include version.
    /// </summary>
    public string? Prefix { get; }

    /// <summary>
    /// Dot delimited suffix, which can include version.
    /// </summary>
    public string? Suffix { get; }
}