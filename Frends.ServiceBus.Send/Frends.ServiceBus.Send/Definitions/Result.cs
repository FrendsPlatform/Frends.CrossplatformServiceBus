﻿using System.Collections.Generic;

namespace Frends.ServiceBus.Send.Definitions;

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
