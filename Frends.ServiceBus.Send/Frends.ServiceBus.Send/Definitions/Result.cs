using System.Collections.Generic;

namespace Frends.ServiceBus.Send.Definitions;

/// <summary>
/// Send result.
/// </summary>
/// <example>{ Success = true, Error = null, Results = [ { MessageId = "254e5c7a-03eb-4637-935a-e1e61345c11d" } ] }</example>
public class Result
{
    /// <summary>
    /// Indicates whether the operation completed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; internal set; }

    /// <summary>
    /// Error details. Null when Success is true.
    /// </summary>
    /// <example>null</example>
    public Error Error { get; internal set; }

    /// <summary>
    /// Send result.
    /// </summary>
    /// <example>[ { MessageId = "254e5c7a-03eb-4637-935a-e1e61345c11d", SessionId = null, ContentType = "text/plain; charset=UTF-8" } ]</example>
    public List<SendResult> Results { get; internal set; }
}
