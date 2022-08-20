namespace Frends.ServiceBus.Read.Definitions;

/// <summary>
/// The encoding to use with the message contents
/// </summary>
public enum MessageEncoding
{
#pragma warning disable CS1591 // self explanatory
    UTF8,
    UTF32,
    ASCII,
    Unicode,
    Latin1,
    BigEndianUnicode,
#pragma warning restore CS1591 // self explanatory
}

/// <summary>
/// How the body of the message should be serialized
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
/// Is the message source a queue or a topic
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