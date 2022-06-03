using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.CrossplatformServiceBus.Read.Definitions;

/// <summary>
/// Option parameters 
/// </summary>
public class Options
    {
    /// <summary>
    /// Timeout in seconds.
    /// </summary>
    /// <example>60</example>
    [DefaultValue("60")]
    public int TimeoutSeconds { get; set; }

    /// <summary>
    /// Should the service bus connection (MessagingFactory) be cached. This speeds up the execution as creating a new connection is slow by keeping the connection open in the background, a single namespace is limited to 1000 concurrent connections.
    /// </summary>
    /// <example>true</example>
    [DefaultValue(true)]
    public bool UseCachedConnection { get; set; }

    /// <summary>
    /// How the body is expected to be serialized.
    /// </summary>
    /// <example>Stream</example>
    [DefaultValue(BodySerializationType.Stream)]
    public BodySerializationType BodySerializationType { get; set; }

    /// <summary>
    /// What encoding the message contents is expected to be in.
    /// </summary>
    /// <example>UTF8, UTF32, ASCII, Unicode, Latin1, BigEndianUnicode</example>
    [DefaultValue(MessageEncoding.UTF8)]
    public MessageEncoding DefaultEncoding { get; set; }

    /// <summary>
    /// Should the existence of the message source be checked and created if it does not exist.
    /// </summary>
    /// <example>False</example>
    [DefaultValue(false)]
    public bool CreateQueueOrTopicIfItDoesNotExist { get; set; }

    /// <summary>
    /// Idle interval after which the queue or topic+subscription is automatically deleted. See TimeFormat to select Minutes/Hours/Days/Never. The minimum duration is 5 minutes and contains converter e.g. 61 minutes = 1 hour 1 minute.
    /// </summary>
    /// <example>5</example>
    [UIHint(nameof(CreateQueueOrTopicIfItDoesNotExist), "", true)]
    [DefaultValue("5")]
    public int AutoDeleteOnIdle { get; set; }

    /// <summary>
    /// Timeformat for AutoDeleteOnIdle.
    /// </summary>
    /// <example>Minutes/Hours/Days/Never</example>
    [UIHint(nameof(CreateQueueOrTopicIfItDoesNotExist), "", true)]
    [DefaultValue(TimeFormat.Minutes)]
    public TimeFormat TimeFormat { get; set; }

    /// <summary>
    /// The maximum size of the queue/topic in megabytes. Default value is 1024.
    /// </summary>
    /// <example>1024</example>
    [UIHint(nameof(CreateQueueOrTopicIfItDoesNotExist), "", true)]
    [DefaultValue(1024)]
    public int MaxSize { get; set; }
}
