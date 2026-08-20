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
    /// <example>null</example>
    public List<ReadResult> Results { get; set; }
}