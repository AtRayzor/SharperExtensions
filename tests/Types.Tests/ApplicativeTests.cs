using System.Collections;
using DotNetCoreFunctional.Result;
using FluentAssertions;
using NetFunction.Types.Tests.DummyTypes;

namespace NetFunction.Types.Tests;

public class ApplicativeTests
{
    [Theory]
    [ClassData(typeof(ApplyTestsCases))]
    public void ApplyTests(
        Result<DummyValue, DummyError> result,
        Result<Func<DummyValue, DummyNewValue>, DummyError> wrappedMapping,
        Result<DummyNewValue, DummyError> expected
    )
    {
        Result
            .Applicative.Apply(result, wrappedMapping)
            .Should()
            .BeEquivalentTo(expected, config => config.RespectingRuntimeTypes());
    }
}

file class ApplyTestsCases : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            Result.Ok<DummyValue, DummyError>(ResultTestData.Value),
            Result.Ok<Func<DummyValue, DummyNewValue>, DummyError>(ResultTestMethods.TestMapping),
            Result.Ok<DummyNewValue, DummyError>(ResultTestData.NewValue),
        ];

        yield return
        [
            Result.Ok<DummyValue, DummyError>(ResultTestData.Value),
            Result.Error<Func<DummyValue, DummyNewValue>, DummyError>(ResultTestData.Error),
            Result.Error<DummyNewValue, DummyError>(ResultTestData.Error),
        ];

        yield return
        [
            Result.Error<DummyValue, DummyError>(ResultTestData.Error),
            Result.Ok<Func<DummyValue, DummyNewValue>, DummyError>(ResultTestMethods.TestMapping),
            Result.Error<DummyNewValue, DummyError>(ResultTestData.Error),
        ];

        yield return
        [
            Result.Error<DummyValue, DummyError>(ResultTestData.Error),
            Result.Error<Func<DummyValue, DummyNewValue>, DummyError>(ResultTestData.Error),
            Result.Error<DummyNewValue, DummyError>(ResultTestData.Error),
        ];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

file class ApplyAsyncTestsCases : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            () => Task.FromResult(Result.Ok<DummyValue, DummyError>(ResultTestData.Value)),
            () =>
                Task.FromResult(
                    Result.Ok<Func<DummyValue, DummyNewValue>, DummyError>(
                        ResultTestMethods.TestMapping
                    )
                ),
            Result.Ok<DummyNewValue, DummyError>(ResultTestData.NewValue),
        ];

        yield return
        [
            () => Task.FromResult(Result.Ok<DummyValue, DummyError>(ResultTestData.Value)),
            () =>
                Task.FromResult(
                    Result.Error<Func<DummyValue, DummyNewValue>, DummyError>(ResultTestData.Error)
                ),
            Result.Error<DummyNewValue, DummyError>(ResultTestData.Error),
        ];

        yield return
        [
            () => Task.FromResult(Result.Error<DummyValue, DummyError>(ResultTestData.Error)),
            () =>
                Task.FromResult(
                    Result.Ok<Func<DummyValue, DummyNewValue>, DummyError>(
                        ResultTestMethods.TestMapping
                    )
                ),
            Result.Error<DummyNewValue, DummyError>(ResultTestData.Error),
        ];

        yield return
        [
            () => Task.FromResult(Result.Error<DummyValue, DummyError>(ResultTestData.Error)),
            () =>
                Task.FromResult(
                    Result.Error<Func<DummyValue, DummyNewValue>, DummyError>(ResultTestData.Error)
                ),
            Result.Error<DummyNewValue, DummyError>(ResultTestData.Error),
        ];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
