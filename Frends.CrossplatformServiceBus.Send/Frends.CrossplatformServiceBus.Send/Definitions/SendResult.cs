namespace Frends.CrossplatformServiceBus.Definitions.Send;
/// <summary>
/// Return object
/// </summary>
public class SendResult
{
    /// <summary>
    /// The message identifier.
    /// </summary>
    /// <example>254e5c7a-03eb-4637-935a-e1e61345c11d</example>
    public string MessageId { get; private set; }

    /// <summary>
    /// Message's session identifier.
    /// </summary>
    /// <example>f580ef61-bd66-4676-b98f-b9804961b339</example>
    public string SessionId { get; private set; }

    /// <summary>
    /// The content type header of the sent message.
    /// </summary>
    /// <example>text/plain; charset=UTF-8</example>
    public string ContentType { get; private set; }

    internal SendResult(string messageId, string sessionId, string contentType)
    {
        MessageId = messageId;
        SessionId = sessionId;
        ContentType = contentType;
    }
}
