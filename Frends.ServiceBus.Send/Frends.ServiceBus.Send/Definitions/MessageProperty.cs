using System.ComponentModel.DataAnnotations;

namespace Frends.ServiceBus.Send.Definitions;

/// <summary>
/// A single custom property for a Service Bus message.
/// </summary>
public class MessageProperty
{
    /// <summary>
    /// Name of the Service Bus message's custom property.
    /// </summary>
    /// <example>ExampleName</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string Name { get; set; }

    /// <summary>
    /// Value of the Service Bus message's custom property.
    /// </summary>
    /// <example>ExampleValue</example>
    [DisplayFormat(DataFormatString = "Text")]
    public object Value { get; set; }
}