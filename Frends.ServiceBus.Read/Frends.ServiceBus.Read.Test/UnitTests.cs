using System.Net.Mime;
using System.Text;
using Frends.ServiceBus.Read.Definitions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.InteropExtensions;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.ServiceBus.Read.Test;

[TestClass]
public class UnitTests
{
    private readonly string? _connectionString = Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTIONSTRING");

    private readonly string? _connectionStringReadOnly =
        Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTIONSTRING_READONLY");

    private const string QueueName = "ServiceBus_Read_TestQueue";
    private const string TopicName = "ServiceBus_Read_TestTopic";
    private const string SubName = "ServiceBus_Read_TestSub";

    private Input? _input;
    private Options? _options;

    [TestCleanup]
    public async Task CleanUp()
    {
        await Cleanup();
    }

    /// <summary>
    /// Get an error because queue doesn't exist and CreateQueueOrSubscriptionIfItDoesNotExist = false.
    /// </summary>
    [TestMethod]
    public void Queue_CreateQueueOrTopicIfItDoesNotExist_False_NoExistsError_Test()
    {
        _options = new Options
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

        _input = new Input
        {
            ConnectionString = _connectionStringReadOnly,
            QueueOrTopicName = "DoesntExists",
            SourceType = QueueOrTopic.Queue,
            SubscriptionName = null
        };

        var ex = Assert
            .ThrowsExceptionAsync<MessagingEntityNotFoundException>(async () =>
                await ServiceBus.Read(_input, _options, CancellationToken.None)).Result;
        Assert.IsTrue(ex.Message.Contains("The messaging entity") && ex.Message.Contains("could not be found."));
    }

    /// <summary>
    /// Get an error because topic doesn't exist and CreateQueueOrSubscriptionIfItDoesNotExist = false.
    /// </summary>
    [TestMethod]
    public void Topic_CreateQueueOrTopicIfItDoesNotExist_False_NoExistsError_Test()
    {
        _options = new Options
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

        _input = new Input
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = "DoesntExists",
            SourceType = QueueOrTopic.Topic,
            SubscriptionName = SubName
        };
        var ex = Assert
            .ThrowsExceptionAsync<MessagingEntityNotFoundException>(async () =>
                await ServiceBus.Read(_input, _options, CancellationToken.None)).Result;
        Assert.IsTrue(ex.Message.Contains("The messaging entity") && ex.Message.Contains("could not be found."));
    }

    /// <summary>
    /// Get an error because subscription doesn't exist and CreateQueueOrSubscriptionIfItDoesNotExist = false.
    /// </summary>
    [TestMethod]
    public void Subscription_CreateQueueOrTopicIfItDoesNotExist_False_NoExistsError_Test()
    {
        _options = new Options
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

        _input = new Input
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = TopicName,
            SourceType = QueueOrTopic.Topic,
            SubscriptionName = "DoesntExists"
        };
        var ex = Assert
            .ThrowsExceptionAsync<MessagingEntityNotFoundException>(async () =>
                await ServiceBus.Read(_input, _options, CancellationToken.None)).Result;
        Assert.IsTrue(ex.Message.Contains("The messaging entity") && ex.Message.Contains("could not be found."));
    }

    /// <summary>
    /// Try to create a new queue without correct access policies.
    /// </summary>
    [TestMethod]
    public void Queue_UTF8_CreateQueueOrTopicIfItDoesNotExist_True_NoAccess_Test()
    {
        _options = new Options
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

        _input = new Input
        {
            ConnectionString = _connectionStringReadOnly,
            QueueOrTopicName = "GetAnError",
            SourceType = QueueOrTopic.Queue,
            SubscriptionName = null
        };

        var ex = Assert
            .ThrowsExceptionAsync<UnauthorizedException>(async () =>
                await ServiceBus.Read(_input, _options, CancellationToken.None))
            .Result;
        Assert.IsTrue(ex.Message.Contains("Authorization failed for specified action: Manage,EntityWrite"));
    }

    /// <summary>
    /// Read message from queue. Checking that content matches with sent message.
    /// </summary>
    [TestMethod]
    public async Task Queue_UTF8_CreateQueueOrTopicIfItDoesNotExist_False_Read_Test()
    {
        await Create();

        _options = new Options
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

        _input = new Input
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = QueueName,
            SourceType = QueueOrTopic.Queue,
            SubscriptionName = null
        };

        var data = await Send(_input, _options, TimeSpan.FromSeconds(_options.TimeoutSeconds), CancellationToken.None);

        var result = await ServiceBus.Read(_input, _options, CancellationToken.None);
        Assert.IsTrue(result.Results.Any(x =>
            x.Content.Contains(data) && result.Results.Any(readResult => readResult.Properties.Count == 2)));
    }

    /// <summary>
    /// Read message from Topic. Checking that content matches with sent message.
    /// </summary>
    [TestMethod]
    public async Task Subscription_UTF8_CreateQueueOrTopicIfItDoesNotExist_False_Read_Test()
    {
        await Create();

        _options = new Options
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

        _input = new Input
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = TopicName,
            SourceType = QueueOrTopic.Topic,
            SubscriptionName = SubName
        };

        var data = await Send(_input, _options, TimeSpan.FromSeconds(_options.TimeoutSeconds), CancellationToken.None);

        var result = await ServiceBus.Read(_input, _options, CancellationToken.None);
        Assert.IsTrue(result.Results.Any(x =>
            x.Content.Contains(data) && result.Results.Any(readResult => readResult.Properties.Count == 2)));
    }

    /// <summary>
    /// Create and read message from a new queue. Checking that new queue exists and content matches with sent message.
    /// </summary>
    [TestMethod]
    public async Task Queue_UTF8_CreateQueueOrTopicIfItDoesNotExist_True_Read_Test()
    {
        await Create();
        _options = new Options
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

        _input = new Input
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = "NewTestQueue",
            SourceType = QueueOrTopic.Queue,
            SubscriptionName = null
        };

        var data = await Send(_input, _options, TimeSpan.FromSeconds(_options.TimeoutSeconds), CancellationToken.None);

        var result = await ServiceBus.Read(_input, _options, CancellationToken.None);
        Assert.IsTrue(await EnsureNewExists(_input.SourceType.ToString().ToLower(), _connectionString,
            _input.QueueOrTopicName, _input.SubscriptionName, CancellationToken.None));
        Assert.IsTrue(result.Results.Any(x =>
            x.Content.Contains(data) && result.Results.Any(readResult => readResult.Properties.Count == 2)));
    }

    /// <summary>
    /// Create and read message from a subscription. Checking that new topic and subscription exists and content matches with sent message.
    /// </summary>
    [TestMethod]
    public async Task SubscriptionAndTopic_UTF8_CreateQueueOrTopicIfItDoesNotExist_True_Read_Test()
    {
        await Create();

        _options = new Options
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

        _input = new Input
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = "NewTestTopic",
            SourceType = QueueOrTopic.Topic,
            SubscriptionName = "NewTestSub"
        };

        var data = await Send(_input, _options, TimeSpan.FromSeconds(_options.TimeoutSeconds), CancellationToken.None);

        var result = await ServiceBus.Read(_input, _options, CancellationToken.None);
        Assert.IsTrue(await EnsureNewExists(_input.SourceType.ToString().ToLower(), _connectionString,
            _input.QueueOrTopicName, _input.SubscriptionName, CancellationToken.None));
        Assert.IsTrue(result.Results.Any(x =>
            x.Content.Contains(data) && result.Results.Any(readResult => readResult.Properties.Count == 2)));
    }

    /// <summary>
    /// Create and read message from a subscription. Checking that new subscription exists and content matches with sent message.
    /// </summary>
    [TestMethod]
    public async Task Subscription_UTF8_CreateQueueOrTopicIfItDoesNotExist_True_Read_Test()
    {
        await Create();

        _options = new Options
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

        _input = new Input
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = TopicName,
            SourceType = QueueOrTopic.Topic,
            SubscriptionName = "AnotherTestSub"
        };

        var data = await Send(_input, _options, TimeSpan.FromSeconds(_options.TimeoutSeconds), CancellationToken.None);

        var result = await ServiceBus.Read(_input, _options, CancellationToken.None);
        Assert.IsTrue(await EnsureNewExists("subscription", _connectionString, _input.QueueOrTopicName,
            _input.SubscriptionName, CancellationToken.None));
        Assert.IsTrue(result.Results.Any(x =>
            x.Content.Contains(data) && result.Results.Any(readResult => readResult.Properties.Count == 2)));
    }

    /// <summary>
    /// Making sure that AutoDelete options works as expected. AutoDeleteOnIdle can't be under 5mins so 4 should be modified to 5. Check must be handled via Azure portal while debugging for now.
    /// </summary>
    [TestMethod]
    public async Task Queue_CreateQueueOrTopicIfItDoesNotExist_True_TimeFormat_Minutes_Test()
    {
        _options = new Options
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

        _input = new Input
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = "NewTestQueue",
            SourceType = QueueOrTopic.Queue,
            SubscriptionName = null
        };

        await ServiceBus.Read(_input, _options, CancellationToken.None);
        Assert.IsTrue(await EnsureNewExists(_input.SourceType.ToString().ToLower(), _connectionString,
            _input.QueueOrTopicName, _input.SubscriptionName, CancellationToken.None));
    }

    /// <summary>
    /// Making sure that AutoDelete options works as expected. AutoDeleteOnIdle can't be under 5mins so 4 should be modified to 5. Check must be handled via Azure portal while debugging for now.
    /// </summary>
    [TestMethod]
    public async Task Queue_CreateQueueOrTopicIfItDoesNotExist_True_TimeFormat_MinutesToHour_Test()
    {
        _options = new Options
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

        _input = new Input
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = "NewTestQueue",
            SourceType = QueueOrTopic.Queue,
            SubscriptionName = null
        };

        await ServiceBus.Read(_input, _options, CancellationToken.None);
        Assert.IsTrue(await EnsureNewExists(_input.SourceType.ToString().ToLower(), _connectionString,
            _input.QueueOrTopicName, _input.SubscriptionName, CancellationToken.None));
    }

    /// <summary>
    /// Making sure that AutoDelete options works as expected. Check must be handled via Azure portal while debugging for now.
    /// </summary>
    [TestMethod]
    public async Task Queue_CreateQueueOrTopicIfItDoesNotExist_True_TimeFormat_Hours_Test()
    {
        _options = new Options
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

        _input = new Input
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = "NewTestQueue",
            SourceType = QueueOrTopic.Queue,
            SubscriptionName = null
        };

        await ServiceBus.Read(_input, _options, CancellationToken.None);
        Assert.IsTrue(await EnsureNewExists(_input.SourceType.ToString().ToLower(), _connectionString,
            _input.QueueOrTopicName, _input.SubscriptionName, CancellationToken.None));
    }

    /// <summary>
    /// Making sure that AutoDelete options works as expected. Check must be handled via Azure portal while debugging for now.
    /// </summary>
    [TestMethod]
    public async Task Queue_CreateQueueOrTopicIfItDoesNotExist_True_TimeFormat_Days_Test()
    {
        _options = new Options
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

        _input = new Input
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = "NewTestQueue",
            SourceType = QueueOrTopic.Queue,
            SubscriptionName = null
        };

        await ServiceBus.Read(_input, _options, CancellationToken.None);
        Assert.IsTrue(await EnsureNewExists(_input.SourceType.ToString().ToLower(), _connectionString,
            _input.QueueOrTopicName, _input.SubscriptionName, CancellationToken.None));
    }

    /// <summary>
    /// Making sure that AutoDelete options works as expected. Check must be handled via Azure portal while debugging for now.
    /// AutoDeleteOnIdle=0 = Never
    /// </summary>
    [TestMethod]
    public async Task Queue_CreateQueueOrTopicIfItDoesNotExist_True_TimeFormat_Never_Test()
    {
        _options = new Options
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

        _input = new Input
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = "NewTestQueue",
            SourceType = QueueOrTopic.Queue,
            SubscriptionName = null
        };

        await ServiceBus.Read(_input, _options, CancellationToken.None);
        Assert.IsTrue(await EnsureNewExists(_input.SourceType.ToString().ToLower(), _connectionString,
            _input.QueueOrTopicName, _input.SubscriptionName, CancellationToken.None));
    }

    [TestMethod]
    public void ReadMessage_SerializationType_String()
    {
        var serializer = DataContractBinarySerializer<string>.Instance;
        using var stream = new MemoryStream();
        serializer.WriteObject(stream, "foo");
        var msg = new Message(stream.ToArray());
        var result = MessageReader.Read(msg, BodySerializationType.String, MessageEncoding.UTF8);
        Assert.AreEqual(result, "foo");
    }

    [TestMethod]
    public void ReadMessage_SerializationType_Stream()
    {
        var bytes = Encoding.UTF8.GetBytes("foo");
        var msg = new Message(bytes)
        {
            ContentType = "text/plain; charset=ascii"
        };

        var result = MessageReader.Read(msg, BodySerializationType.Stream, MessageEncoding.ASCII);
        Assert.AreEqual("foo", result);
    }

    [TestMethod]
    public void ReadMessage_SerializationType_ByteArray()
    {
        var textInBytes = Encoding.UTF8.GetBytes("foo");
        var serializer = DataContractBinarySerializer<byte[]>.Instance;
        using var stream = new MemoryStream();
        serializer.WriteObject(stream, textInBytes);
        var msg = new Message(stream.ToArray())
        {
            ContentType = "text/plain; charset=utf-8"
        };

        var result = MessageReader.Read(msg, BodySerializationType.ByteArray, MessageEncoding.UTF8);
        Assert.AreEqual("foo", result);
    }

    #region Send test message

    /// <summary>
    /// Send test message.
    /// </summary>
    /// <returns></returns>
    private static async Task<string> Send(Input input, Options options, TimeSpan timeout,
        CancellationToken cancellationToken)
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
                            deleteIdle = options.AutoDeleteOnIdle > 5
                                ? TimeSpan.FromMinutes(options.AutoDeleteOnIdle)
                                : TimeSpan.FromMinutes(5);
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
                        if (string.IsNullOrWhiteSpace(input.QueueOrTopicName) ||
                            string.IsNullOrWhiteSpace(input.ConnectionString))
                            throw new Exception("QueueOrTopicName and ConnectionString required.");
                        await EnsureQueueExists(input.QueueOrTopicName, input.ConnectionString, deleteIdle,
                            options.MaxSize, cancellationToken);
                        break;
                    case QueueOrTopic.Topic:
                        if (string.IsNullOrWhiteSpace(input.QueueOrTopicName) ||
                            string.IsNullOrWhiteSpace(input.SubscriptionName) ||
                            string.IsNullOrWhiteSpace(input.ConnectionString))
                            throw new Exception("QueueOrTopicName, SubscriptionName and ConnectionString required.");
                        await EnsureTopicExists(input.ConnectionString, input.QueueOrTopicName, input.SubscriptionName,
                            deleteIdle, options.MaxSize, cancellationToken);
                        break;
                    default:
                        throw new Exception($"Unexpected destination type: {input.SourceType}");
                }
            }

            requestClient =
                ServiceBusMessagingFactory.Instance.GetMessageSender(input.ConnectionString, input.QueueOrTopicName,
                    timeout);

            //Create random data to make sure the correct message is picked up.
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var data = new string(Enumerable.Repeat(chars, 20)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            var createBody = CreateBody(options, data);
            var body = createBody != null ? CreateBody(options, data) : null;

            var message = new Message(body)
            {
                MessageId = Guid.NewGuid().ToString()
            };
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
        catch (Exception ex)
        {
            throw new Exception(ex.ToString());
        }
        finally
        {
            await (requestClient?.CloseAsync() ?? Task.CompletedTask);
            await (connection?.CloseAsync() ?? Task.CompletedTask);
        }
    }

    private static async Task EnsureQueueExists(string queueOrTopicName, string connectionString, TimeSpan deleteIdle,
        int maxSize, CancellationToken cancellationToken)
    {
        var manager = new ManagementClient(connectionString);

        if (!await manager.QueueExistsAsync(queueOrTopicName, cancellationToken).ConfigureAwait(false))
        {
            var queueDescription = new QueueDescription(queueOrTopicName)
            {
                EnableBatchedOperations = true,
                MaxSizeInMB = maxSize,
                AutoDeleteOnIdle = deleteIdle,
            };

            await manager.CreateQueueAsync(queueDescription, cancellationToken).ConfigureAwait(false);
        }
    }

    private static async Task EnsureTopicExists(string connectionString, string queueOrTopicName,
        string subscriptionName, TimeSpan deleteIdle, int maxSize, CancellationToken cancellationToken)
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

        await EnsureSubscriptionExists(queueOrTopicName, subscriptionName, connectionString, deleteIdle,
            cancellationToken);
    }

    private static async Task EnsureSubscriptionExists(string queueOrTopicName, string subscriptionName,
        string connectionString, TimeSpan deleteIdle, CancellationToken cancellationToken)
    {
        var manager = new ManagementClient(connectionString);

        if (!await manager.SubscriptionExistsAsync(queueOrTopicName, subscriptionName, cancellationToken)
                .ConfigureAwait(false))
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

        // This format matches the older Frends.ServiceBus format to ensure interoperability
        switch (options.BodySerializationType)
        {
            case BodySerializationType.String:
                var stringResult = SerializeObject<string>(data);
                return stringResult ?? null;

            case BodySerializationType.ByteArray:
                var byteResult = SerializeObject<byte[]>(encoding.GetBytes(data));
                return byteResult ?? null;

            case BodySerializationType.Stream:
            default:
                return encoding.GetBytes(data);
        }
    }

    private static byte[]? SerializeObject<T>(object? serializableObject)
    {
        var serializer = DataContractBinarySerializer<T>.Instance;
        if (serializableObject == null)
            return null;

        using var memoryStream = new MemoryStream(256);
        serializer.WriteObject(memoryStream, serializableObject);
        memoryStream.Flush();
        memoryStream.Position = 0L;
        return memoryStream.ToArray();
    }

    private static Encoding GetEncodingFromContentType(string contentTypeString, Encoding defaultEncoding)
    {
        var encoding = defaultEncoding;
        if (string.IsNullOrEmpty(contentTypeString)) return encoding;

        var contentType = new ContentType(contentTypeString);
        if (!string.IsNullOrEmpty(contentType.CharSet))
            encoding = Encoding.GetEncoding(contentType.CharSet);

        return encoding;
    }

    #endregion Send test message

    private static async Task<bool> EnsureNewExists(string sourceType, string? connectionString,
        string queueOrTopicName, string? subscriptionName, CancellationToken cancellationToken)
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
                if (await managementClient
                        .SubscriptionExistsAsync(queueOrTopicName, subscriptionName, cancellationToken)
                        .ConfigureAwait(false))
                    return true;
                break;
        }

        return false;
    }

    private async Task Create()
    {
        var managementClient = new ManagementClient(_connectionString);

        if (!await managementClient.QueueExistsAsync(QueueName).ConfigureAwait(false))
            await managementClient.CreateQueueAsync(QueueName).ConfigureAwait(false);

        if (!await managementClient.TopicExistsAsync(TopicName).ConfigureAwait(false))
            await managementClient.CreateTopicAsync(TopicName).ConfigureAwait(false);

        if (!await managementClient.SubscriptionExistsAsync(TopicName, SubName).ConfigureAwait(false))
            await managementClient.CreateSubscriptionAsync(TopicName, SubName).ConfigureAwait(false);
    }

    private async Task Cleanup()
    {
        var managementClient = new ManagementClient(_connectionString);
        var queueNames = new List<string> { QueueName, "NewTestQueue" };
        var topicNames = new List<string> { TopicName, "NewTestTopic" };

        foreach (var queueName in queueNames)
        {
            if (await managementClient.QueueExistsAsync(queueName))
                await managementClient.DeleteQueueAsync(queueName);
        }

        foreach (var topicName in topicNames)
        {
            if (await managementClient.TopicExistsAsync(topicName))
                await managementClient.DeleteTopicAsync(topicName);
        }
    }
}