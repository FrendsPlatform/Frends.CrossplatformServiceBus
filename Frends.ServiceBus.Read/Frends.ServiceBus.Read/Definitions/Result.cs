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
    public ReadResult Results { get; private set; }

    internal Result(ReadResult results)
    {
        Results = results;
    }
}
