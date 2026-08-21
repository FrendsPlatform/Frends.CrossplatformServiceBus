using System.Collections.Generic;

namespace Frends.ServiceBus.Read.Definitions;

/// <summary>
/// Read result.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates whether the operation completed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// Error details. Null when Success is true.
    /// </summary>
    /// <example>null</example>
    public Error Error { get; set; }

    /// <summary>
    /// Read result.
    /// </summary>
    /// <example>[{ ReceivedMessage: true, Content: "Hello World", MessageId: "74ac7f11-1d38-4ff6-a28f-ae2333cc49cb", ContentType: "text/plain; charset=UTF-8", DeliveryCount: 1, SequenceNumber: 1 }]</example>
    public List<ReadResult> Results { get; set; }
}