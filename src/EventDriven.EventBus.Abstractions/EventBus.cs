using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EventDriven.EventBus.Abstractions;

///<inheritdoc/>
public abstract class EventBus : IEventBus
{
    ///<inheritdoc/>
    public Dictionary<string, List<IIntegrationEventHandler>> Topics { get; } = new();

    ///<inheritdoc/>
    public virtual void Subscribe(
        IIntegrationEventHandler handler,
        string? topic = null,
        string? prefix = null,
        string? suffix = null)
    {
        var topicName = GetTopicName(handler, topic, prefix, suffix);
        if (Topics.TryGetValue(topicName, out var handlers))
            handlers.Add(handler);
        else
            Topics.Add(topicName, new List<IIntegrationEventHandler> { handler });
    }

    ///<inheritdoc/>
    public virtual void UnSubscribe(
        IIntegrationEventHandler handler,
        string? topic = null,
        string? prefix = null,
        string? suffix = null)
    {
        var topicName = GetTopicName(handler, topic, prefix, suffix);
        if (!Topics.TryGetValue(topicName, out var handlers)) return;
        handlers.Remove(handler);
        if (handlers.Count == 0)
        {
            Topics.Remove(topicName);
        }
    }

    ///<inheritdoc/>
    public abstract Task PublishAsync<TIntegrationEvent>(
        TIntegrationEvent @event,
        string? topic = null,
        string? prefix = null,
        string? suffix = null)
        where TIntegrationEvent : IntegrationEvent;

    /// <summary>
    /// Get topic name from event handler.
    /// </summary>
    /// <param name="handler">Subscription event handler.</param>
    /// <param name="topic">Subscription topic.</param>
    /// <param name="prefix">Dot delimited prefix, which can include version.</param>
    /// <param name="suffix">Dot delimited suffix, which can include version.</param>
    /// <returns>Fully qualified topic name.</returns>
    protected string GetTopicName(
        IIntegrationEventHandler handler,
        string? topic,
        string? prefix,
        string? suffix)
    {
        var eventType = handler.GetType().BaseType?.GetGenericArguments().FirstOrDefault();
        SetTopicFromAttribute(ref topic, ref prefix, ref suffix, eventType);
        return FormatTopicName(handler.Topic, topic, prefix, suffix);
    }

    /// <summary>
    /// Get topic name from event handler.
    /// </summary>
    /// <param name="eventType">Integration event type.</param>
    /// <param name="topic">Subscription topic.</param>
    /// <param name="prefix">Dot delimited prefix, which can include version.</param>
    /// <param name="suffix">Dot delimited suffix, which can include version.</param>
    /// <returns>Fully qualified topic name.</returns>
    protected string GetTopicName(
        Type eventType,
        string? topic,
        string? prefix,
        string? suffix)
    {
        SetTopicFromAttribute(ref topic, ref prefix, ref suffix, eventType);
        return FormatTopicName(eventType.Name, topic, prefix, suffix);
    }

    private void SetTopicFromAttribute(ref string? topic, ref string? prefix, ref string? suffix, Type? eventType)
    {
        var attribute = eventType?.GetCustomAttribute<TopicInfoAttribute>();
        if (attribute is null ||
            topic is not null ||
            prefix is not null ||
            suffix is not null) return;
        topic = attribute.Topic;
        prefix = attribute.Prefix;
        suffix = attribute.Suffix;
    }

    private string FormatTopicName(
        string implicitTopic,
        string? explicitTopic,
        string? prefix,
        string? suffix)
    {
        var topicName = string.IsNullOrWhiteSpace(explicitTopic) ? implicitTopic : explicitTopic;
        topicName = string.IsNullOrWhiteSpace(prefix) ? topicName : $"{prefix}.{topicName}";
        topicName = string.IsNullOrWhiteSpace(suffix) ? topicName : $"{topicName}.{suffix}";
        return topicName;
    }
}
