using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.ServiceBus.Send.Definitions;

/// <summary>
/// Option parameters 
/// </summary>
public class Options
{
    /// <summary>
    /// How should the body of the message be serialized.
    /// </summary>
    /// <example>Stream</example>
    [DefaultValue(BodySerializationType.Stream)]
    public BodySerializationType BodySerializationType { get; set; }

    /// <summary>
    /// Content-Type descriptor.
    /// </summary>
    /// <example>text/plain; charset=UTF-8</example>
    [DefaultValue("\"text/plain; charset=UTF-8\"")]
    [DisplayFormat(DataFormatString = "Text")]
    public string ContentType { get; set; }

    /// <summary>
    /// The message identifier can be used to detect duplicate messages. A new guid is generated as the value if left empty or null.
    /// </summary>
    /// <example>254e5c7a-03eb-4637-935a-e1e61345c11d</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string MessageId { get; set; }

    /// <summary>
    /// Message's session identifier. Messages can be filtered according to the session identifier and handled on the same Service Bus broker ensuring delivery order.
    /// </summary>
    /// <example>f580ef61-bd66-4676-b98f-b9804961b339</example>
    
    [DisplayFormat(DataFormatString = "Text")]
    public string SessionId { get; set; }

    /// <summary>
    /// The correlation identifier for the message can be used when filtering messages for subscriptions.
    /// </summary>
    /// <example>a1b58684-f04f-432a-bc73-1e72129ff6cc</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string CorrelationId { get; set; }

    /// <summary>
    /// The reply to field for the message can be used to specify the name of the queue where the receiver of the message should reply to.
    /// </summary>
    /// <example>ReplyToQueue</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string ReplyTo { get; set; }

    /// <summary>
    /// The reply to session identifier can be used to specify the session identifier for a reply message.
    /// </summary>
    /// <example>fff56b95-af19-4e1b-87f4-3464543dab02</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string ReplyToSessionId { get; set; }

    /// <summary>
    /// The intended recipient of the message.
    /// </summary>
    /// <example>Recipient</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("")]
    public string To { get; set; }

    /// <summary>
    /// The time in seconds the message is kept in the queue before being discarded or deadlettered.
    /// </summary>
    /// <example>60</example>
    public long? TimeToLiveSeconds { get; set; }

    /// <summary>
    /// UTC time when the message should be added to the queue. Can be used to delay the delivery of a message. Using DateTime.Now if left empty.
    /// </summary>
    /// <example>DateTime.Now.AddMinutes(5)</example>
    public DateTime? ScheduledEnqueueTimeUtc { get; set; }

    /// <summary>
    /// Should the queue or topic be created if it does not already exist.
    /// </summary>
    /// <example>True</example>
    [DefaultValue(false)]
    public bool CreateQueueOrTopicIfItDoesNotExist { get; set; }

    /// <summary>
    /// Idle interval after which the queue or topic+subscription is automatically deleted. See TimeFormat to select Minutes/Hours/Days/Never. The minimum duration is 5 minutes. Contains converter e.g. 61 minutes = 1 hour 1 minute or 4 minutes = 5 minutes.
    /// </summary>
    /// <example>5, 61</example>
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

    /// <summary>
    /// Should the Service Bus connection (MessagingFactory) be cached. This speeds up the execution as creating a new connection is slow by keeping the connection open in the background. A single namespace is limited to 1000 concurrent connections.
    /// </summary>
    /// <example>True</example>
    [DefaultValue(true)]
    public bool UseCachedConnection { get; set; }

    /// <summary>
    /// Timeout in seconds.
    /// </summary>
    /// <example>60</example>
    [DefaultValue(60)]
    public long TimeoutSeconds { get; set; }
}
