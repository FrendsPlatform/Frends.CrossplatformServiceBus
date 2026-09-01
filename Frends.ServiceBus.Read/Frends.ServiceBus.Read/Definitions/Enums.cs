namespace Frends.ServiceBus.Read.Definitions;

/// <summary>
/// The encoding to use with the message contents
/// </summary>
public enum MessageEncoding
{
    /// <summary>UTF-8 encoding.</summary>
    UTF8,

    /// <summary>UTF-32 encoding.</summary>
    UTF32,

    /// <summary>ASCII encoding.</summary>
    ASCII,

    /// <summary>Unicode (UTF-16) encoding.</summary>
    Unicode,

    /// <summary>Latin-1 (ISO-8859-1) encoding.</summary>
    Latin1,

    /// <summary>Big-endian Unicode encoding.</summary>
    BigEndianUnicode,
}

/// <summary>
/// How the body of the message should be serialized
/// </summary>
public enum BodySerializationType
{
    /// <summary>Message body is a raw byte stream.</summary>
    Stream,

    /// <summary>Message body is a byte array.</summary>
    ByteArray,

    /// <summary>Message body is a string.</summary>
    String,
}

/// <summary>
/// Is the message source a queue or a topic
/// </summary>
public enum QueueOrTopic
{
    /// <summary>Source is a queue.</summary>
    Queue,

    /// <summary>Source is a topic subscription.</summary>
    Topic,
}

/// <summary>
/// Time format for AutoDeleteOnIdle.
/// </summary>
public enum TimeFormat
{
    /// <summary>Time unit is minutes.</summary>
    Minutes,

    /// <summary>Time unit is hours.</summary>
    Hours,

    /// <summary>Time unit is days.</summary>
    Days,
}