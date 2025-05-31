using System.Collections;

namespace SharperExtensions.Core.Tests;

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
            .Monad
            .Bind(result, binder)
            .Should()
            .BeEquivalentTo(expected, config => config.PreferringRuntimeMemberTypes());
    }

    [Theory]
    [ClassData(typeof(FlattenTestCases))]
    public void FlattenTests(
        Result<Result<DummyValue, DummyError>, DummyError> wrappedResult,
        Result<DummyValue, DummyError> expected
    )
    {
        Result
            .Monad
            .Flatten(wrappedResult)
            .Should()
            .BeEquivalentTo(expected, config => config.PreferringRuntimeMemberTypes());
    }
}

file class BindTestCases : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            Result.Ok<DummyValue, DummyError>(ResultTestData.Value),
            (Func<DummyValue, Result<DummyNewValue, DummyError>>)ResultTestMethods
                .TestBinder,
            Result.Ok<DummyNewValue, DummyError>(ResultTestData.NewValue),
        ];

        yield return
        [
            Result.Error<DummyValue, DummyError>(ResultTestData.Error),
            (Func<DummyValue, Result<DummyNewValue, DummyError>>)ResultTestMethods
                .TestBinder,
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
            Result.Error<Result<DummyValue, DummyError>, DummyError>(
                ResultTestData.Error
            ),
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
            () => Task.FromResult(
                Result.Ok<DummyValue, DummyError>(ResultTestData.Value)
            ),
            (Func<DummyValue, Task<Result<DummyNewValue, DummyError>>>)
            ResultTestMethods.AsyncTestBinder,
            Result.Ok<DummyNewValue, DummyError>(ResultTestData.NewValue),
        ];

        yield return
        [
            () => Task.FromResult(
                Result.Error<DummyValue, DummyError>(ResultTestData.Error)
            ),
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
                        Task.FromResult(
                            Result.Ok<DummyValue, DummyError>(ResultTestData.Value)
                        )
                    )
                ),
            Result.Ok<DummyValue, DummyError>(ResultTestData.Value),
        ];

        yield return
        [
            () =>
                Task.FromResult(
                    Result.Ok<Task<Result<DummyValue, DummyError>>, DummyError>(
                        Task.FromResult(
                            Result.Error<DummyValue, DummyError>(ResultTestData.Error)
                        )
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