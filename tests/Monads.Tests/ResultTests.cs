using FluentAssertions;
using Monads.ResultMonad;
using Monads.Tests.DummyTypes;

namespace Monads.Tests;

public class ResultTests
{
    private readonly DummyValue _dummyValue = new() { Name = "Jack Black", Email = "jack.black@exmaple.com" };
    private readonly DummyError _dummyError = new() { Message = "Error message" };
    private readonly DummyNewValue _dummyNewValue = new() { NameAllCaps = "JACK BLACK" };
    private readonly DummyNewError _dummyNewError = new() { Message = "Error message", Count = 2 };

    private Result<DummyValue, DummyError> DummyOkResult => new Result<DummyValue, DummyError>.Ok(_dummyValue);
    private Result<DummyValue, DummyError> DummyErrorResult => new Result<DummyValue, DummyError>.Error(_dummyError);


    [Fact]
    public void ReturnOk_CallMethodWithExplicitErrorType_ReturnsOkResult()
    {
        var okResult = Result.ReturnOk<DummyValue, DummyError>(_dummyValue);

        okResult.Should().BeAssignableTo<Result<DummyValue, DummyError>>();
        okResult.Should().BeOfType<Result<DummyValue, DummyError>.Ok>();
        ((Result<DummyValue, DummyError>.Ok)okResult).Value.Should().BeEquivalentTo(_dummyValue);
    }

    [Fact]
    public void ReturnOk_CallMethodWithExplicitErrorType_ReturnsErrorResult()
    {
        var errorResult = Result.ReturnError<DummyValue, DummyError>(_dummyError);

        errorResult.Should().BeAssignableTo<Result<DummyValue, DummyError>>();
        errorResult.Should().BeOfType<Result<DummyValue, DummyError>.Error>();
        ((Result<DummyValue, DummyError>.Error)errorResult).ErrorOutput.Should().BeEquivalentTo(_dummyError);
    }

    [Fact]
    public void ReturnOk_CallMethodWithStringErrorType_ReturnsOkResult()
    {
        var value = new DummyValue { Name = "JackBlack", Email = "jack.black@exmaple.com" };
        var okResult = Result.ReturnOk(value);

        okResult.Should().BeAssignableTo<Result<DummyValue>>();
        okResult.Should().BeOfType<Result<DummyValue>.Ok>();
        ((Result<DummyValue>.Ok)okResult).Value.Should().BeEquivalentTo(value);
    }

    [Fact]
    public void ReturnOk_CallMethodWithStringErrorType_ReturnsErrorResult()
    {
        var error = _dummyError.Message;
        var errorResult = Result.ReturnError<DummyValue>(error);

        errorResult.Should().BeAssignableTo<Result<DummyValue>>();
        errorResult.Should().BeOfType<Result<DummyValue>.Error>();
        ((Result<DummyValue>.Error)errorResult).Message.Should().BeEquivalentTo(error);
    }

    [Fact]
    public void ImplicitOperator_DoubleGenericParamResultToSingleGenericParamResult_SuccessfulCasts()
    {
        var doubleGenericResult = Result.ReturnOk<DummyValue, string>(_dummyValue);

        doubleGenericResult.Invoking(dgr =>
        {
            Result<DummyValue> singleGenericResult = dgr;
            singleGenericResult.Should().BeOfType<Result<DummyValue>>();
        }).Should().NotThrow<InvalidCastException>();

        doubleGenericResult.Invoking(dgr =>
        {
            var singleGenericResult = (Result<DummyValue>)dgr;
            singleGenericResult.Should().BeOfType<Result<DummyValue>>();
        }).Should().NotThrow<InvalidCastException>();
    }

    [Fact]
    public void ImplicitOperator_SingleGenericParamResultToDoubleGenericParamResult_SuccessfulCasts()
    {
        var singleGenericResult = Result.ReturnOk<DummyValue>(_dummyValue);

        singleGenericResult.Invoking(sgr =>
        {
            Result<DummyValue, string> doubleGenericResult = sgr;
            doubleGenericResult.Should().BeOfType<Result<DummyValue>>();
        }).Should().NotThrow<InvalidCastException>();

        singleGenericResult.Invoking(sgr =>
        {
            var doubleGenericResult = (Result<DummyValue, string>)sgr;
            doubleGenericResult.Should().BeOfType<Result<DummyValue>>();
        }).Should().NotThrow<InvalidCastException>();
    }

    [Fact]
    public void Map_PassTwoPartFunctorWithOkResultAsInput_MapSuccessfully()
    {
        var result = Result.ReturnOk<DummyValue, DummyError>(_dummyValue);
        var mapped = result.Map<DummyNewValue, DummyNewError>(
            value => new DummyNewValue { NameAllCaps = value.Name.ToUpper() },
            error => new DummyNewError { Message = error.Message, Count = 2 }
        );

        mapped.Should().BeAssignableTo<Result<DummyNewValue, DummyNewError>>();
        mapped.Should().BeOfType<Result<DummyNewValue, DummyNewError>.Ok>();
        ((Result<DummyNewValue, DummyNewError>.Ok)mapped).Value.Should().BeEquivalentTo(_dummyNewValue);
    }

    [Fact]
    public void Map_PassTwoPartFunctorWithErrorResultAsInput_MapSuccessfully()
    {
        var result = Result.ReturnError<DummyValue, DummyError>(_dummyError);
        var mapped = result.Map<DummyNewValue, DummyNewError>(
            value => new DummyNewValue { NameAllCaps = value.Name.ToUpper() },
            error => new DummyNewError { Message = error.Message, Count = 2 }
        );

        mapped.Should().BeAssignableTo<Result<DummyNewValue, DummyNewError>>();
        mapped.Should().BeOfType<Result<DummyNewValue, DummyNewError>.Error>();
        ((Result<DummyNewValue, DummyNewError>.Error)mapped).ErrorOutput.Should().BeEquivalentTo(_dummyNewError);
    }

    [Fact]
    public void Map_ValueOnlyFunctorWithOkResultAsInput_MapToOkResult()
    {
        var result = Result.ReturnOk<DummyValue, DummyError>(_dummyValue);
        var mapped = result.Map<DummyNewValue>(
            value => new DummyNewValue { NameAllCaps = value.Name.ToUpper() });

        mapped.Should().BeAssignableTo<Result<DummyNewValue, DummyError>>();
        mapped.Should().BeOfType<Result<DummyNewValue, DummyError>.Ok>();
        ((Result<DummyNewValue, DummyError>.Ok)mapped).Value.Should().BeEquivalentTo(_dummyNewValue);
    }

    [Fact]
    public void Map_ValueOnlyFunctorWithOkResultAsInput_MapToErrorResult()
    {
        var result = Result.ReturnOk<DummyValue, DummyError>(_dummyValue);
        var mapped = result.Map<DummyNewValue>(
            value => _dummyError
        );

        mapped.Should().BeAssignableTo<Result<DummyNewValue, DummyError>>();
        mapped.Should().BeOfType<Result<DummyNewValue, DummyError>.Error>();
        ((Result<DummyNewValue, DummyError>.Error)mapped).ErrorOutput.Should().BeEquivalentTo(_dummyError);
    }

    [Fact]
    public void Map_ErrorOnlyFunctorWithOkResultAsInput_MapToOkResult()
    {
        var result = Result.ReturnError<DummyNewValue, DummyError>(_dummyError);
        var mapped = result.Map<DummyError>(
            error => _dummyNewValue);

        mapped.Should().BeAssignableTo<Result<DummyNewValue, DummyError>>();
        mapped.Should().BeOfType<Result<DummyNewValue, DummyError>.Ok>();
        ((Result<DummyNewValue, DummyError>.Ok)mapped).Value.Should().BeEquivalentTo(_dummyNewValue);
    }

    [Fact]
    public void Map_ErrorOnlyFunctorWithOkResultAsInput_MapToErrorResult()
    {
        var result = Result.ReturnError<DummyNewValue, DummyError>(_dummyError);
        var mapped = result.Map<DummyError>(
            error => error
        );

        mapped.Should().BeAssignableTo<Result<DummyNewValue, DummyError>>();
        mapped.Should().BeOfType<Result<DummyNewValue, DummyError>.Error>();
        ((Result<DummyNewValue, DummyError>.Error)mapped).ErrorOutput.Should().BeEquivalentTo(_dummyError);
    }

    [Fact]
    public void Map__()
    {
        var result = Result.ReturnOk(_dummyValue);
        var mapped = result.Map<DummyNewValue>(
            value => new DummyNewValue { NameAllCaps = value.Name.ToUpper() },
            message => message);

        mapped.Should().BeAssignableTo<Result<DummyNewValue>>();
        mapped.Should().BeOfType<Result<DummyNewValue>.Ok>();
        ((Result<DummyNewValue>.Ok)mapped).Value.Should().BeEquivalentTo(_dummyNewValue);
    }

    [Fact]
    public void Map___()
    {
        var result = Result.ReturnError<DummyValue>(_dummyError.Message);
        var mapped = result.Map<DummyNewValue>(
            value => new DummyNewValue { NameAllCaps = value.Name.ToUpper() },
            message => message + " new");

        mapped.Should().BeAssignableTo<Result<DummyNewValue>>();
        mapped.Should().BeOfType<Result<DummyNewValue>.Error>();
        ((Result<DummyNewValue>.Error)mapped).Message.Should().Be("Error message new");
    }

    [Fact]
    public void GetValueIfOk_CallOnOkResult_OutputsValue()
    {
        var result = Result.ReturnOk<DummyValue, DummyError>(_dummyValue);

        result.GetValueIfOk(out var value).Should().BeTrue();
        value.Should().BeEquivalentTo(_dummyValue);
    }

    [Fact]
    public void GetValueIfOk_CallOnErrorResult_ValueNull()
    {
        var result = Result.ReturnError<DummyValue, DummyError>(_dummyError);

        result.GetValueIfOk(out var value).Should().BeFalse();
        value.Should().BeNull();
    }

    [Fact]
    public void GetOutputIfError_CallOnOkResult_OutputNull()
    {
        var result = Result.ReturnOk<DummyValue, DummyError>(_dummyValue);

        result.GetOutputIfError(out var error).Should().BeFalse();
        error.Should().BeNull();
    }

    [Fact]
    public void GetOutputIfError_CallOnErrorResult_OutputsError()
    {
        var result = Result.ReturnError<DummyValue, DummyError>(_dummyError);

        result.GetOutputIfError(out var error).Should().BeTrue();
        error.Should().BeEquivalentTo(_dummyError);
    }

    [Fact]
    public void GetValueOrThrow_CallOnErrorResultWithNoErrorMessage_ThrowsException()
    {
        var result = Result.ReturnError<DummyValue, DummyError>(_dummyError);
        result.Invoking(r => { r.GetValueOrThrow<InvalidOperationException>(); }).Should()
            .Throw<InvalidOperationException>();
    }

    [Fact]
    public void GetValueOrThrow_CallOnOkResultWithNoErrorMessage_DoesNotThrowAndReturnsValue()
    {
        var result = Result.ReturnOk<DummyValue, DummyError>(_dummyValue);
        result.Invoking(r =>
            {
                r.GetValueOrThrow<InvalidOperationException>()
                    .Should().BeEquivalentTo(_dummyValue);
            })
            .Should().NotThrow<InvalidOperationException>();
    }

    [Fact]
    public void GetValueOrThrow_CallOnErrorResultWithMessageLambda_ThrowsExceptionWithMessage()
    {
        var result = Result.ReturnError<DummyValue, DummyError>(_dummyError);
        result.Invoking(r => { r.GetValueOrThrow<InvalidOperationException>(error => error.Message); }).Should()
            .Throw<InvalidOperationException>()
            .WithMessage(_dummyError.Message);
    }

    [Fact]
    public void Execute_CallMethodWithFallbackActionOnOkResult_ExecutesAction()
    {
        string? executed = default;

        DummyOkResult.Execute(value => { executed = "action"; }, error => { executed = "fallback"; });

        executed.Should().NotBeNull();
        executed.Should().Be("action");
    }
    
    [Fact]
    public void Execute_CallMethodWithFallbackActionOnErrorResult_ExecutesFallback()
    {
        string? executed = default;

        DummyErrorResult.Execute(value => { executed = "action"; }, error => { executed = "fallback"; });

        executed.Should().NotBeNull();
        executed.Should().Be("fallback");
    }
}