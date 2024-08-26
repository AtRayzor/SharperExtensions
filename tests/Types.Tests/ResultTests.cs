using System.Collections;
using FluentAssertions;
using NetFunction.Types.Tests.DummyTypes;
using NetFunctional.Core.UnionTypes;
using NetFunctional.Types;
using Xunit;

namespace NetFunction.Types.Tests;

public class ResultTests
{
    [Theory]
    [ClassData(typeof(IsOkTestCases))]
    public void TestIsOk(Result<DummyValue, DummyError> result, bool expected)
    {
        Result.IsOk(result).Should().Be(expected);
    }

    [Theory]
    [ClassData(typeof(IsErrorTestCases))]
    public void TestIsError(Result<DummyValue, DummyError> result, bool expected)
    {
        Result.IsError(result).Should().Be(expected);
    }

    [Theory]
    [ClassData(typeof(TryGetValueTestCases))]
    public void TestTryGetValue(Result<DummyValue, DummyError> result, bool expected, DummyValue? expectedValue)
    {
        Result.Unsafe.TryGetValue(result, out var valueOut).Should().Be(expected);
        valueOut.Should().Be(expectedValue);
    }

    [Theory]
    [ClassData(typeof(TryGetErrorTestCases))]
    public void TestTryGetError(Result<DummyValue, DummyError> result, bool expected, DummyError? expectedError)
    {
        Result.Unsafe.TryGetError(result, out var errorOut).Should().Be(expected);
        errorOut.Should().Be(expectedError);
    }

    [Fact]
    public void DoIfOk_OkResult()
    {
        string? testString = default;
        
        Result.Unsafe.DoIfOk(
            Result.Ok<DummyValue,DummyError>(ResultTestData.Value),
            value => { testString = value.Name; }
        );
        
        testString.Should().Be(ResultTestData.Value.Name);
    }
    
    [Fact]
    public void DoIfOk_ErrorResult()
    {
        string? testString = default;
        
        Result.Unsafe.DoIfOk(
            Result.Error<DummyValue,DummyError>(ResultTestData.Error),
            value => { testString = value.Name; }
        );
        
        testString.Should().BeNull();
    }
    
    [Fact]
    public void DoIfError_ErrorResult()
    {
        string? testString = default;
        
        Result.Unsafe.DoIfError(
            Result.Error<DummyValue,DummyError>(ResultTestData.Error),
            error => { testString = error.Message; }
        );
        
        testString.Should().Be(ResultTestData.Error.Message);
    }
    
    [Fact]
    public void DoIfError_OkResult()
    {
        string? testString = default;
        
        Result.Unsafe.DoIfError(
            Result.Ok<DummyValue,DummyError>(ResultTestData.Value),
            error => { testString = error.Message; }
        );
        
        testString.Should().BeNull();
    }
}

file class IsOkTestCases : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return [Result.Ok<DummyValue, DummyError>(ResultTestData.Value), true];
        yield return [Result.Error<DummyValue, DummyError>(ResultTestData.Error), false];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

file class IsErrorTestCases : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return [Result.Error<DummyValue, DummyError>(ResultTestData.Error), true];
        yield return [Result.Ok<DummyValue, DummyError>(ResultTestData.Value), false];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

file class TryGetValueTestCases : IEnumerable<object?[]>
{
    public IEnumerator<object?[]> GetEnumerator()
    {
        yield return [Result.Ok<DummyValue, DummyError>(ResultTestData.Value), true, ResultTestData.Value];
        yield return [Result.Error<DummyValue, DummyError>(ResultTestData.Error), false, null];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

file class TryGetErrorTestCases : IEnumerable<object?[]>
{
    public IEnumerator<object?[]> GetEnumerator()
    {
        yield return [Result.Error<DummyValue, DummyError>(ResultTestData.Error), true, ResultTestData.Error];
        yield return [Result.Ok<DummyValue, DummyError>(ResultTestData.Value), false, null];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

file class DoIfOkTestCases : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return [Result.Ok<DummyValue, DummyError>(ResultTestData.Value), ];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
