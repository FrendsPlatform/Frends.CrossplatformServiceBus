using System;

namespace Frends.ServiceBus.Read.Definitions;

/// <summary>
/// Error details for a failed operation.
/// </summary>
public class Error
{
    /// <summary>
    /// Error message.
    /// </summary>
    /// <example>The messaging entity could not be found.</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional error information, typically the original exception.
    /// </summary>
    /// <example>null</example>
    public Exception AdditionalInfo { get; set; }
}
