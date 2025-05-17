using DotNetCoreFunctional.Async;
using DotNetCoreFunctional.Result;
using FluentAssertions;
using NetFunction.Types.Tests.DummyTypes;

namespace NetFunction.Types.Tests;

public class AsyncCatchTests
{
    [Fact]
    public void CreateResult_ShouldReturnErrorString()
    {
        const string testMessage = "This is a test exception.";
        var asyncCatch = new AsyncCatch<DummyValue>(new InvalidOperationException(testMessage));

        var result = asyncCatch.CreateResult(exc => exc.Message);

        result.TryGetError(out var message).Should().BeTrue();
        message.Should().Be(message);
    }

    [Fact]
    public void AsAsyncResult_ResultCallbackShouldNotBeNull()
    {
        var asyncCatch = new AsyncCatch<DummyValue>(ResultTestData.Value);

        var asyncResult = asyncCatch.AsAsyncResult();

        asyncResult.WrappedResult.State.ResultCallback.Should().NotBeNull();
    }

    [Fact]
    public void AsAsyncResult_AsyncFunctionReturnsResult_ResultCallbackShouldNotBeNull()
    {
        var asyncCatch = ReturnValueAsync();
        var asyncResult = asyncCatch.AsAsyncResult();

        asyncResult.WrappedResult.State.ResultCallback.Should().NotBeNull();

        return;

        async AsyncCatch<DummyValue> ReturnValueAsync()
        {
            await Task.Delay(2000);

            return ResultTestData.DefaultValue;
        }
    }

    [Fact]
    public void AsAsyncResult_AsyncFunctionThrowsException_ResultCallbackShouldNotBeNull()
    {
        var asyncCatch = ReturnValueAsync();
        var asyncResult = asyncCatch.AsAsyncResult();

        asyncResult.WrappedResult.State.ResultCallback.Should().NotBeNull();

        return;

        async AsyncCatch<DummyValue> ReturnValueAsync()
        {
            await Task.Delay(2000);

            throw new InvalidOperationException();
        }
    }

    [Fact]
    public async Task CreateAsyncResult_ShouldReturnErrorString()
    {
        const string testMessage = "This is a test exception.";
        var asyncCatch = new AsyncCatch<DummyValue>(new InvalidOperationException(testMessage));

        var result = asyncCatch.CreateAsyncResult(exc => exc.Message);

        (await result.AsTask()).TryGetError(out var message).Should().BeTrue();
        message.Should().Be(testMessage);
    }

    [Fact]
    public async Task Await_ReturnsOk()
    {
        const string testString = "This is a test.";

        var asyncCatch = CreateAsyncCatch();
        var result = await asyncCatch;

        result.TryGetValue(out var response).Should().BeTrue();
        response.Should().Be(testString);

        return;

        static async AsyncCatch<string> CreateAsyncCatch() => await TaskFactory();

        static async Task<string> TaskFactory()
        {
            await Task.Delay(1000);
            return testString;
        }
    }

    [Fact]
    public async Task Await_ReturnsError()
    {
        const string testString = "This is a test exception.";

        var result = await CreateAsyncCatch();

        result.TryGetError(out var exception).Should().BeTrue();
        exception!.Message.Should().Be(testString);

        return;

        static async AsyncCatch<string> CreateAsyncCatch() => await TaskFactory();

        static async Task<string> TaskFactory()
        {
            await Task.Delay(1000);
            throw new InvalidOperationException(testString);
        }
    }
}
