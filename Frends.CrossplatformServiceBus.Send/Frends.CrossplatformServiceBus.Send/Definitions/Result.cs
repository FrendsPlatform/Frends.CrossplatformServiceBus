using System.Collections.Generic;

namespace Frends.CrossplatformServiceBus.Definitions.Send;

/// <summary>
/// Send result.
/// </summary>
public class Result
{
    /// <summary>
    /// Send result.
    /// </summary>
    public List<SendResult> Results { get; internal set; }
}
