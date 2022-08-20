﻿using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Frends.ServiceBus.Send.Definitions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using System.Text;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Azure.ServiceBus.InteropExtensions;
using System.Net.Mime;
using System.IO;
using System.Collections.Generic;

namespace Frends.ServiceBus.Send;

/// <summary>
/// Azure Service Bus task.
/// </summary>
public class ServiceBus
{
    /// <summary>
    /// Send message to Azure Service Bus queue or topic.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.ServiceBus.Send)
    /// </summary>
    /// <param name="input">Input parameters</param>
    /// <param name="options">Options parameters</param>
    /// <param name="cancellationToken">Token generated by Frends to stop this task.</param>
    /// <returns>List { string messageId, string sessionId, string contentType }</returns>
    public static async Task<Result> Send([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
    {
        if (options.CreateQueueOrTopicIfItDoesNotExist)
        {
            var deleteIdle = TimeSpan.Zero;

            if (options.AutoDeleteOnIdle > 0)
            {
                switch (options.TimeFormat)
                {
                    case TimeFormat.Minutes:
                        deleteIdle = options.AutoDeleteOnIdle > 5 ? TimeSpan.FromMinutes(options.AutoDeleteOnIdle) : TimeSpan.FromMinutes(5);
                        break;
                    case TimeFormat.Hours:
                        deleteIdle = options.AutoDeleteOnIdle > 0.0833333333 ? TimeSpan.FromHours(options.AutoDeleteOnIdle) : TimeSpan.FromMinutes(5);
                        break;
                    case TimeFormat.Days:
                        deleteIdle = options.AutoDeleteOnIdle > 0.00347222222 ? TimeSpan.FromDays(options.AutoDeleteOnIdle) : TimeSpan.FromMinutes(5);
                        break;
                }
            }

            switch (input.DestinationType)
            {
                case QueueOrTopic.Queue:
                    if (string.IsNullOrWhiteSpace(input.QueueOrTopicName) || string.IsNullOrWhiteSpace(input.ConnectionString)) throw new Exception("Connection parameters required.");
                    await EnsureQueueExists(input.QueueOrTopicName, input.ConnectionString, deleteIdle, options.MaxSize, cancellationToken);
                    break;
                case QueueOrTopic.Topic:
                    if (string.IsNullOrWhiteSpace(input.QueueOrTopicName) || string.IsNullOrWhiteSpace(input.SubscriptionName) || string.IsNullOrWhiteSpace(input.ConnectionString)) throw new Exception("Connection parameters required.");
                    await EnsureTopicExists(input.ConnectionString, input.QueueOrTopicName, input.SubscriptionName, deleteIdle, options.MaxSize, cancellationToken);
                    break;
                default:
                    throw new Exception($"Unexpected destination type: {input.DestinationType}");
            }
        }

        return new Result { Results = await DoQueueSendOperation(input, options, TimeSpan.FromSeconds(options.TimeoutSeconds)) };
    }

    private static async Task<List<SendResult>> DoQueueSendOperation(Input input, Options options, TimeSpan timeout)
    {
        ServiceBusConnection connection = null;
        MessageSender requestClient = null;
        var result = new List<SendResult>();
        
        try
        {
            if (options.UseCachedConnection)
                requestClient = ServiceBusMessagingFactory.Instance.GetMessageSender(input.ConnectionString, input.QueueOrTopicName, timeout);
            else
            {
                connection = ServiceBusMessagingFactory.CreateConnectionWithTimeout(input.ConnectionString, timeout);
                requestClient = new MessageSender(input.ConnectionString, input.QueueOrTopicName);
            }

            byte[] body = CreateBody(input, options);

            var message = new Message(body);
            message.MessageId = string.IsNullOrEmpty(options.MessageId) ? Guid.NewGuid().ToString() : options.MessageId;
            message.SessionId = string.IsNullOrEmpty(options.SessionId) ? message.SessionId : options.SessionId;
            message.ContentType = options.ContentType;
            message.CorrelationId = options.CorrelationId;
            message.ReplyToSessionId = options.ReplyToSessionId;
            message.ReplyTo = options.ReplyTo;
            message.To = options.To;
            message.TimeToLive = options.TimeToLiveSeconds.HasValue ? TimeSpan.FromSeconds(options.TimeToLiveSeconds.Value) : TimeSpan.MaxValue;
            message.ScheduledEnqueueTimeUtc = options.ScheduledEnqueueTimeUtc.HasValue ? (DateTime)options.ScheduledEnqueueTimeUtc : DateTime.UtcNow;

            foreach (var property in input.Properties)
                message.UserProperties.Add(property.Name, property.Value);

            await requestClient.SendAsync(message).ConfigureAwait(false);
            result.Add(new SendResult(message.MessageId, message.SessionId, message.ContentType));

            return result;
        }
        finally
        {
            await (requestClient?.CloseAsync() ?? Task.CompletedTask);
            await (connection?.CloseAsync() ?? Task.CompletedTask);
        }
    }

    private static async Task EnsureQueueExists(string queueOrTopicName, string connectionString, TimeSpan deleteIdle, int MaxSize, CancellationToken cancellationToken)
    {
        var manager = new ManagementClient(connectionString);
        QueueDescription queueDescription;

        if (!await manager.QueueExistsAsync(queueOrTopicName, cancellationToken).ConfigureAwait(false))
        {
            if (deleteIdle == TimeSpan.Zero)
            {
                queueDescription = new QueueDescription(queueOrTopicName)
                {
                    EnableBatchedOperations = true,
                    MaxSizeInMB = MaxSize,
                };
            }
            else
            {
                queueDescription = new QueueDescription(queueOrTopicName)
                {
                    EnableBatchedOperations = true,
                    MaxSizeInMB = MaxSize,
                    AutoDeleteOnIdle = deleteIdle,
                };
            }
            await manager.CreateQueueAsync(queueDescription, cancellationToken).ConfigureAwait(false);
        }
    }

    private static async Task EnsureTopicExists(string connectionString, string queueOrTopicName, string subscriptionName, TimeSpan deleteIdle, int maxSize, CancellationToken cancellationToken)
    {
        var managementClient = new ManagementClient(connectionString);
        TopicDescription topicDescription;

        if (!await managementClient.TopicExistsAsync(queueOrTopicName, cancellationToken).ConfigureAwait(false))
        {
            if (deleteIdle == TimeSpan.Zero)
            {
                topicDescription = new TopicDescription(queueOrTopicName)
                {
                    EnableBatchedOperations = true,
                    MaxSizeInMB = maxSize,
                };
            }
            else
            {
                topicDescription = new TopicDescription(queueOrTopicName)
                {
                    EnableBatchedOperations = true,
                    MaxSizeInMB = maxSize,
                    AutoDeleteOnIdle = deleteIdle,
                };
            }
            await managementClient.CreateTopicAsync(topicDescription, cancellationToken).ConfigureAwait(false);
        }
        await EnsureSubscriptionExists(queueOrTopicName, subscriptionName, connectionString, deleteIdle, cancellationToken);
    }

    private static async Task EnsureSubscriptionExists(string queueOrTopicName, string subscriptionName, string connectionString, TimeSpan deleteIdle, CancellationToken cancellationToken)
    {
        var manager = new ManagementClient(connectionString);
        SubscriptionDescription subscriptionDescription;

        if (!await manager.SubscriptionExistsAsync(queueOrTopicName, subscriptionName, cancellationToken).ConfigureAwait(false))
        {
            if (deleteIdle == TimeSpan.Zero)
            {
                subscriptionDescription = new SubscriptionDescription(queueOrTopicName, subscriptionName)
                {
                    EnableBatchedOperations = true,
                };
            }
            else
            {
                subscriptionDescription = new SubscriptionDescription(queueOrTopicName, subscriptionName)
                {
                    EnableBatchedOperations = true,
                    AutoDeleteOnIdle = deleteIdle,
                };
            }
            await manager.CreateSubscriptionAsync(subscriptionDescription, cancellationToken).ConfigureAwait(false);
        }
    }

    private static byte[] CreateBody(Input input, Options options)
    {
        if (input.Data == null)
            return null;

        var contentTypeString = options.ContentType;
        var encoding = GetEncodingFromContentType(contentTypeString, Encoding.UTF8);

        // This format matches the older Frends.ServiceBus format to ensure interoperability
        return options.BodySerializationType switch
        {
            BodySerializationType.String => SerializeObject<string>(input.Data),
            BodySerializationType.ByteArray => SerializeObject<byte[]>(encoding.GetBytes(input.Data)),
            _ => encoding.GetBytes(input.Data),
        };
    }

    internal static byte[] SerializeObject<T>(object serializableObject)
    {
        var serializer = DataContractBinarySerializer<T>.Instance;
        if (serializableObject == null)
            return null;

        using MemoryStream memoryStream = new MemoryStream(256);
        serializer.WriteObject(memoryStream, serializableObject);
        memoryStream.Flush();
        memoryStream.Position = 0L;
        return memoryStream.ToArray();
    }

    private static Encoding GetEncodingFromContentType(string contentTypeString, Encoding defaultEncoding)
    {
        Encoding encoding = defaultEncoding;
        if (!string.IsNullOrEmpty(contentTypeString))
        {
            var contentType = new ContentType(contentTypeString);
            if (!string.IsNullOrEmpty(contentType.CharSet))
                encoding = Encoding.GetEncoding(contentType.CharSet);
        }
        return encoding;
    }
}