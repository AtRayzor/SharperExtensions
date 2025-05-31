namespace SharperExtensions.Async.Tests;

public class AsyncMethodBuilderTests
{
    [Fact]
    public async Task TestAsyncMethod()
    {
        var value = await AsyncMethods.TestMethodAsync();
        value.Should().Be(1);
    }
}

file static class AsyncMethods
{
    public static async Async<int> TestMethodAsync()
    {
        return await Task.FromResult(1);
    }
}
