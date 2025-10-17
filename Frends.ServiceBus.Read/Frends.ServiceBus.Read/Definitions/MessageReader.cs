using System;
using System.Net.Mime;
using System.Text;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.InteropExtensions;

namespace Frends.ServiceBus.Read.Definitions;

/// <summary>
/// Class for reading the message body according to the selected serialization type
/// </summary>
public static class MessageReader
{
    /// <summary>
    /// Read the message body according to the selected serialization type
    /// </summary>
    public static string Read(Message msg, BodySerializationType serializationType, MessageEncoding messageEncoding)
    {
        switch (serializationType)
        {
            case BodySerializationType.String:
                return msg.GetBody<string>();
            case BodySerializationType.ByteArray:
                var byteArrayEncoding = GetEncodingFromContentType(msg.ContentType, messageEncoding);
                var messageBytesArray = msg.GetBody<byte[]>();
                return messageBytesArray == null ? null : byteArrayEncoding.GetString(messageBytesArray);
            case BodySerializationType.Stream:
                var streamEncoding = GetEncodingFromContentType(msg.ContentType, messageEncoding);
                var messageStreamBytes = msg.Body;
                return messageStreamBytes == null ? null : streamEncoding.GetString(messageStreamBytes);
            default:
                throw new ArgumentException($"Unsupported BodySerializationType: {serializationType}");
        }
    }

    private static Encoding GetEncodingFromContentType(string contentTypeString, MessageEncoding encodingChoice)
    {
        var encoding = encodingChoice switch
        {
            MessageEncoding.UTF8 => Encoding.UTF8,
            MessageEncoding.UTF32 => Encoding.UTF32,
            MessageEncoding.ASCII => Encoding.ASCII,
            MessageEncoding.Unicode => Encoding.Unicode,
            MessageEncoding.Latin1 => Encoding.Latin1,
            MessageEncoding.BigEndianUnicode => Encoding.BigEndianUnicode,
            _ => null
        };

        if (string.IsNullOrEmpty(contentTypeString)) return encoding;

        var contentType = new ContentType(contentTypeString);
        if (!string.IsNullOrEmpty(contentType.CharSet))
            encoding = Encoding.GetEncoding(contentType.CharSet);

        return encoding;
    }
}