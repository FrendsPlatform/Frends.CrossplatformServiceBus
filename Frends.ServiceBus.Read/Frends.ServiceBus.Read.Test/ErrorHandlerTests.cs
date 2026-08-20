using Frends.ServiceBus.Read.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.ServiceBus.Read.Test;

[TestClass]
public class ErrorHandlerTests
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    private static Input InvalidInput()
    {
        return new Input
        {
            ConnectionString = "Endpoint=sb://invalid.servicebus.windows.net/;SharedAccessKeyName=invalid;SharedAccessKey=invalid",
            QueueOrTopicName = "nonexistent-queue",
            SourceType = QueueOrTopic.Queue,
            SubscriptionName = null,
        };
    }

    private static Options DefaultOptions()
    {
        return new Options
        {
            BodySerializationType = BodySerializationType.String,
            DefaultEncoding = MessageEncoding.UTF8,
            CreateQueueOrTopicIfItDoesNotExist = false,
            UseCachedConnection = false,
            TimeoutSeconds = 5,
            AutoDeleteOnIdle = 5,
            TimeFormat = TimeFormat.Minutes,
            MaxSize = 1024,
            ThrowErrorOnFailure = true,
            ErrorMessageOnFailure = string.Empty,
        };
    }

    [TestMethod]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = true;

        var ex = Assert.ThrowsExceptionAsync<Exception>(async () =>
            await ServiceBus.Read(InvalidInput(), options, CancellationToken.None)).Result;
        Assert.IsNotNull(ex);
    }

    [TestMethod]
    public async Task Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = false;

        var result = await ServiceBus.Read(InvalidInput(), options, CancellationToken.None);
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
            await ServiceBus.Read(InvalidInput(), options, CancellationToken.None)).Result;
        Assert.IsNotNull(ex);
        Assert.IsTrue(ex.Message.Contains(CustomErrorMessage));
    }
}
