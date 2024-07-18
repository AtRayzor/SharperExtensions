using System.Collections;
using System.Reflection;
using FluentAssertions;
using Monads.ResultMonad;
using Monads.Tests.DummyTypes;
using Monads.Traits;

namespace Monads.Tests;

public class ResultMonadTests
{
    [Fact]
    public void Bind_CallOnOkResult_ReturnsOk()
    {
        var boundMonad = TestData.ValueResultMonad.Bind(
            TestFunctions.Binder
        ) as Monad<ResultType<DummyNewValue, DummyError>.Ok>?;
        boundMonad?.Type.Should().BeEquivalentTo(TestData.NewValueResult);
    }
    
    [Fact]
    public void Bind_CallOnErrorResult_ReturnsError()
    {
        var boundMonad = TestData.ErrorResultMonad.Bind(
            TestFunctions.Binder
        ) as Monad<ResultType<DummyNewValue, DummyError>.Error>?;
        boundMonad?.Type.Should().BeEquivalentTo(TestData.NewErrorResult);
    }
    
    [Fact]
    public async Task BindAsync_CallOnOkResult_ReturnsOk()
    {
        var boundMonad = await Task.FromResult(TestData.ValueResultMonad).BindAsync(
            TestFunctions.BinderAsync
        ) as Monad<ResultType<DummyNewValue, DummyError>.Ok>?;
        boundMonad?.Type.Should().BeEquivalentTo(TestData.NewValueResult);
    }

    [Fact]
    public async Task BindAsync_CallOnErrorResult_ReturnsError()
    {
        var boundMonad = await Task.FromResult(TestData.ErrorResultMonad).BindAsync(
            TestFunctions.BinderAsync
        ) as Monad<ResultType<DummyNewValue, DummyError>.Error>?;
        boundMonad?.Type.Should().BeEquivalentTo(TestData.NewErrorResult);
    }
}

file static class TestData
{
    public static DummyValue Value => new() { Name = "Jack Black", Email = "jack.black@example.com" };
    public static DummyNewValue NewValue => new() { NameAllCaps = "JACK BLACK" };
    public static string ErrorMessage => "error message";
    public static DummyError Error => new() { Message = "error message" };

    public static ResultType<DummyValue, DummyError> ValueResult => new ResultType<DummyValue, DummyError>.Ok(Value);
    public static ResultType<DummyValue, DummyError> ErrorResult => new ResultType<DummyValue, DummyError>.Error(Error);

    public static ResultType<DummyNewValue, DummyError> NewValueResult =>
        new ResultType<DummyNewValue, DummyError>.Ok(NewValue);

    public static ResultType<DummyNewValue, DummyError> NewErrorResult =>
        new ResultType<DummyNewValue, DummyError>.Error(Error);

    public static IMonad<ResultType<DummyValue, DummyError>> ValueResultMonad =>
        new Monad<ResultType<DummyValue, DummyError>>(new ResultType<DummyValue, DummyError>.Ok(Value));

    public static IMonad<ResultType<DummyValue, DummyError>>
        ErrorResultMonad =>
        new Monad<ResultType<DummyValue, DummyError>>(new ResultType<DummyValue, DummyError>.Error(Error));
}

file static class TestFunctions
{
    public static IMonad<ResultType<DummyNewValue, DummyError>> Binder(DummyValue value) =>
        new Monad<ResultType<DummyNewValue, DummyError>>(
            new ResultType<DummyNewValue, DummyError>.Ok(
                new DummyNewValue { NameAllCaps = value.Name.ToUpper() }
            ));

     public static Task<IMonad<ResultType<DummyNewValue, DummyError>>> BinderAsync(DummyValue value) =>
        Task.FromResult(
            (IMonad<ResultType<DummyNewValue, DummyError>>)new Monad<ResultType<DummyNewValue, DummyError>>(
                new ResultType<DummyNewValue, DummyError>.Ok(
                    new DummyNewValue { NameAllCaps = value.Name.ToUpper() }
                )));
}
