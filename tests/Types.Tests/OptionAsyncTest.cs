using DotNetCoreFunctional.Option;
using DotNetCoreFunctional.Result;
using FluentAssertions;
using NetFunction.Types.Tests.DummyTypes;
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

    [Fact]
    public async Task TestValueOrAsync_SomeValueTask()
    {
        var valueTaskOption = ValueTask.FromResult<Option<DummyValue>>(OptionTestData.SomeValue);
        var fallbackValue = new DummyValue { Email = "Jim.Slime@example.com", Name = "Jim Slim" };
        (await Option.Async.ValueOrAsync(valueTaskOption, fallbackValue))
            .Should()
            .Be(OptionTestData.Value);
    }
    
    
    [Fact]
    public async Task TestValueOrAsync_NoneValueTask()
    {
        var valueTaskOption = ValueTask.FromResult<Option<DummyValue>>(OptionTestData.NoneValue);
        var fallbackValue = new DummyValue { Email = "Jim.Slime@example.com", Name = "Jim Slim" };
        (await Option.Async.ValueOrAsync(valueTaskOption, fallbackValue))
            .Should()
            .Be(fallbackValue);
    }
    
    
    [Fact]
    public async Task TestValueOrAsync_SomeTask()
    {
        var valueTaskOption = Task.FromResult<Option<DummyValue>>(OptionTestData.SomeValue);
        var fallbackValue = new DummyValue { Email = "Jim.Slime@example.com", Name = "Jim Slim" };
        (await Option.Async.ValueOrAsync(valueTaskOption, fallbackValue))
            .Should()
            .Be(OptionTestData.Value);
    }
    
    
    [Fact]
    public async Task TestValueOrAsync_NoneTask()
    {
        var valueTaskOption = Task.FromResult<Option<DummyValue>>(OptionTestData.NoneValue);
        var fallbackValue = new DummyValue { Email = "Jim.Slime@example.com", Name = "Jim Slim" };
        (await Option.Async.ValueOrAsync(valueTaskOption, fallbackValue))
            .Should()
            .Be(fallbackValue);
    }
}