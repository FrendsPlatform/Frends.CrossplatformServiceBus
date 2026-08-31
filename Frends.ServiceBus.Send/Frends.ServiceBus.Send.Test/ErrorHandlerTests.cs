using Frends.ServiceBus.Send.Definitions;
using Microsoft.Azure.ServiceBus;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.ServiceBus.Send.Test;

[TestClass]
public class ErrorHandlerTests
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    private static Input InvalidInput()
    {
        return new Input
        {
            ConnectionString = "Endpoint=sb://invalid.servicebus.windows.net/;SharedAccessKeyName=Invalid;SharedAccessKey=InvalidKey",
            QueueOrTopicName = "nonexistent-queue",
            DestinationType = QueueOrTopic.Queue,
            SubscriptionName = null,
            Data = "test",
            Properties = Array.Empty<MessageProperty>(),
        };
    }

    private static Options DefaultOptions()
    {
        return new Options
        {
            BodySerializationType = BodySerializationType.String,
            UseCachedConnection = false,
            TimeoutSeconds = 5,
            ContentType = "text/plain; charset=UTF-8",
            ScheduledEnqueueTimeUtc = DateTime.UtcNow,
            TimeToLiveSeconds = 30,
            CreateQueueOrTopicIfItDoesNotExist = false,
            AutoDeleteOnIdle = 5,
            TimeFormat = TimeFormat.Minutes,
            MaxSize = 1024,
        };
    }

    [TestMethod]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = true;

        var ex = Assert.ThrowsExceptionAsync<ServiceBusCommunicationException>(async () =>
            await ServiceBus.Send(InvalidInput(), options, CancellationToken.None)).Result;
        Assert.IsNotNull(ex);
    }

    [TestMethod]
    public async Task Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = false;

        var result = await ServiceBus.Send(InvalidInput(), options, CancellationToken.None);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsFalse(string.IsNullOrEmpty(result.Error.Message));
    }

    [TestMethod]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = true;
        options.ErrorMessageOnFailure = CustomErrorMessage;

        var ex = Assert.ThrowsExceptionAsync<Exception>(async () =>
            await ServiceBus.Send(InvalidInput(), options, CancellationToken.None)).Result;
        Assert.IsNotNull(ex);
        Assert.IsTrue(ex.Message.Contains(CustomErrorMessage));
    }
}
