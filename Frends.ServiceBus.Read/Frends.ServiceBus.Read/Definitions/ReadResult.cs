using System;
using System.Collections.Generic;

namespace Frends.ServiceBus.Read.Definitions;
/// <summary>
/// Return object
/// </summary>
public class ReadResult
{
    /// <summary>
    /// Did the service bus provide a message
    /// </summary>
    public bool ReceivedMessage { get; set; }

    /// <summary>
    /// The body contents of the message
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// The content type header of the received message
    /// </summary>
    public string ContentType { get; set; }

    /// <summary>
    /// The session id of the received message
    /// </summary>
    public string SessionId { get; set; }

    /// <summary>
    /// The message id of the received message
    /// </summary>
    public string MessageId { get; set; }

    /// <summary>
    /// The correlation id of the received message
    /// </summary>
    public string CorrelationId { get; set; }

    /// <summary>
    /// The delivery count of the received message
    /// </summary>
    public int DeliveryCount { get; set; }

    /// <summary>
    /// The enqueued sequence number of the received message
    /// </summary>
    public long EnqueuedSequenceNumber { get; set; }

    /// <summary>
    /// The sequence number of the received message
    /// </summary>
    public long SequenceNumber { get; set; }


    /// <summary>
    /// The label of the received message
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// The custom properties of the received message
    /// </summary>
    public Dictionary<string, object> Properties { get; set; }

    /// <summary>
    /// The reply to address of the received message
    /// </summary>
    public string ReplyTo { get; set; }

    /// <summary>
    /// The reply to session id of the received message
    /// </summary>
    public string ReplyToSessionId { get; set; }

    /// <summary>
    /// The size of the received message
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// The to field of the received message
    /// </summary>
    public string To { get; set; }

    /// <summary>
    /// The time when the message was scheduled to be queued
    /// </summary>
    public DateTime ScheduledEnqueueTimeUtc { get; set; }
}
