namespace Frends.CrossplatformServiceBus.Definitions.Send;

/// <summary>
/// How the body of the message should be serialized.
/// </summary>
public enum BodySerializationType
{
#pragma warning disable CS1591 // self explanatory
    Stream,
    ByteArray,
    String
#pragma warning restore CS1591 // self explanatory
}

/// <summary>
/// Is the message source a queue or a topic.
/// </summary>
public enum QueueOrTopic
{
#pragma warning disable CS1591 // self explanatory
    Queue,
    Topic
#pragma warning restore CS1591 // self explanatory
}

/// <summary>
/// Time format for AutoDeleteOnIdle.
/// </summary>
public enum TimeFormat
{
#pragma warning disable CS1591 // self explanatory
    Minutes,
    Hours,
    Days,
#pragma warning restore CS1591 // self explanatory
}
