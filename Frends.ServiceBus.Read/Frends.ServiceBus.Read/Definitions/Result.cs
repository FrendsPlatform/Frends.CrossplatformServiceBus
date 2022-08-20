using System.Collections.Generic;

namespace Frends.ServiceBus.Read.Definitions;

/// <summary>
/// Read result.
/// </summary>
public class Result
{
    /// <summary>
    /// Read result.
    /// </summary>
    public List<ReadResult> Results { get; internal set; }
}