using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.CrossplatformServiceBus.Read.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Source type.
    /// </summary>
    /// <example>Queue</example>
    [DefaultValue(QueueOrTopic.Queue)]
    public QueueOrTopic SourceType { get; set; }

    /// <summary>
    /// The name of the queue or topic.
    /// </summary>
    /// <example>QueueOrTopicName</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string QueueOrTopicName { get; set; }

    /// <summary>
    /// The name of the subscription
    /// </summary>
    /// <example>SubscriptionName</example>
    [UIHint(nameof(SourceType), "", QueueOrTopic.Topic)]
    [DisplayFormat(DataFormatString = "Text")]
    public string SubscriptionName { get; set; }

    /// <summary>
    /// Service Bus connection string
    /// </summary>
    /// <example>"Endpoint=sb://[namespace].servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=[secret]</example>
    [PasswordPropertyText]
    [DefaultValue("\"Endpoint=sb://[namespace].servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=[secret]\"")]
    [DisplayFormat(DataFormatString = "Text")]
    public string ConnectionString { get; set; }
}