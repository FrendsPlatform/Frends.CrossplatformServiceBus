namespace Frends.ServiceBus.Send.Definitions;

/// <summary>
/// How the body of the message should be serialized.
/// </summary>
public enum BodySerializationType
{
    /// <summary>
    /// Serialize as Stream.
    /// </summary>
    Stream,

    /// <summary>
    /// Serialize as ByteArray.
    /// </summary>
    ByteArray,

    /// <summary>
    /// Serialize as String.
    /// </summary>
    String,
}

/// <summary>
/// Is the message source a queue or a topic.
/// </summary>
public enum QueueOrTopic
{
    /// <summary>
    /// Queue.
    /// </summary>
    Queue,

    /// <summary>
    /// Topic.
    /// </summary>
    Topic,
}

/// <summary>
/// Time format for AutoDeleteOnIdle.
/// </summary>
public enum TimeFormat
{
    /// <summary>
    /// Minutes.
    /// </summary>
    Minutes,

    /// <summary>
    /// Hours.
    /// </summary>
    Hours,

    /// <summary>
    /// Days.
    /// </summary>
    Days,
}
