using System.Collections;
using FluentAssertions;
using NetFunction.Types.Tests.DummyTypes;
using NetFunctional.Types;
using Xunit;

namespace NetFunction.Types.Tests;

public class ResultFunctorTests
{
    [Theory]
    [ClassData(typeof(MapTestCases))]
    public void MapTests(
        Result<DummyValue, DummyError> result,
        Func<DummyValue, DummyNewValue> mapper,
        Result<DummyNewValue, DummyError> expected
    )
    {
        Result
            .Functor
            .Map(result, mapper)
            .Should()
            .BeEquivalentTo(expected, config => config.RespectingRuntimeTypes());
    }

    [Theory]
    [ClassData(typeof(MapErrorTestCases))]
    public static void MapErrorTests(
        Result<DummyValue, DummyError> result,
        Func<DummyError, DummyNewError> mapper,
        Result<DummyValue, DummyNewError> expected
    )
    {
        Result
            .Functor
            .MapError(result, mapper)
            .Should()
            .BeEquivalentTo(expected, config => config.RespectingRuntimeTypes());
    }

    [Theory]
    [ClassData(typeof(MatchTestCases))]
    public static void MatchTests(
        Result<DummyValue, DummyError> result,
        Func<DummyValue, DummyNewValue> okMapper,
        Func<DummyError, DummyNewError> errorMapper,
        Result<DummyNewValue, DummyNewError> expected
    )
    {
        Result
            .Functor
            .Match(result, okMapper, errorMapper)
            .Should()
            .BeEquivalentTo(expected, config => config.RespectingRuntimeTypes());
    }

    [Theory]
    [ClassData(typeof(MapAsyncTestCases))]
    public async Task MapAsyncTests(
        Func<Task<Result<DummyValue, DummyError>>> resultTaskFactory,
        Func<DummyValue, DummyNewValue> mapper,
        Result<DummyNewValue, DummyError> expected
    )
    {
        (await Result.Functor.MapAsync(resultTaskFactory(), mapper))
            .Should()
            .BeEquivalentTo(expected, config => config.RespectingRuntimeTypes());
    }

    [Theory]
    [ClassData(typeof(MapErrorAsyncTestCases))]
    public static async Task MapErrorAsyncTests(
        Func<Task<Result<DummyValue, DummyError>>> resultTaskFactory,
        Func<DummyError, DummyNewError> mapper,
        Result<DummyValue, DummyNewError> expected
    )
    {
        (await Result.Functor.MapErrorAsync(resultTaskFactory(), mapper))
            .Should()
            .BeEquivalentTo(expected, config => config.RespectingRuntimeTypes());
    }

    [Theory]
    [ClassData(typeof(MatchAsyncTestCases))]
    public static async Task MatchAsyncTests(
        Func<Task<Result<DummyValue, DummyError>>> resultTaskFactory,
        Func<DummyValue, DummyNewValue> okMapper,
        Func<DummyError, DummyNewError> errorMapper,
        Result<DummyNewValue, DummyNewError> expected
    )
    {
        (await Result.Functor.MatchAsync(resultTaskFactory(), okMapper, errorMapper))
            .Should()
            .BeEquivalentTo(expected, config => config.RespectingRuntimeTypes());
    }
}

file class MapTestCases : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            Result.Ok<DummyValue, DummyError>(ResultTestData.Value),
            (Func<DummyValue, DummyNewValue>)ResultTestMethods.TestMapping,
            Result.Ok<DummyNewValue, DummyError>(ResultTestData.NewValue)
        ];

        yield return
        [
            Result.Error<DummyValue, DummyError>(ResultTestData.Error),
            (Func<DummyValue, DummyNewValue>)ResultTestMethods.TestMapping,
            Result.Error<DummyNewValue, DummyError>(ResultTestData.Error)
        ];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

file class MapErrorTestCases : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            Result.Error<DummyValue, DummyError>(ResultTestData.Error),
            (Func<DummyError, DummyNewError>)ResultTestMethods.TestErrorMapping,
            Result.Error<DummyValue, DummyNewError>(ResultTestData.NewError)
        ];

        yield return
        [
            Result.Ok<DummyValue, DummyError>(ResultTestData.Value),
            (Func<DummyError, DummyNewError>)ResultTestMethods.TestErrorMapping,
            Result.Ok<DummyValue, DummyNewError>(ResultTestData.Value)
        ];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

file class MatchTestCases : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            Result.Ok<DummyValue, DummyError>(ResultTestData.Value),
            (Func<DummyValue, DummyNewValue>)ResultTestMethods.TestMapping,
            (Func<DummyError, DummyNewError>)ResultTestMethods.TestErrorMapping,
            Result.Ok<DummyNewValue, DummyNewError>(ResultTestData.NewValue)
        ];

        yield return
        [
            Result.Error<DummyValue, DummyError>(ResultTestData.Error),
            (Func<DummyValue, DummyNewValue>)ResultTestMethods.TestMapping,
            (Func<DummyError, DummyNewError>)ResultTestMethods.TestErrorMapping,
            Result.Error<DummyNewValue, DummyNewError>(ResultTestData.NewError)
        ];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

file class MapAsyncTestCases : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            () => Task.FromResult(Result.Ok<DummyValue, DummyError>(ResultTestData.Value)),
            (Func<DummyValue, DummyNewValue>)ResultTestMethods.TestMapping,
            Result.Ok<DummyNewValue, DummyError>(ResultTestData.NewValue)
        ];

        yield return
        [
            () => Task.FromResult(Result.Error<DummyValue, DummyError>(ResultTestData.Error)),
            (Func<DummyValue, DummyNewValue>)ResultTestMethods.TestMapping,
            Result.Error<DummyNewValue, DummyError>(ResultTestData.Error)
        ];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

file class MapErrorAsyncTestCases : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            () => Task.FromResult(Result.Error<DummyValue, DummyError>(ResultTestData.Error)),
            (Func<DummyError, DummyNewError>)ResultTestMethods.TestErrorMapping,
            Result.Error<DummyValue, DummyNewError>(ResultTestData.NewError)
        ];

        yield return
        [
            () => Task.FromResult(Result.Ok<DummyValue, DummyError>(ResultTestData.Value)),
            (Func<DummyError, DummyNewError>)ResultTestMethods.TestErrorMapping,
            Result.Ok<DummyValue, DummyNewError>(ResultTestData.Value)
        ];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

file class MatchAsyncTestCases : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return
        [
            () => Task.FromResult(Result.Ok<DummyValue, DummyError>(ResultTestData.Value)),
            (Func<DummyValue, DummyNewValue>)ResultTestMethods.TestMapping,
            (Func<DummyError, DummyNewError>)ResultTestMethods.TestErrorMapping,
            Result.Ok<DummyNewValue, DummyNewError>(ResultTestData.NewValue)
        ];

        yield return
        [
            () => Task.FromResult(Result.Error<DummyValue, DummyError>(ResultTestData.Error)),
            (Func<DummyValue, DummyNewValue>)ResultTestMethods.TestMapping,
            (Func<DummyError, DummyNewError>)ResultTestMethods.TestErrorMapping,
            Result.Error<DummyNewValue, DummyNewError>(ResultTestData.NewError)
        ];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
