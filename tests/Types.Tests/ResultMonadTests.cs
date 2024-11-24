using System.Collections;
using DotNetCoreFunctional.Result;
using FluentAssertions;
using NetFunction.Types.Tests.DummyTypes;
using Xunit;

namespace NetFunction.Types.Tests;

public class MonadTests
{
    [Theory]
    [ClassData(typeof(BindTestCases))]
    public void BindTests(
        Result<DummyValue, DummyError> result,
        Func<DummyValue, Result<DummyNewValue, DummyError>> binder,
        Result<DummyNewValue, DummyError> expected
    )
    {
        Result
            .Monad.Bind(result, binder)
            .Should()
            .BeEquivalentTo(expected, config => config.RespectingRuntimeTypes());
    }

    [Theory]
    [ClassData(typeof(FlattenTestCases))]
    public void FlattenTests(
        Result<Result<DummyValue, DummyError>, DummyError> wrappedResult,
        Result<DummyValue, DummyError> expected
    )
    {
        Result
            .Monad.Flatten(wrappedResult)
            .Should()
            .BeEquivalentTo(expected, config => config.RespectingRuntimeTypes());
    }

    [Theory]
    [ClassData(typeof(BindAsyncTestCases))]
    public async Task BindAsyncTests(
        Func<Task<Result<DummyValue, DummyError>>> resultTaskFactory,
        Func<DummyValue, Task<Result<DummyNewValue, DummyError>>> asyncBinder,
        Result<DummyNewValue, DummyError> expected
    )
    {
        (await Result.Monad.BindAsync(resultTaskFactory(), asyncBinder))
            .Should()
            .BeEquivalentTo(expected, config => config.RespectingRuntimeTypes());
    }

    [Theory]
    [ClassData(typeof(FlattenAsyncTestCases))]
    public async Task FlattenAsyncTests(
        Func<
            Task<Result<Task<Result<DummyValue, DummyError>>, DummyError>>
        > wrappedResultTaskFactory,
        Result<DummyValue, DummyError> expected
    )
    {
        (await Result.Monad.FlattenAsync(wrappedResultTaskFactory()))
            .Should()
            .BeEquivalentTo(expected, config => config.RespectingRuntimeTypes());
    }
}

file class BindTestCases : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            Result.Ok<DummyValue, DummyError>(ResultTestData.Value),
            (Func<DummyValue, Result<DummyNewValue, DummyError>>)ResultTestMethods.TestBinder,
            Result.Ok<DummyNewValue, DummyError>(ResultTestData.NewValue),
        ];

        yield return
        [
            Result.Error<DummyValue, DummyError>(ResultTestData.Error),
            (Func<DummyValue, Result<DummyNewValue, DummyError>>)ResultTestMethods.TestBinder,
            Result.Error<DummyNewValue, DummyError>(ResultTestData.Error),
        ];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

file class FlattenTestCases : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            Result.Ok<Result<DummyValue, DummyError>, DummyError>(
                Result.Ok<DummyValue, DummyError>(ResultTestData.Value)
            ),
            Result.Ok<DummyValue, DummyError>(ResultTestData.Value),
        ];

        yield return
        [
            Result.Ok<Result<DummyValue, DummyError>, DummyError>(
                Result.Error<DummyValue, DummyError>(ResultTestData.Error)
            ),
            Result.Error<DummyValue, DummyError>(ResultTestData.Error),
        ];

        yield return
        [
            Result.Error<Result<DummyValue, DummyError>, DummyError>(ResultTestData.Error),
            Result.Error<DummyValue, DummyError>(ResultTestData.Error),
        ];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

file class BindAsyncTestCases : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            () => Task.FromResult(Result.Ok<DummyValue, DummyError>(ResultTestData.Value)),
            (Func<DummyValue, Task<Result<DummyNewValue, DummyError>>>)
                ResultTestMethods.AsyncTestBinder,
            Result.Ok<DummyNewValue, DummyError>(ResultTestData.NewValue),
        ];

        yield return
        [
            () => Task.FromResult(Result.Error<DummyValue, DummyError>(ResultTestData.Error)),
            (Func<DummyValue, Task<Result<DummyNewValue, DummyError>>>)
                ResultTestMethods.AsyncTestBinder,
            Result.Error<DummyNewValue, DummyError>(ResultTestData.Error),
        ];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

file class FlattenAsyncTestCases : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            () =>
                Task.FromResult(
                    Result.Ok<Task<Result<DummyValue, DummyError>>, DummyError>(
                        Task.FromResult(Result.Ok<DummyValue, DummyError>(ResultTestData.Value))
                    )
                ),
            Result.Ok<DummyValue, DummyError>(ResultTestData.Value),
        ];

        yield return
        [
            () =>
                Task.FromResult(
                    Result.Ok<Task<Result<DummyValue, DummyError>>, DummyError>(
                        Task.FromResult(Result.Error<DummyValue, DummyError>(ResultTestData.Error))
                    )
                ),
            Result.Error<DummyValue, DummyError>(ResultTestData.Error),
        ];

        yield return
        [
            () =>
                Task.FromResult(
                    Result.Error<Task<Result<DummyValue, DummyError>>, DummyError>(
                        ResultTestData.Error
                    )
                ),
            Result.Error<DummyValue, DummyError>(ResultTestData.Error),
        ];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
