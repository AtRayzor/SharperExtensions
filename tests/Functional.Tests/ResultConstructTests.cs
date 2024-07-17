using FluentAssertions;
using Monads.ResultMonad;
using Monads.Tests.DummyTypes;
using Monads.Traits;

namespace Monads.Tests;

public class ResultConstructTests
{
    private static DummyValue Value => new() { Name = "Jack Black", Email = "jack.black@example.com" };
    private static DummyValue? NullValue => default;

    private static DummyError Error => new() { Message = "error message" };

    [Fact]
    public void FunctorConstruct_CallWithNonNullValue_ReturnOkResult()
    {
        var expected =
            new Functor<ResultType<DummyValue, DummyError>>(new ResultType<DummyValue, DummyError>.Ok(Value));
        Functor.Result.Construct(Value, Error).Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void FunctorConstruct_CallWithNullValue_ReturnErrorResult()
    {
        var expected =
            new Functor<ResultType<DummyValue, DummyError>>(new ResultType<DummyValue, DummyError>.Error(Error));
        Functor.Result.Construct(NullValue, Error).Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void FunctorStringErrorConstruct_CallWithNonNullValue_ReturnOkResult()
    {
        var expected =
            new Functor<ResultType<DummyValue>>(new ResultType<DummyValue>.Ok(Value));
        Functor.Result.Construct(Value, "error message").Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void FunctorStringErrorConstruct_CallWithNullValue_ReturnErrorResult()
    {
        const string expectedMessage = "error message";
        var expected =
            new Functor<ResultType<DummyValue>>(new ResultType<DummyValue>.Error(expectedMessage));
        Functor.Result.Construct(NullValue, Error).Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void MonadConstruct_CallWithNonNullValue_ReturnOkResult()
    {
        var expected =
            new Monad<ResultType<DummyValue, DummyError>>(new ResultType<DummyValue, DummyError>.Ok(Value));
        Monad.Result.Construct(Value, Error).Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void MonadConstruct_CallWithNullValue_ReturnErrorResult()
    {
        var expected =
            new Functor<ResultType<DummyValue, DummyError>>(new ResultType<DummyValue, DummyError>.Error(Error));
        Monad.Result.Construct(NullValue, Error).Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void MonadStringErrorConstruct_CallWithNonNullValue_ReturnOkResult()
    {
        var expected =
            new Monad<ResultType<DummyValue>>(new ResultType<DummyValue>.Ok(Value));
        Monad.Result.Construct(Value, "error message").Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void MonadStringErrorConstruct_CallWithNullValue_ReturnErrorResult()
    {
        const string expectedMessage = "error message";
        var expected =
            new Functor<ResultType<DummyValue>>(new ResultType<DummyValue>.Error(expectedMessage));
        Monad.Result.Construct(NullValue, Error).Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void ReturnConstruct_CallWithNonNullValue_ReturnOkResult()
    {
        var expected =
            new Result<DummyValue, DummyError>(new ResultType<DummyValue, DummyError>.Ok(Value));
        Result.Construct(Value, Error).Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void ResultConstruct_CallWithNullValue_ReturnErrorResult()
    {
        var expected =
            new Result<DummyValue, DummyError>(new ResultType<DummyValue, DummyError>.Error(Error));
        Result.Construct(NullValue, Error).Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void ResultStringErrorConstruct_CallWithNonNullValue_ReturnOkResult()
    {
        var expected =
            new Result<DummyValue>(new ResultType<DummyValue>.Ok(Value));
        Result.Construct(Value, "error message").Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void ResultStringErrorConstruct_CallWithNullValue_ReturnErrorResult()
    {
        const string expectedMessage = "error message";
        var expected =
            new Result<DummyValue>(new ResultType<DummyValue>.Error(expectedMessage));
        Result.Construct(NullValue, Error).Should().BeEquivalentTo(expected);
    }
}