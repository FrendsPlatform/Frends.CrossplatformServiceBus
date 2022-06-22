using Frends.CrossplatformServiceBus.Read.Definitions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.InteropExtensions;
using Microsoft.Azure.ServiceBus.Management;
using System.Net.Mime;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.CrossplatformServiceBus.Read.Test;

[TestClass]
public class UnitTests
{
    private readonly string? _connectionString = Environment.GetEnvironmentVariable("HIQ_ServiceBus_Manage_CS");
    private readonly string? _connectionStringReadOnly = Environment.GetEnvironmentVariable("HIQ_ServiceBus_CS");

    private readonly string _queueName = "ServiceBus_Read_TestQueue";
    private readonly string _topicName = "ServiceBus_Read_TestTopic";
    private readonly string _subName = "ServiceBus_Read_TestSub";

    Input? input;
    Options? options;

    [TestCleanup]
    public async Task CleanUp()
    {
        await Cleanup();
    }

    /// <summary>
    /// Get an error because queue doesn't exists and CreateQueueOrSubscriptionIfItDoesNotExist = false.
    /// </summary>
    [TestMethod]
    public void Queue_CreateQueueOrTopicIfItDoesNotExist_False_NoExistsError_Test()
    {
        options = new Options()
        {
            BodySerializationType = BodySerializationType.String,
            DefaultEncoding = MessageEncoding.UTF8,
            CreateQueueOrTopicIfItDoesNotExist = false,
            UseCachedConnection = true,
            TimeoutSeconds = 10,
            AutoDeleteOnIdle = 5,
            TimeFormat = TimeFormat.Minutes,
            MaxSize = 1024,
        };

        input = new Input()
        {
            ConnectionString = _connectionStringReadOnly,
            QueueOrTopicName = "DoesntExists",
            SourceType = QueueOrTopic.Queue,
            SubscriptionName = null
        };

        var ex = Assert.ThrowsExceptionAsync<MessagingEntityNotFoundException>(async () => await CrossplatformServiceBus.Read(input, options, default)).Result;
        Assert.IsTrue(ex.Message.Contains("The messaging entity") && ex.Message.Contains("could not be found."));
    }

    /// <summary>
    /// Get an error because topic doesn't exists and CreateQueueOrSubscriptionIfItDoesNotExist = false.
    /// </summary>
    [TestMethod]
    public void Topic_CreateQueueOrTopicIfItDoesNotExist_False_NoExistsError_Test()
    {
        options = new Options()
        {
            BodySerializationType = BodySerializationType.String,
            DefaultEncoding = MessageEncoding.UTF8,
            CreateQueueOrTopicIfItDoesNotExist = false,
            UseCachedConnection = true,
            TimeoutSeconds = 10,
            AutoDeleteOnIdle = 5,
            TimeFormat = TimeFormat.Minutes,
            MaxSize = 1024,
        };

        input = new Input()
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = "DoesntExists",
            SourceType = QueueOrTopic.Topic,
            SubscriptionName = _subName
        };
        var ex = Assert.ThrowsExceptionAsync<MessagingEntityNotFoundException>(async () => await CrossplatformServiceBus.Read(input, options, default)).Result;
        Assert.IsTrue(ex.Message.Contains("The messaging entity") && ex.Message.Contains("could not be found."));
    }

    /// <summary>
    /// Get an error because subscription doesn't exists and CreateQueueOrSubscriptionIfItDoesNotExist = false.
    /// </summary>
    [TestMethod]
    public void Subscription_CreateQueueOrTopicIfItDoesNotExist_False_NoExistsError_Test()
    {
        options = new Options()
        {
            BodySerializationType = BodySerializationType.String,
            DefaultEncoding = MessageEncoding.UTF8,
            CreateQueueOrTopicIfItDoesNotExist = false,
            UseCachedConnection = true,
            TimeoutSeconds = 10,
            AutoDeleteOnIdle = 5,
            TimeFormat = TimeFormat.Minutes,
            MaxSize = 1024,
        };

        input = new Input()
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = _topicName,
            SourceType = QueueOrTopic.Topic,
            SubscriptionName = "DoesntExists"
        };
        var ex = Assert.ThrowsExceptionAsync<MessagingEntityNotFoundException>(async () => await CrossplatformServiceBus.Read(input, options, default)).Result;
        Assert.IsTrue(ex.Message.Contains("The messaging entity") && ex.Message.Contains("could not be found."));
    }

    /// <summary>
    /// Try to create a new queue without correct access policies.
    /// </summary>
    [TestMethod]
    public void Queue_UTF8_CreateQueueOrTopicIfItDoesNotExist_True_NoAccess_Test()
    {

        options = new Options()
        {
            BodySerializationType = BodySerializationType.String,
            DefaultEncoding = MessageEncoding.UTF8,
            CreateQueueOrTopicIfItDoesNotExist = true,
            UseCachedConnection = false,
            TimeoutSeconds = 10,
            AutoDeleteOnIdle = 5,
            TimeFormat = TimeFormat.Minutes,
            MaxSize = 1024,
        };

        input = new Input()
        {
            ConnectionString = _connectionStringReadOnly,
            QueueOrTopicName = "GetAnError",
            SourceType = QueueOrTopic.Queue,
            SubscriptionName = null
        };

        var ex = Assert.ThrowsExceptionAsync<UnauthorizedException>(async () => await CrossplatformServiceBus.Read(input, options, default)).Result;
        Assert.IsTrue(ex.Message.Contains("Authorization failed for specified action: Manage,EntityWrite"));
    }

    /// <summary>
    /// Read message from queue. Checking that content matches with sent message.
    /// </summary>
    [TestMethod]
    public async Task Queue_UTF8_CreateQueueOrTopicIfItDoesNotExist_False_Read_Test()
    {
        await Create();

        options = new Options()
        {
            BodySerializationType = BodySerializationType.String,
            DefaultEncoding = MessageEncoding.UTF8,
            CreateQueueOrTopicIfItDoesNotExist = false,
            UseCachedConnection = false,
            TimeoutSeconds = 10,
            AutoDeleteOnIdle = 5,
            TimeFormat = TimeFormat.Minutes,
            MaxSize = 1024,
        };

        input = new Input()
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = _queueName,
            SourceType = QueueOrTopic.Queue,
            SubscriptionName = null
        };

        var data = await Send(input, options, TimeSpan.FromSeconds(options.TimeoutSeconds), default);

        var result = await CrossplatformServiceBus.Read(input, options, default);
        Assert.IsTrue(result.Results.Any(x => x.Content.Contains(data) && result.Results.Any(x => x.Properties.Count == 2)));
    }

    /// <summary>
    /// Read message from Topic. Checking that content matches with sent message.
    /// </summary>
    [TestMethod]
    public async Task Subscription_UTF8_CreateQueueOrTopicIfItDoesNotExist_False_Read_Test()
    {
        await Create();

        options = new Options()
        {
            BodySerializationType = BodySerializationType.String,
            DefaultEncoding = MessageEncoding.UTF8,
            CreateQueueOrTopicIfItDoesNotExist = false,
            UseCachedConnection = false,
            TimeoutSeconds = 10,
            AutoDeleteOnIdle = 5,
            TimeFormat = TimeFormat.Minutes,
            MaxSize = 1024,
        };

        input = new Input()
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = _topicName,
            SourceType = QueueOrTopic.Topic,
            SubscriptionName = _subName
        };

        var data = await Send(input, options, TimeSpan.FromSeconds(options.TimeoutSeconds), default);

        var result = await CrossplatformServiceBus.Read(input, options, default);
        Assert.IsTrue(result.Results.Any(x => x.Content.Contains(data) && result.Results.Any(x => x.Properties.Count == 2)));
    }

    /// <summary>
    /// Create and read message from a new queue. Checking that new queue exists and content matches with sent message.
    /// </summary>
    [TestMethod]
    public async Task Queue_UTF8_CreateQueueOrTopicIfItDoesNotExist_True_Read_Test()
    {
        await Create();
        options = new Options()
        {
            BodySerializationType = BodySerializationType.String,
            DefaultEncoding = MessageEncoding.UTF8,
            CreateQueueOrTopicIfItDoesNotExist = true,
            UseCachedConnection = false,
            TimeoutSeconds = 10,
            AutoDeleteOnIdle = 5,
            TimeFormat = TimeFormat.Minutes,
            MaxSize = 1024,
        };

        input = new Input()
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = "NewTestQueue",
            SourceType = QueueOrTopic.Queue,
            SubscriptionName = null
        };

        var data = await Send(input, options, TimeSpan.FromSeconds(options.TimeoutSeconds), default);

        var result = await CrossplatformServiceBus.Read(input, options, default);
        Assert.IsTrue(await EnsureNewExists(input.SourceType.ToString().ToLower(), _connectionString, input.QueueOrTopicName, input.SubscriptionName, default));
        Assert.IsTrue(result.Results.Any(x => x.Content.Contains(data) && result.Results.Any(x => x.Properties.Count == 2)));
    }

    /// <summary>
    /// Create and read message from a subscription. Checking that new topic and subscription exists and content matches with sent message.
    /// </summary>
    [TestMethod]
    public async Task SubscriptionAndTopic_UTF8_CreateQueueOrTopicIfItDoesNotExist_True_Read_Test()
    {
        await Create();

        options = new Options()
        {
            BodySerializationType = BodySerializationType.String,
            DefaultEncoding = MessageEncoding.UTF8,
            CreateQueueOrTopicIfItDoesNotExist = true,
            UseCachedConnection = false,
            TimeoutSeconds = 10,
            AutoDeleteOnIdle = 5,
            TimeFormat = TimeFormat.Minutes,
            MaxSize = 1024,
        };

        input = new Input()
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = "NewTestTopic",
            SourceType = QueueOrTopic.Topic,
            SubscriptionName = "NewTestSub"
        };

        var data = await Send(input, options, TimeSpan.FromSeconds(options.TimeoutSeconds), default);

        var result = await CrossplatformServiceBus.Read(input, options, default);
        Assert.IsTrue(await EnsureNewExists(input.SourceType.ToString().ToLower(), _connectionString, input.QueueOrTopicName, input.SubscriptionName, default));
        Assert.IsTrue(result.Results.Any(x => x.Content.Contains(data) && result.Results.Any(x => x.Properties.Count == 2)));
    }

    /// <summary>
    /// Create and read message from a subscription. Checking that new subscription exists and content matches with sent message.
    /// </summary>
    [TestMethod]
    public async Task Subscription_UTF8_CreateQueueOrTopicIfItDoesNotExist_True_Read_Test()
    {
        await Create();

        options = new Options()
        {
            BodySerializationType = BodySerializationType.String,
            DefaultEncoding = MessageEncoding.UTF8,
            CreateQueueOrTopicIfItDoesNotExist = true,
            UseCachedConnection = false,
            TimeoutSeconds = 10,
            AutoDeleteOnIdle = 5,
            TimeFormat = TimeFormat.Minutes,
            MaxSize = 1024,
        };

        input = new Input()
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = _topicName,
            SourceType = QueueOrTopic.Topic,
            SubscriptionName = "AnotherTestSub"
        };

        var data = await Send(input, options, TimeSpan.FromSeconds(options.TimeoutSeconds), default);

        var result = await CrossplatformServiceBus.Read(input, options, default);
        Assert.IsTrue(await EnsureNewExists("subscription", _connectionString, input.QueueOrTopicName, input.SubscriptionName, default));
        Assert.IsTrue(result.Results.Any(x => x.Content.Contains(data) && result.Results.Any(x => x.Properties.Count == 2)));
    }

    /// <summary>
    /// Making sure that AutoDelete options works as expected. AutoDeleteOnIdle can't be under 5mins so 4 should be modified to 5. Check must be handled via Azure portal while debuging for now.
    /// </summary>
    [TestMethod]
    public async Task Queue_CreateQueueOrTopicIfItDoesNotExist_True_TimeFormat_Minutes_Test()
    {
        options = new Options()
        {
            BodySerializationType = BodySerializationType.String,
            DefaultEncoding = MessageEncoding.UTF8,
            CreateQueueOrTopicIfItDoesNotExist = true,
            UseCachedConnection = false,
            TimeoutSeconds = 10,
            AutoDeleteOnIdle = 4,
            TimeFormat = TimeFormat.Minutes,
            MaxSize = 1024,
        };

        input = new Input()
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = "NewTestQueue",
            SourceType = QueueOrTopic.Queue,
            SubscriptionName = null
        };

        await CrossplatformServiceBus.Read(input, options, default);
        Assert.IsTrue(await EnsureNewExists(input.SourceType.ToString().ToLower(), _connectionString, input.QueueOrTopicName, input.SubscriptionName, default));
    }

    /// <summary>
    /// Making sure that AutoDelete options works as expected. AutoDeleteOnIdle can't be under 5mins so 4 should be modified to 5. Check must be handled via Azure portal while debuging for now.
    /// </summary>
    [TestMethod]
    public async Task Queue_CreateQueueOrTopicIfItDoesNotExist_True_TimeFormat_MinutesToHour_Test()
    {
        options = new Options()
        {
            BodySerializationType = BodySerializationType.String,
            DefaultEncoding = MessageEncoding.UTF8,
            CreateQueueOrTopicIfItDoesNotExist = true,
            UseCachedConnection = false,
            TimeoutSeconds = 10,
            AutoDeleteOnIdle = 61,
            TimeFormat = TimeFormat.Minutes,
            MaxSize = 1024,
        };

        input = new Input()
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = "NewTestQueue",
            SourceType = QueueOrTopic.Queue,
            SubscriptionName = null
        };

        await CrossplatformServiceBus.Read(input, options, default);
        Assert.IsTrue(await EnsureNewExists(input.SourceType.ToString().ToLower(), _connectionString, input.QueueOrTopicName, input.SubscriptionName, default));
    }

    /// <summary>
    /// Making sure that AutoDelete options works as expected. Check must be handled via Azure portal while debuging for now.
    /// </summary>
    [TestMethod]
    public async Task Queue_CreateQueueOrTopicIfItDoesNotExist_True_TimeFormat_Hours_Test()
    {
        options = new Options()
        {
            BodySerializationType = BodySerializationType.String,
            DefaultEncoding = MessageEncoding.UTF8,
            CreateQueueOrTopicIfItDoesNotExist = true,
            UseCachedConnection = false,
            TimeoutSeconds = 10,
            AutoDeleteOnIdle = 5,
            TimeFormat = TimeFormat.Hours,
            MaxSize = 1024,
        };

        input = new Input()
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = "NewTestQueue",
            SourceType = QueueOrTopic.Queue,
            SubscriptionName = null
        };

        await CrossplatformServiceBus.Read(input, options, default);
        Assert.IsTrue(await EnsureNewExists(input.SourceType.ToString().ToLower(), _connectionString, input.QueueOrTopicName, input.SubscriptionName, default));
    }

    /// <summary>
    /// Making sure that AutoDelete options works as expected. Check must be handled via Azure portal while debuging for now.
    /// </summary>
    [TestMethod]
    public async Task Queue_CreateQueueOrTopicIfItDoesNotExist_True_TimeFormat_Days_Test()
    {
        options = new Options()
        {
            BodySerializationType = BodySerializationType.String,
            DefaultEncoding = MessageEncoding.UTF8,
            CreateQueueOrTopicIfItDoesNotExist = true,
            UseCachedConnection = false,
            TimeoutSeconds = 10,
            AutoDeleteOnIdle = 5,
            TimeFormat = TimeFormat.Days,
            MaxSize = 1024,
        };

        input = new Input()
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = "NewTestQueue",
            SourceType = QueueOrTopic.Queue,
            SubscriptionName = null
        };

        await CrossplatformServiceBus.Read(input, options, default);
        Assert.IsTrue(await EnsureNewExists(input.SourceType.ToString().ToLower(), _connectionString, input.QueueOrTopicName, input.SubscriptionName, default));
    }

    /// <summary>
    /// Making sure that AutoDelete options works as expected. Check must be handled via Azure portal while debuging for now.
    /// AutoDeleteOnIdle=0 = Never
    /// </summary>
    [TestMethod]
    public async Task Queue_CreateQueueOrTopicIfItDoesNotExist_True_TimeFormat_Never_Test()
    {
        options = new Options()
        {
            BodySerializationType = BodySerializationType.String,
            DefaultEncoding = MessageEncoding.UTF8,
            CreateQueueOrTopicIfItDoesNotExist = true,
            UseCachedConnection = false,
            TimeoutSeconds = 10,
            AutoDeleteOnIdle = 0,
            TimeFormat = TimeFormat.Minutes,
            MaxSize = 1024,
        };

        input = new Input()
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = "NewTestQueue",
            SourceType = QueueOrTopic.Queue,
            SubscriptionName = null
        };

        await CrossplatformServiceBus.Read(input, options, default);
        Assert.IsTrue(await EnsureNewExists(input.SourceType.ToString().ToLower(), _connectionString, input.QueueOrTopicName, input.SubscriptionName, default));
    }

    #region Send test message
    /// <summary>
    /// Send test message.
    /// </summary>
    /// <returns></returns>
    private static async Task<string> Send(Input input, Options options, TimeSpan timeout, CancellationToken cancellationToken)
    {
        ServiceBusConnection? connection = null;
        MessageSender? requestClient = null;

        try
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
                            deleteIdle = TimeSpan.FromHours(options.AutoDeleteOnIdle); 
                            break;
                        case TimeFormat.Days:
                            deleteIdle = TimeSpan.FromDays(options.AutoDeleteOnIdle); 
                            break;
                    }
                }


                switch (input.SourceType)
                {
                    case QueueOrTopic.Queue:
                        if (string.IsNullOrWhiteSpace(input.QueueOrTopicName) || string.IsNullOrWhiteSpace(input.ConnectionString)) throw new Exception("QueueOrTopicName and ConnectionString required.");
                        await EnsureQueueExists(input.QueueOrTopicName, input.ConnectionString, deleteIdle, options.MaxSize, cancellationToken);
                        break;
                    case QueueOrTopic.Topic:
                        if (string.IsNullOrWhiteSpace(input.QueueOrTopicName) || string.IsNullOrWhiteSpace(input.SubscriptionName) || string.IsNullOrWhiteSpace(input.ConnectionString)) throw new Exception("QueueOrTopicName, SubscriptionName and ConnectionString required.");
                        await EnsureTopicExists(input.ConnectionString, input.QueueOrTopicName, input.SubscriptionName, deleteIdle, options.MaxSize, cancellationToken);
                        break;
                    default:
                        throw new Exception($"Unexpected destination type: {input.SourceType}");
                }
            }

            requestClient = ServiceBusMessagingFactory.Instance.GetMessageSender(input.ConnectionString, input.QueueOrTopicName, timeout);

            //Create random data to make sure the correct message is picked up.
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var data = new string(Enumerable.Repeat(chars, 20)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            var createBody = CreateBody(options, data);
            byte[]? body = createBody != null ? CreateBody(options, data) : null;

            var message = new Message(body);
            message.MessageId = Guid.NewGuid().ToString();
            message.SessionId = message.SessionId;
            message.ContentType = "text/plain; charset=UTF-8";
            message.CorrelationId = null;
            message.ReplyToSessionId = null;
            message.ReplyTo = null;
            message.To = null;
            message.TimeToLive = TimeSpan.FromSeconds(options.TimeoutSeconds);
            message.ScheduledEnqueueTimeUtc = DateTime.MinValue;
            message.UserProperties.Add("PropName", "PropValue");
            message.UserProperties.Add("Prop Name 2", "Prop Value 2");

            cancellationToken.ThrowIfCancellationRequested();
            await requestClient.SendAsync(message).ConfigureAwait(false);


            return data;
        }
        catch (Exception ex) { throw new Exception(ex.ToString()); }
        finally
        {
            await (requestClient?.CloseAsync() ?? Task.CompletedTask);
            await (connection?.CloseAsync() ?? Task.CompletedTask);
        }
    }

    private static async Task EnsureQueueExists(string queueOrTopicName, string connectionString, TimeSpan deleteIdle, int MaxSize, CancellationToken cancellationToken)
    {
        var manager = new ManagementClient(connectionString);

        if (!await manager.QueueExistsAsync(queueOrTopicName, cancellationToken).ConfigureAwait(false))
        {
            var queueDescription = new QueueDescription(queueOrTopicName)
            {
                EnableBatchedOperations = true,
                MaxSizeInMB = MaxSize,
                AutoDeleteOnIdle = deleteIdle,
            };

            await manager.CreateQueueAsync(queueDescription, cancellationToken).ConfigureAwait(false);
        }
    }

    private static async Task EnsureTopicExists(string connectionString, string queueOrTopicName, string subscriptionName, TimeSpan deleteIdle, int maxSize, CancellationToken cancellationToken)
    {
        var managementClient = new ManagementClient(connectionString);

        if (!await managementClient.TopicExistsAsync(queueOrTopicName, cancellationToken).ConfigureAwait(false))
        {
            var topicDescription = new TopicDescription(queueOrTopicName)
            {
                EnableBatchedOperations = true,
                MaxSizeInMB = maxSize,
                AutoDeleteOnIdle = deleteIdle,

            };
            await managementClient.CreateTopicAsync(topicDescription, cancellationToken).ConfigureAwait(false);
        }

        await EnsureSubscriptionExists(queueOrTopicName, subscriptionName, connectionString, deleteIdle, cancellationToken);
    }

    private static async Task EnsureSubscriptionExists(string queueOrTopicName, string subscriptionName, string connectionString, TimeSpan deleteIdle, CancellationToken cancellationToken)
    {
        var manager = new ManagementClient(connectionString);

        if (!await manager.SubscriptionExistsAsync(queueOrTopicName, subscriptionName, cancellationToken).ConfigureAwait(false))
        {
            var subscriptionDescription = new SubscriptionDescription(queueOrTopicName, subscriptionName)
            {
                EnableBatchedOperations = true,
                AutoDeleteOnIdle = deleteIdle,
            };
            await manager.CreateSubscriptionAsync(subscriptionDescription, cancellationToken).ConfigureAwait(false);
        }
    }

    private static byte[]? CreateBody(Options options, string data)
    {
        var contentTypeString = "text/plain; charset=UTF-8";
        var encoding = GetEncodingFromContentType(contentTypeString, Encoding.UTF8);

        // This format matches the older Frends.CrossplatformServiceBus format to ensure interoperability
        switch (options.BodySerializationType)
        {
            case BodySerializationType.String:
                var stringResult = SerializeObject<string>(data);
                return stringResult ?? null;

            case BodySerializationType.ByteArray:
                var byteResult = SerializeObject<byte[]>(encoding.GetBytes(data));
                return byteResult ?? null;

            default:
                return encoding.GetBytes(data);
        }
    }

    internal static byte[]? SerializeObject<T>(object serializableObject)
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
    #endregion Send test message

    private static async Task<bool> EnsureNewExists(string sourceType, string? connectionString, string queueOrTopicName, string? subscriptionName, CancellationToken cancellationToken)
    {
        var managementClient = new ManagementClient(connectionString);

        switch (sourceType)
        {
            case "queue":
                if (await managementClient.QueueExistsAsync(queueOrTopicName, cancellationToken).ConfigureAwait(false))
                    return true;
                break;
            case "topic":
                if (await managementClient.TopicExistsAsync(queueOrTopicName, cancellationToken).ConfigureAwait(false))
                    return true;
                break;
            case "subscription":
                if (await managementClient.SubscriptionExistsAsync(queueOrTopicName, subscriptionName, cancellationToken).ConfigureAwait(false))
                    return true;
                break;
            default:
                return false;
        }

        return false;
    }

    private async Task Create()
    {
        var managementClient = new ManagementClient(_connectionString);

        if (!await managementClient.QueueExistsAsync(_queueName).ConfigureAwait(false))
            await managementClient.CreateQueueAsync(_queueName).ConfigureAwait(false);

        if (!await managementClient.TopicExistsAsync(_topicName).ConfigureAwait(false))
            await managementClient.CreateTopicAsync(_topicName).ConfigureAwait(false);

        if (!await managementClient.SubscriptionExistsAsync(_topicName, _subName).ConfigureAwait(false))
            await managementClient.CreateSubscriptionAsync(_topicName, _subName).ConfigureAwait(false);
    }

    private async Task Cleanup()
    {
        var managementClient = new ManagementClient(_connectionString);
        var queueNames = new List<string> { _queueName, "NewTestQueue" };
        var topicNames = new List<string> { _topicName, "NewTestTopic" };

        foreach (var queueName in queueNames)
        {
            if (await managementClient.QueueExistsAsync(queueName).ConfigureAwait(false))
                await managementClient.DeleteQueueAsync(queueName).ConfigureAwait(false);
        }

        foreach(var topicName in topicNames)
        {
            if (await managementClient.TopicExistsAsync(topicName).ConfigureAwait(false))
                await managementClient.DeleteTopicAsync(topicName).ConfigureAwait(false);
        }
    }
}