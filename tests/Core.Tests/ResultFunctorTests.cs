using System.Collections;

namespace SharperExtensions.Core.Tests;

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
        Result.Functor
            .Map(result, mapper)
            .Should()
            .Satisfy<Result<DummyNewValue, DummyError>>(r =>
                r.Value.Should().BeEquivalentTo(expected.Value)
            )
            .And
            .Satisfy<Result<DummyNewValue, DummyError>>(r =>
                r.ErrorValue.Should().BeEquivalentTo(expected.ErrorValue)
            );
    }

    [Theory]
    [ClassData(typeof(MapErrorTestCases))]
    public static void MapErrorTests(
        Result<DummyValue, DummyError> result,
        Func<DummyError, DummyNewError> mapper,
        Result<DummyValue, DummyNewError> expected
    )
    {
        Result.Functor
            .MapError(result, mapper)
            .Should()
            .Satisfy<Result<DummyValue, DummyNewError>>(r =>
                r.Value.Should().BeEquivalentTo(expected.Value)
            )
            .And
            .Satisfy<Result<DummyValue, DummyNewError>>(r =>
                r.ErrorValue.Should().BeEquivalentTo(expected.ErrorValue)
            );
    }

    [Theory]
    [ClassData(typeof(MatchTestCases))]
    public static void MatchTests(
        Result<DummyValue, DummyError> result,
        Func<DummyValue, DummyNewValue> okMapper,
        Func<DummyError, DummyNewValue> errorMapper,
        DummyNewValue expected
    )
    {
        Result.Functor
            .Match(result, okMapper, errorMapper)
            .Should()
            .BeEquivalentTo(expected, config => config.PreferringRuntimeMemberTypes());
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
            Result.Ok<DummyNewValue, DummyError>(ResultTestData.NewValue),
        ];

        yield return
        [
            Result.Error<DummyValue, DummyError>(ResultTestData.Error),
            (Func<DummyValue, DummyNewValue>)ResultTestMethods.TestMapping,
            Result.Error<DummyNewValue, DummyError>(ResultTestData.Error),
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
            Result.Error<DummyValue, DummyNewError>(ResultTestData.NewError),
        ];

        yield return
        [
            Result.Ok<DummyValue, DummyError>(ResultTestData.Value),
            (Func<DummyError, DummyNewError>)ResultTestMethods.TestErrorMapping,
            Result.Ok<DummyValue, DummyNewError>(ResultTestData.Value),
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
            (Func<DummyValue, DummyNewValue>)ResultTestMethods.MatchOk,
            (Func<DummyError, DummyNewValue>)ResultTestMethods.MatchError,
            ResultTestData.NewValue,
        ];

        yield return
        [
            Result.Error<DummyValue, DummyError>(ResultTestData.Error),
            (Func<DummyValue, DummyNewValue>)ResultTestMethods.MatchOk,
            (Func<DummyError, DummyNewValue>)ResultTestMethods.MatchError,
            new DummyNewValue { NameAllCaps = "ERROR" },
        ];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
