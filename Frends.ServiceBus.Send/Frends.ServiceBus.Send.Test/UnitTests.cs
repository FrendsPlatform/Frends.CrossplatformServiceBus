using Frends.ServiceBus.Send.Definitions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.ServiceBus.Send.Test;

[TestClass]
public class UnitTests
{
    private readonly string? _connectionString = Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTIONSTRING");

    private readonly string? _connectionStringReadOnly =
        Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTIONSTRING_READONLY");

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
            UseCachedConnection = true,
            TimeoutSeconds = 10,
            ContentType = "text/plain; charset=UTF-8",
            ScheduledEnqueueTimeUtc = DateTime.UtcNow,
            TimeToLiveSeconds = 30,
            CreateQueueOrTopicIfItDoesNotExist = false,
            AutoDeleteOnIdle = 5,
            TimeFormat = TimeFormat.Minutes,
            MaxSize = 1024,
            CorrelationId = null,
            SessionId = null,
            ReplyToSessionId = null,
            MessageId = null,
            ReplyTo = null,
            To = null
        };


        input = new Input()
        {
            ConnectionString = _connectionStringReadOnly,
            QueueOrTopicName = "DoesntExists",
            DestinationType = QueueOrTopic.Queue,
            SubscriptionName = null,
            Data = "test",
            Properties = Array.Empty<MessageProperty>()
        };

        var ex = Assert
            .ThrowsExceptionAsync<MessagingEntityNotFoundException>(async () =>
                await ServiceBus.Send(input, options, default)).Result;
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
            UseCachedConnection = true,
            TimeoutSeconds = 10,
            ContentType = "text/plain; charset=UTF-8",
            ScheduledEnqueueTimeUtc = DateTime.UtcNow,
            TimeToLiveSeconds = 30,
            CreateQueueOrTopicIfItDoesNotExist = false,
            AutoDeleteOnIdle = 5,
            TimeFormat = TimeFormat.Minutes,
            MaxSize = 1024,
            CorrelationId = null,
            SessionId = null,
            ReplyToSessionId = null,
            MessageId = null,
            ReplyTo = null,
            To = null
        };

        input = new Input()
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = "DoesntExists",
            DestinationType = QueueOrTopic.Topic,
            SubscriptionName = _subName,
            Data = "test",
            Properties = Array.Empty<MessageProperty>()
        };
        var ex = Assert
            .ThrowsExceptionAsync<MessagingEntityNotFoundException>(async () =>
                await ServiceBus.Send(input, options, default)).Result;
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
            UseCachedConnection = true,
            TimeoutSeconds = 10,
            ContentType = "text/plain; charset=UTF-8",
            ScheduledEnqueueTimeUtc = DateTime.UtcNow,
            TimeToLiveSeconds = 30,
            CreateQueueOrTopicIfItDoesNotExist = false,
            AutoDeleteOnIdle = 5,
            TimeFormat = TimeFormat.Minutes,
            MaxSize = 1024,
            CorrelationId = null,
            SessionId = null,
            ReplyToSessionId = null,
            MessageId = null,
            ReplyTo = null,
            To = null
        };

        input = new Input()
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = _topicName,
            DestinationType = QueueOrTopic.Topic,
            SubscriptionName = "DoesntExists",
            Data = "test",
            Properties = Array.Empty<MessageProperty>()
        };
        var ex = Assert
            .ThrowsExceptionAsync<MessagingEntityNotFoundException>(async () =>
                await ServiceBus.Send(input, options, default)).Result;
        Assert.IsTrue(ex.Message.Contains("The messaging entity") && ex.Message.Contains("could not be found."));
    }

    /// <summary>
    /// Try to create a new queue without correct access policies.
    /// </summary>
    [TestMethod]
    public void Queue_CreateQueueOrTopicIfItDoesNotExist_True_NoAccess_Test()
    {
        options = new Options()
        {
            BodySerializationType = BodySerializationType.String,
            UseCachedConnection = false,
            TimeoutSeconds = 10,
            ContentType = "text/plain; charset=UTF-8",
            ScheduledEnqueueTimeUtc = DateTime.UtcNow,
            TimeToLiveSeconds = 30,
            CreateQueueOrTopicIfItDoesNotExist = true,
            AutoDeleteOnIdle = 5,
            TimeFormat = TimeFormat.Minutes,
            MaxSize = 1024,
            CorrelationId = null,
            SessionId = null,
            ReplyToSessionId = null,
            MessageId = null,
            ReplyTo = null,
            To = null
        };

        input = new Input()
        {
            ConnectionString = _connectionStringReadOnly,
            QueueOrTopicName = "GetAnError",
            DestinationType = QueueOrTopic.Queue,
            SubscriptionName = null,
            Data = "test",
            Properties = Array.Empty<MessageProperty>()
        };

        var ex = Assert
            .ThrowsExceptionAsync<UnauthorizedException>(async () => await ServiceBus.Send(input, options, default))
            .Result;
        Assert.IsTrue(ex.Message.Contains("Authorization failed for specified action: Manage,EntityWrite"));
    }

    /// <summary>
    /// Create and send message to new queue.
    /// </summary>
    [TestMethod]
    public async Task Queue_CreateQueueOrTopicIfItDoesNotExist_True_Send_Test()
    {
        options = new Options()
        {
            BodySerializationType = BodySerializationType.String,
            UseCachedConnection = false,
            TimeoutSeconds = 10,
            ContentType = "text/plain; charset=UTF-8",
            ScheduledEnqueueTimeUtc = DateTime.UtcNow,
            TimeToLiveSeconds = 30,
            CreateQueueOrTopicIfItDoesNotExist = true,
            AutoDeleteOnIdle = 5,
            TimeFormat = TimeFormat.Minutes,
            MaxSize = 1024,
            CorrelationId = null,
            SessionId = null,
            ReplyToSessionId = null,
            MessageId = null,
            ReplyTo = null,
            To = null
        };

        input = new Input()
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = "NewTestQueue",
            DestinationType = QueueOrTopic.Queue,
            SubscriptionName = null,
            Data = "test",
            Properties = Array.Empty<MessageProperty>()
        };

        var result = await ServiceBus.Send(input, options, default);
        Assert.IsTrue(result.Results.Any(x => x.MessageId != null));
    }

    /// <summary>
    /// Create and send message to new subscription.
    /// </summary>
    [TestMethod]
    public async Task SubscriptionAndTopic_CreateQueueOrTopicIfItDoesNotExist_True_Send_Test()
    {
        options = new Options()
        {
            BodySerializationType = BodySerializationType.String,
            UseCachedConnection = false,
            TimeoutSeconds = 10,
            ContentType = "text/plain; charset=UTF-8",
            ScheduledEnqueueTimeUtc = DateTime.UtcNow,
            TimeToLiveSeconds = 30,
            CreateQueueOrTopicIfItDoesNotExist = true,
            AutoDeleteOnIdle = 5,
            TimeFormat = TimeFormat.Minutes,
            MaxSize = 1024,
            CorrelationId = null,
            SessionId = null,
            ReplyToSessionId = null,
            MessageId = null,
            ReplyTo = null,
            To = null
        };

        input = new Input()
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = "NewTestTopic",
            DestinationType = QueueOrTopic.Topic,
            SubscriptionName = "NewTestSub",
            Data = "test",
            Properties = Array.Empty<MessageProperty>()
        };

        var result = await ServiceBus.Send(input, options, default);
        Assert.IsTrue(result.Results.Any(x => x.MessageId != null));
    }

    /// <summary>
    /// Making sure that AutoDelete options works as expected. AutoDeleteOnIdle can't be under 5mins so 4 should be modified to 5. Check must be handled via Azure portal while debuging.
    /// </summary>
    [TestMethod]
    public async Task Queue_CreateQueueOrTopicIfItDoesNotExist_True_TimeFormat_Minutes_Test()
    {
        options = new Options()
        {
            BodySerializationType = BodySerializationType.String,
            UseCachedConnection = false,
            TimeoutSeconds = 10,
            ContentType = "text/plain; charset=UTF-8",
            ScheduledEnqueueTimeUtc = DateTime.UtcNow,
            TimeToLiveSeconds = 30,
            CreateQueueOrTopicIfItDoesNotExist = true,
            AutoDeleteOnIdle = 4,
            TimeFormat = TimeFormat.Minutes,
            MaxSize = 1024,
            CorrelationId = null,
            SessionId = null,
            ReplyToSessionId = null,
            MessageId = null,
            ReplyTo = null,
            To = null
        };

        input = new Input()
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = "NewTestQueue",
            DestinationType = QueueOrTopic.Queue,
            SubscriptionName = null,
            Data = "test",
            Properties = Array.Empty<MessageProperty>()
        };

        var result = await ServiceBus.Send(input, options, default);
        Assert.IsTrue(result.Results.Any(x => x.MessageId != null));
    }

    /// <summary>
    /// Making sure that AutoDelete options works as expected. 61 minutes should be 1h 1min. Check must be handled via Azure portal while debuging.
    /// </summary>
    [TestMethod]
    public async Task Queue_CreateQueueOrTopicIfItDoesNotExist_True_TimeFormat_MinutesToHour_Test()
    {
        options = new Options()
        {
            BodySerializationType = BodySerializationType.String,
            UseCachedConnection = false,
            TimeoutSeconds = 10,
            ContentType = "text/plain; charset=UTF-8",
            ScheduledEnqueueTimeUtc = DateTime.UtcNow,
            TimeToLiveSeconds = 30,
            CreateQueueOrTopicIfItDoesNotExist = true,
            AutoDeleteOnIdle = 61,
            TimeFormat = TimeFormat.Minutes,
            MaxSize = 1024,
            CorrelationId = null,
            SessionId = null,
            ReplyToSessionId = null,
            MessageId = null,
            ReplyTo = null,
            To = null
        };

        input = new Input()
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = "NewTestQueue",
            DestinationType = QueueOrTopic.Queue,
            SubscriptionName = null,
            Data = "test",
            Properties = Array.Empty<MessageProperty>()
        };

        var result = await ServiceBus.Send(input, options, default);
        Assert.IsTrue(result.Results.Any(x => x.MessageId != null));
    }

    /// <summary>
    /// Making sure that AutoDelete options works as expected. Should be 5 hours. Check must be handled via Azure portal while debuging.
    /// </summary>
    [TestMethod]
    public async Task Queue_CreateQueueOrTopicIfItDoesNotExist_True_TimeFormat_Hours_Test()
    {
        options = new Options()
        {
            BodySerializationType = BodySerializationType.String,
            UseCachedConnection = false,
            TimeoutSeconds = 10,
            ContentType = "text/plain; charset=UTF-8",
            ScheduledEnqueueTimeUtc = DateTime.UtcNow,
            TimeToLiveSeconds = 30,
            CreateQueueOrTopicIfItDoesNotExist = true,
            AutoDeleteOnIdle = 5,
            TimeFormat = TimeFormat.Hours,
            MaxSize = 1024,
            CorrelationId = null,
            SessionId = null,
            ReplyToSessionId = null,
            MessageId = null,
            ReplyTo = null,
            To = null
        };

        input = new Input()
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = "NewTestQueue",
            DestinationType = QueueOrTopic.Queue,
            SubscriptionName = null,
            Data = "test",
            Properties = Array.Empty<MessageProperty>()
        };

        var result = await ServiceBus.Send(input, options, default);
        Assert.IsTrue(result.Results.Any(x => x.MessageId != null));
    }

    /// <summary>
    /// Making sure that AutoDelete options works as expected. Should be 5 days. Check must be handled via Azure portal while debuging.
    /// </summary>
    [TestMethod]
    public async Task Queue_CreateQueueOrTopicIfItDoesNotExist_True_TimeFormat_Days_Test()
    {
        options = new Options()
        {
            BodySerializationType = BodySerializationType.String,
            UseCachedConnection = false,
            TimeoutSeconds = 10,
            ContentType = "text/plain; charset=UTF-8",
            ScheduledEnqueueTimeUtc = DateTime.UtcNow,
            TimeToLiveSeconds = 30,
            CreateQueueOrTopicIfItDoesNotExist = true,
            AutoDeleteOnIdle = 5,
            TimeFormat = TimeFormat.Days,
            MaxSize = 1024,
            CorrelationId = null,
            SessionId = null,
            ReplyToSessionId = null,
            MessageId = null,
            ReplyTo = null,
            To = null
        };

        input = new Input()
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = "NewTestQueue",
            DestinationType = QueueOrTopic.Queue,
            SubscriptionName = null,
            Data = "test",
            Properties = Array.Empty<MessageProperty>()
        };

        var result = await ServiceBus.Send(input, options, default);
        Assert.IsTrue(result.Results.Any(x => x.MessageId != null));
    }

    /// <summary>
    /// Making sure that AutoDelete options works as expected. Should be never. Check must be handled via Azure portal while debuging.
    /// AutoDeleteOnIdle=0 = Never
    /// </summary>
    [TestMethod]
    public async Task Queue_CreateQueueOrTopicIfItDoesNotExist_True_TimeFormat_Never_Test()
    {
        options = new Options()
        {
            BodySerializationType = BodySerializationType.String,
            UseCachedConnection = false,
            TimeoutSeconds = 10,
            ContentType = "text/plain; charset=UTF-8",
            ScheduledEnqueueTimeUtc = DateTime.UtcNow,
            TimeToLiveSeconds = 30,
            CreateQueueOrTopicIfItDoesNotExist = true,
            AutoDeleteOnIdle = 0,
            TimeFormat = TimeFormat.Minutes,
            MaxSize = 1024,
            CorrelationId = null,
            SessionId = null,
            ReplyToSessionId = null,
            MessageId = null,
            ReplyTo = null,
            To = null
        };

        input = new Input()
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = "NewTestQueue",
            DestinationType = QueueOrTopic.Queue,
            SubscriptionName = null,
            Data = "test",
            Properties = Array.Empty<MessageProperty>()
        };

        var result = await ServiceBus.Send(input, options, default);
        Assert.IsTrue(result.Results.Any(x => x.MessageId != null));
    }

    /// <summary>
    /// ScheduledEnqueueTimeUtc +1 days. Check must be handled via Azure portal while debuging.
    /// </summary>
    [TestMethod]
    public async Task Queue_CreateQueueOrTopicIfItDoesNotExist_True_TimeFormat_Never_Delayed_Test()
    {
        options = new Options()
        {
            BodySerializationType = BodySerializationType.String,
            UseCachedConnection = false,
            TimeoutSeconds = 10,
            ContentType = "text/plain; charset=UTF-8",
            ScheduledEnqueueTimeUtc = DateTime.UtcNow.AddDays(1),
            TimeToLiveSeconds = 30,
            CreateQueueOrTopicIfItDoesNotExist = true,
            AutoDeleteOnIdle = 0,
            TimeFormat = TimeFormat.Minutes,
            MaxSize = 1024,
            CorrelationId = null,
            SessionId = null,
            ReplyToSessionId = null,
            MessageId = null,
            ReplyTo = null,
            To = null
        };

        input = new Input()
        {
            ConnectionString = _connectionString,
            QueueOrTopicName = "NewTestQueue",
            DestinationType = QueueOrTopic.Queue,
            SubscriptionName = null,
            Data = "test",
            Properties = Array.Empty<MessageProperty>()
        };

        var result = await ServiceBus.Send(input, options, default);
        Assert.IsTrue(result.Results.Any(x => x.MessageId != null));
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

        foreach (var topicName in topicNames)
        {
            if (await managementClient.TopicExistsAsync(topicName).ConfigureAwait(false))
                await managementClient.DeleteTopicAsync(topicName).ConfigureAwait(false);
        }
    }
}