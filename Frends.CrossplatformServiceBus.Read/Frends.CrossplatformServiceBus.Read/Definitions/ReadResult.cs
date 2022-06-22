using System;
using System.Collections.Generic;

namespace Frends.CrossplatformServiceBus.Read.Definitions;
/// <summary>
/// Return object
/// </summary>
public class ReadResult
{
    /// <summary>
    /// Did the service bus provide a message
    /// </summary>
    /// <example>true</example>
    public bool ReceivedMessage { get; private set; }

    /// <summary>
    /// The body contents of the message
    /// </summary>
    /// <example>Message Content</example>
    public string Content { get; private set; }

    /// <summary>
    /// The content type header of the received message
    /// </summary>
    /// <example>text/plain; charset=UTF-8</example>
    public string ContentType { get; private set; }

    /// <summary>
    /// The session id of the received message
    /// </summary>
    /// <example>NULL</example>
    public string SessionId { get; private set; }

    /// <summary>
    /// The message id of the received message
    /// </summary>
    /// <example>74ac7f11-1d38-4ff6-a28f-ae2333cc49cb</example>
    public string MessageId { get; private set; }

    /// <summary>
    /// The correlation id of the received message
    /// </summary>
    /// <example>NULL</example>
    public string CorrelationId { get; private set; }

    /// <summary>
    /// The delivery count of the received message
    /// </summary>
    /// <example>1</example>
    public int DeliveryCount { get; private set; }

    /// <summary>
    /// The enqueued sequence number of the received message
    /// </summary>
    /// <example>0</example>
    public long EnqueuedSequenceNumber { get; private set; }

    /// <summary>
    /// The sequence number of the received message
    /// </summary>
    /// <example>1</example>
    public long SequenceNumber { get; private set; }


    /// <summary>
    /// The label of the received message
    /// </summary>
    /// <example>NULL</example>
    public string Label { get; private set; }

    /// <summary>
    /// The custom properties of the received message
    /// </summary>
    /// <example>{[PropName, PropValue]}</example>
    public Dictionary<string, object> Properties { get; private set; }

    /// <summary>
    /// The reply to address of the received message
    /// </summary>
    /// <example>NULL</example>
    public string ReplyTo { get; private set; }

    /// <summary>
    /// The reply to session id of the received message
    /// </summary>
    /// <example>NULL</example>
    public string ReplyToSessionId { get; private set; }

    /// <summary>
    /// The size of the received message
    /// </summary>
    /// <example>80</example>
    public long Size { get; private set; }

    /// <summary>
    /// The to field of the received message
    /// </summary>
    /// <example>NULL</example>
    public string To { get; private set; }

    /// <summary>
    /// The time when the message was scheduled to be queued
    /// </summary>
    /// <example>{1.1.0001 0.00.00}</example>
    public DateTime ScheduledEnqueueTimeUtc { get; private set; }

    internal ReadResult(bool receivedMessage)
    {
        ReceivedMessage = receivedMessage;
    }

    internal ReadResult(bool receivedMessage, string contentType, Dictionary<string, object> properties, string sessionId, string messageId, string correlationId, string label, int deliveryCount, long enqueuedSequenceNumber, long sequenceNumber, string replyTo, string replyToSessionId, long size, string to, DateTime scheduledEnqueueTimeUtc, string content)
    {
        ReceivedMessage = receivedMessage;
        ContentType = contentType;
        Properties = properties;
        SessionId = sessionId;
        MessageId = messageId;
        CorrelationId = correlationId;
        Label = label;
        DeliveryCount = deliveryCount;
        EnqueuedSequenceNumber = enqueuedSequenceNumber;
        SequenceNumber = sequenceNumber;
        ReplyTo = replyTo;
        ReplyToSessionId = replyToSessionId;
        Size = size;
        To = to;
        ScheduledEnqueueTimeUtc = scheduledEnqueueTimeUtc;
        Content = content;
    }
}
