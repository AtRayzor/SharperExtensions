using System;
using DotNetCoreFunctional.Result;
using FluentAssertions;
using NetFunction.Types.Tests;
using NetFunction.Types.Tests.DummyTypes;
using Xunit;

namespace NetFunction.Types.Tests;

public class ResultTests
{
    [Fact]
    public void Ok_CreatesOkResult()
    {
        var ok = Result.Ok<DummyValue, DummyError>(ResultTestData.Value);

        ok.Should().BeOfType<Ok<DummyValue, DummyError>>();
        ok.As<Ok<DummyValue, DummyError>>().Value.Should().Be(ResultTestData.Value);
    }

    [Fact]
    public void Error_CreatesErrorResult()
    {
        var error = Result.Error<DummyValue, DummyError>(ResultTestData.Error);

        error.Should().BeOfType<Error<DummyValue, DummyError>>();
        error.As<Error<DummyValue, DummyError>>().Err.Should().Be(ResultTestData.Error);
    }

    [Fact]
    public void IsOk_ReturnsTrueForOk()
    {
        var ok = Result.Ok<DummyValue, DummyError>(ResultTestData.Value);

        Result.IsOk(ok).Should().BeTrue();
        Result.IsError(ok).Should().BeFalse();
    }

    [Fact]
    public void IsError_ReturnsTrueForError()
    {
        var error = Result.Error<DummyValue, DummyError>(ResultTestData.Error);

        Result.IsError(error).Should().BeTrue();
        Result.IsOk(error).Should().BeFalse();
    }

    [Fact]
    public void Unsafe_GetValueOrDefault_ReturnsValueOrDefault()
    {
        var ok = Result.Ok<DummyValue, DummyError>(ResultTestData.Value);
        var error = Result.Error<DummyValue, DummyError>(ResultTestData.Error);

        Result.Unsafe.GetValueOrDefault(ok).Should().Be(ResultTestData.Value);
        Result.Unsafe.GetValueOrDefault(error).Should().BeNull();

        Result.Unsafe.GetValueOrDefault(ok, ResultTestData.DefaultValue).Should().Be(ResultTestData.Value);
        Result.Unsafe.GetValueOrDefault(error, ResultTestData.DefaultValue).Should().Be(ResultTestData.DefaultValue);
    }

    [Fact]
    public void Unsafe_GetErrorOrDefault_ReturnsErrorOrDefault()
    {
        var ok = Result.Ok<DummyValue, DummyError>(ResultTestData.Value);
        var error = Result.Error<DummyValue, DummyError>(ResultTestData.Error);

        Result.Unsafe.GetErrorOrDefault(error).Should().Be(ResultTestData.Error);
        Result.Unsafe.GetErrorOrDefault(ok).Should().BeNull();

        Result.Unsafe.GetErrorOrDefault(error, ResultTestData.DefaultError).Should().Be(ResultTestData.Error);
        Result.Unsafe.GetErrorOrDefault(ok, ResultTestData.DefaultError).Should().Be(ResultTestData.DefaultError);
    }

    [Fact]
    public void Unsafe_TryGetValue_ReturnsTrueAndValueForOk()
    {
        var ok = Result.Ok<DummyValue, DummyError>(ResultTestData.Value);
        var error = Result.Error<DummyValue, DummyError>(ResultTestData.Error);

        var resultOk = Result.Unsafe.TryGetValue(ok, out var value);
        resultOk.Should().BeTrue();
        value.Should().Be(ResultTestData.Value);

        var resultError = Result.Unsafe.TryGetValue(error, out var valueError);
        resultError.Should().BeFalse();
        valueError.Should().BeNull();
    }

    [Fact]
    public void Unsafe_TryGetError_ReturnsTrueAndErrorForError()
    {
        var ok = Result.Ok<DummyValue, DummyError>(ResultTestData.Value);
        var error = Result.Error<DummyValue, DummyError>(ResultTestData.Error);

        var resultError = Result.Unsafe.TryGetError(error, out var err);
        resultError.Should().BeTrue();
        err.Should().Be(ResultTestData.Error);

        var resultOk = Result.Unsafe.TryGetError(ok, out var errOk);
        resultOk.Should().BeFalse();
        errOk.Should().BeNull();
    }

    [Fact]
    public void Unsafe_DoIfOk_ExecutesActionOnlyForOk()
    {
        var ok = Result.Ok<DummyValue, DummyError>(ResultTestData.Value);
        var error = Result.Error<DummyValue, DummyError>(ResultTestData.Error);

        DummyValue? capturedValue = null;
        Result.Unsafe.DoIfOk(ok, v => capturedValue = v);
        capturedValue.Should().Be(ResultTestData.Value);

        capturedValue = null;
        Result.Unsafe.DoIfOk(error, v => capturedValue = v);
        capturedValue.Should().BeNull();
    }

    [Fact]
    public void Unsafe_DoIfError_ExecutesActionOnlyForError()
    {
        var ok = Result.Ok<DummyValue, DummyError>(ResultTestData.Value);
        var error = Result.Error<DummyValue, DummyError>(ResultTestData.Error);

        DummyError? capturedError = null;
        Result.Unsafe.DoIfError(error, e => capturedError = e);
        capturedError.Should().Be(ResultTestData.Error);

        capturedError = null;
        Result.Unsafe.DoIfError(ok, e => capturedError = e);
        capturedError.Should().BeNull();
    }

    [Fact]
    public void Unsafe_Do_ExecutesCorrectActionBasedOnResult()
    {
        var ok = Result.Ok<DummyValue, DummyError>(ResultTestData.Value);
        var error = Result.Error<DummyValue, DummyError>(ResultTestData.Error);

        DummyValue? okValue = null;
        DummyError? errorValue = null;

        Result.Unsafe.Do(ok, v => okValue = v, e => errorValue = e);
        okValue.Should().Be(ResultTestData.Value);
        errorValue.Should().BeNull();

        okValue = null;
        errorValue = null;

        Result.Unsafe.Do(error, v => okValue = v, e => errorValue = e);
        okValue.Should().BeNull();
        errorValue.Should().Be(ResultTestData.Error);
    }
}
