using DotNetCoreFunctional.Result;
using FluentAssertions;
using NetFunction.Types.Tests.DummyTypes;
using Xunit;

namespace NetFunction.Types.Tests;

public class ResultAsync
{
    [Fact]
    public async Task TestAwaitIfOk_ValueTask()
    {
        var expected = Result<DummyValue, DummyError>.Ok(ResultTestData.Value);
        var valueTasKResult = Result<ValueTask<DummyValue>, DummyError>.Ok(
            ValueTask.FromResult(ResultTestData.Value)
        );

        (await Result.Async.AwaitIfOk(valueTasKResult))
            .Should()
            .BeEquivalentTo(expected, config => config.RespectingRuntimeTypes());
    }

    [Fact]
    public async Task TestAwaitIfOk_Task()
    {
        var expected = Result<DummyValue, DummyError>.Ok(ResultTestData.Value);
        var valueTasKResult = Result<Task<DummyValue>, DummyError>.Ok(
            Task.FromResult(ResultTestData.Value)
        );

        (await Result.Async.AwaitIfOk(valueTasKResult))
            .Should()
            .BeEquivalentTo(expected, config => config.RespectingRuntimeTypes());
    }

    [Fact]
    public async Task TestAwaitIfError_ValueTask()
    {
        var expected = Result<DummyValue, DummyError>.Error(ResultTestData.Error);
        var valueTasKResult = Result<DummyValue, ValueTask<DummyError>>.Error(
            ValueTask.FromResult(ResultTestData.Error)
        );

        (await Result.Async.AwaitIfError(valueTasKResult))
            .Should()
            .BeEquivalentTo(expected, config => config.RespectingRuntimeTypes());
    }

    [Fact]
    public async Task TestAwaitIfError_Task()
    {
        var expected = Result<DummyValue, DummyError>.Error(ResultTestData.Error);
        var valueTasKResult = Result<DummyValue, Task<DummyError>>.Error(
            Task.FromResult(ResultTestData.Error)
        );

        (await Result.Async.AwaitIfError(valueTasKResult))
            .Should()
            .BeEquivalentTo(expected, config => config.RespectingRuntimeTypes());
    }
}
