using DotNetCoreFunctional.Option;
using DotNetCoreFunctional.Result;
using FluentAssertions;
using Xunit;

namespace NetFunction.Types.Tests;

public class OptionAsyncTest
{
    [Fact]
    public async Task TestAwaitIfSome_ValueTask()
    {
        var taskOption = Option.Some(ValueTask.FromResult(OptionTestData.Value));
        (await Option.Async.AwaitIfSome(taskOption))
            .Should()
            .BeEquivalentTo(
                OptionTestData.SomeValue,
                config => config.RespectingRuntimeTypes()
            );
    }
    
    [Fact]
    public async Task TestAwaitIfSome_Task()
    {
        var taskOption = Option.Some(Task.FromResult(OptionTestData.Value));
        (await Option.Async.AwaitIfSome(taskOption))
            .Should()
            .BeEquivalentTo(
                OptionTestData.SomeValue,
                config => config.RespectingRuntimeTypes()
            );
    }
}