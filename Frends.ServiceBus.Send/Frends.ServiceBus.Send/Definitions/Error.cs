using System;

namespace Frends.ServiceBus.Send.Definitions;

/// <summary>
/// Error details for a failed operation.
/// </summary>
public class Error
{
    /// <summary>
    /// Error message.
    /// </summary>
    /// <example>The messaging entity could not be found.</example>
    public string Message { get; internal set; }

    /// <summary>
    /// Additional information about the error, such as the original exception.
    /// </summary>
    /// <example>null</example>
    public Exception AdditionalInfo { get; internal set; }
}
