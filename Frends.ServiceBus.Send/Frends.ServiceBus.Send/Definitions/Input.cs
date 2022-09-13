using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.ServiceBus.Send.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Destination type.
    /// </summary>
    /// <example>Queue</example>
    [DefaultValue(QueueOrTopic.Queue)]
    public QueueOrTopic DestinationType { get; set; }

    /// <summary>
    /// Name of the queue or topic.
    /// </summary>
    /// <example>ExampleQueue</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string QueueOrTopicName { get; set; }

    /// <summary>
    /// Name of the subscription.
    /// </summary>
    /// <example>SubscriptionName</example>
    [UIHint(nameof(DestinationType), "", QueueOrTopic.Topic)]
    [DisplayFormat(DataFormatString = "Text")]
    public string SubscriptionName { get; set; }

    /// <summary>
    /// Service Bus connection string.
    /// </summary>
    /// <example>Endpoint=sb://[namespace].servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=[secret]</example>
    [PasswordPropertyText]
    [DefaultValue("\"Endpoint=sb://[namespace].servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=[secret]\"")]
    public string ConnectionString { get; set; }

    /// <summary>
    /// Custom properties for the message.
    /// </summary>
    /// <example>Name = ExampleName, Value = ExampleValue</example>
    public MessageProperty[] Properties { get; set; }

    /// <summary>
    /// Data to send.
    /// </summary>
    /// <example>Example text.</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string Data { get; set; }
}

/// <summary>
/// A single custom property for a Service Bus message.
/// </summary>
public class MessageProperty
{
    /// <summary>
    /// Name of the Service Bus message's custom property.
    /// </summary>
    /// <example>ExampleName</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string Name { get; set; }

    /// <summary>
    /// Value of the Service Bus message's custom property.
    /// </summary>
    /// <example>ExampleValue</example>
    [DisplayFormat(DataFormatString = "Text")]
    public object Value { get; set; }
}