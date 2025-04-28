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
    public async Task CreateAsyncResult_ShouldReturnErrorString()
    {
        const string testMessage = "This is a test exception.";
        var asyncCatch = new AsyncCatch<DummyValue>(new InvalidOperationException(testMessage));

        var result = asyncCatch.CreateAsyncResult(exc => exc.Message);

        (await result.AsTask()).TryGetError(out var message).Should().BeTrue();
        message.Should().Be(testMessage);
    }
}
