namespace Frends.ServiceBus.Read.Definitions;

/// <summary>
/// The encoding to use with the message contents
/// </summary>
public enum MessageEncoding
{
    /// <summary>
    /// UTF8
    /// </summary>
    UTF8,

    /// <summary>
    /// UTF32
    /// </summary>
    UTF32,

    /// <summary>
    /// ASCII
    /// </summary>
    ASCII,

    /// <summary>
    /// Unicode
    /// </summary>
    Unicode,

    /// <summary>
    /// Latin1
    /// </summary>
    Latin1,

    /// <summary>
    /// BigEndianUnicode
    /// </summary>
    BigEndianUnicode, 
}

/// <summary>
/// How the body of the message should be serialized
/// </summary>
public enum BodySerializationType
{
    /// <summary>
    /// As a stream
    /// </summary>
    Stream,
    /// <summary>
    /// As a byte array
    /// </summary>
    ByteArray,
    /// <summary>
    /// As a string
    /// </summary>
    String
}

/// <summary>
/// Is the message source a queue or a subscription
/// </summary>
public enum QueueOrSubscription
{
    /// <summary>
    /// A queue
    /// </summary>
    Queue,
    /// <summary>
    /// A subscription
    /// </summary>
    Subscription
}

/// <summary>
/// Time format for AutoDeleteOnIdle.
/// </summary>
public enum TimeFormat
{
    /// <summary>
    /// Minutes
    /// </summary>
    Minutes,

    /// <summary>
    /// Hours
    /// </summary>
    Hours,

    /// <summary>
    /// Days
    /// </summary>
    Days,
}
