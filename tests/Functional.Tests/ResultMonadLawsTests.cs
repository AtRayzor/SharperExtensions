using FluentAssertions;
using Monads.ResultMonad;
using Monads.Tests.DummyTypes;

namespace Monads.Tests;

public class ResultMonadLawsTests
{
    private readonly DummyValue _dummyValue = new() { Name = "Jack Black", Email = "jack.black@exmaple.com" };
    private readonly DummyError _dummyError = new() { Message = "Error message" };
    private readonly DummyNewValue _dummyNewValue = new() { NameAllCaps = "JACK BLACK" };
    private readonly DummyNewError _dummyNewError = new() { Message = "New error message", Count = 2 };
    private readonly DummyNewerValue _dummyNewerValue = new() { NameLowercase = "jack black" };

    private readonly DummyNewerError _dummyNewerError = new()
        { IsCritical = false, Message = "Newer new error message" };

    private Result<DummyNewValue, DummyError> ValueToNewValueResult(DummyValue value) =>
        new DummyNewValue { NameAllCaps = value.Name.ToUpper() };

    private Result<DummyNewerValue, DummyError> NewValueToNewerValueResult(DummyNewValue value) =>
        new DummyNewerValue { NameLowercase = value.NameAllCaps.ToLower() };

    private Result<DummyValue, DummyNewError> ErrorToNewErrorResult(DummyError error) =>
        new DummyNewError { Message = "New " + error.Message.ToLower(), Count = 2 };

    private Result<DummyValue, DummyNewerError> NewErrorToNewerErrorResult(DummyNewError error) => new DummyNewerError
        { Message = "Newer " + error.Message.ToLower(), IsCritical = false };


    private Result<DummyValue, DummyError> DummyOkResult => new Result<DummyValue, DummyError>.Ok(_dummyValue);
    private Result<DummyValue, DummyError> DummyErrorResult => new Result<DummyValue, DummyError>.Error(_dummyError);


    private Result<DummyNewValue, DummyNewError> DummyNewOkResult =>
        new Result<DummyNewValue, DummyNewError>.Ok(_dummyNewValue);

    private Result<DummyNewValue, DummyNewError> DummyNewErrorResult =>
        new Result<DummyNewValue, DummyNewError>.Error(_dummyNewError);

    [Fact]
    public void Map_CheckValueComponentWithOkResults_ConfirmAssociativity()
    {
        var leftFirstResult =
            (Result<DummyNewerValue, DummyError>.Ok)DummyOkResult.Map(
                    ValueToNewValueResult
                )
                .Map(NewValueToNewerValueResult);

        var rightFirstResult =
            (Result<DummyNewerValue, DummyError>.Ok)DummyOkResult.Map<DummyNewerValue>(
                value =>
                    ValueToNewValueResult(value)
                        .Map(NewValueToNewerValueResult));

        leftFirstResult.Should().BeEquivalentTo(rightFirstResult);
    }

    [Fact]
    public void Map_CheckErrorComponentWithErrorResults_ConfirmAssociativity()
    {
        var leftFirstResult =
            (Result<DummyValue, DummyNewerError>.Error)DummyErrorResult.Map(
                    ErrorToNewErrorResult
                )
                .Map(NewErrorToNewerErrorResult);

        var rightFirstResult =
            (Result<DummyValue, DummyNewerError>.Error)DummyErrorResult.Map<DummyNewerError>(
                error => ErrorToNewErrorResult(error)
                    .Map(NewErrorToNewerErrorResult));

        leftFirstResult.Should().BeEquivalentTo(rightFirstResult);
    }

    [Fact]
    public void CheckLeftIdentityWithOkResult()
    {
        Result
            .ReturnOk<DummyValue, DummyError>(_dummyValue)
            .Map(ValueToNewValueResult)
            .Should()
            .BeEquivalentTo(
                (Result<DummyNewValue, DummyError>.Ok)ValueToNewValueResult(_dummyValue));
    }
    
    [Fact]
    public void CheckRightIdentityWithOkResult()
    {
        Result<DummyValue, DummyError> result = new Result<DummyValue, DummyError>.Ok(_dummyValue);
        
            result
            .Map(Result.ReturnOk<DummyValue, DummyError>)
            .Should()
            .BeEquivalentTo((Result<DummyValue, DummyError>.Ok)result);
    }
    
    [Fact]
    public void CheckLeftIdentityWithErrorResult()
    {
        Result
            .ReturnError<DummyValue, DummyError>(_dummyError)
            .Map(ErrorToNewErrorResult)
            .Should()
            .BeEquivalentTo(
                (Result<DummyValue, DummyNewError>.Error)ErrorToNewErrorResult(_dummyError));
    }
    
    [Fact]
    public void CheckRightIdentityWithErrorResult()
    {
        Result<DummyValue, DummyError> result = new Result<DummyValue, DummyError>.Error(_dummyError);
        
        result
            .Map(Result.ReturnError<DummyValue, DummyError>)
            .Should()
            .BeEquivalentTo((Result<DummyValue, DummyError>.Error)result);
    }
}