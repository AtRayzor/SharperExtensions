using FluentAssertions;
using NetFunction.Types.Tests.DummyTypes;
using NetFunctional.Types.ResultKind;
using NetFunctional.Types.Traits;

namespace NetFunction.Types.Tests;

public class ResultApplicativeTests
{
    [Fact]
    public void Apply_CallOnOkResultWithOkLambda_ReturnsOk()
    {
        (
            TestData.ValueResulApplicative.Apply(TestData.OkApplicativeMapping).Type
            as ResultType<DummyNewValue, DummyError>.Ok
        )
            .Should()
            .BeEquivalentTo(TestData.NewValueResult);
    }

    [Fact]
    public void Apply_CallOnOkResultWithErrorLambda_ReturnsError()
    {
        (
            TestData.ValueResulApplicative.Apply(TestData.ErrorApplicativeMapping).Type
            as ResultType<DummyNewValue, DummyError>.Error
        )
            .Should()
            .BeEquivalentTo(TestData.NewMappingErrorResult);
    }

    [Fact]
    public void Apply_CallOnErrorResultWithOkLambda_ReturnsError()
    {
        (
            TestData.ErrorResultApplicative.Apply(TestData.OkApplicativeMapping).Type
            as ResultType<DummyNewValue, DummyError>.Error
        )
            .Should()
            .BeEquivalentTo(TestData.NewErrorResult);
    }

    [Fact]
    public void Apply_CallOnErrorResultWithErrorLambda_ReturnsError()
    {
        (
            TestData.ErrorResultApplicative.Apply(TestData.ErrorApplicativeMapping).Type
            as ResultType<DummyNewValue, DummyError>.Error
        )
            .Should()
            .BeEquivalentTo(TestData.NewMappingErrorResult);
    }

    [Fact]
    public async Task ApplyAsync_CallOnOkResultWithOkLambda_ReturnsOk()
    {
        (
            (
                await TestData
                    .ValueResulApplicativeAsync
                    .ApplyAsync(TestData.OkApplicativeMappingAsync)
            ).Type as ResultType<DummyNewValue, DummyError>.Ok
        )
            .Should()
            .BeEquivalentTo(TestData.NewValueResult);
    }

    [Fact]
    public async Task ApplyAsync_CallOnOkResultWithErrorLambda_ReturnsError()
    {
        (
            (
                await TestData
                    .ValueResulApplicativeAsync
                    .ApplyAsync(TestData.ErrorApplicativeMappingAsync)
            ).Type as ResultType<DummyNewValue, DummyError>.Error
        )
            .Should()
            .BeEquivalentTo(TestData.NewMappingErrorResult);
    }

    [Fact]
    public async Task ApplyAsync_CallOnErrorResultWithOkLambda_ReturnsError()
    {
        (
            (
                await TestData
                    .ErrorResultApplicativeAsync
                    .ApplyAsync(TestData.OkApplicativeMappingAsync)
            ).Type as ResultType<DummyNewValue, DummyError>.Error
        )
            .Should()
            .BeEquivalentTo(TestData.NewErrorResult);
    }

    [Fact]
    public async Task Apply_CallOnErrorResultWithErrorLambda_ReturnsOk()
    {
        (
            (
                await TestData
                    .ErrorResultApplicativeAsync
                    .ApplyAsync(TestData.ErrorApplicativeMappingAsync)
            ).Type as ResultType<DummyNewValue, DummyError>.Error
        )
            .Should()
            .BeEquivalentTo(TestData.NewMappingErrorResult);
    }
}

file static class TestData
{
    public static DummyValue Value =>
        new() { Name = "Jack Black", Email = "jack.black@example.com" };

    public static DummyNewValue NewValue => new() { NameAllCaps = "JACK BLACK" };
    public static string ErrorMessage => "error message";
    public static DummyError Error => new() { Message = "error message" };
    public static DummyError MappingError => new() { Message = "mapping could not be created" };

    public static ResultType<DummyValue, DummyError>.Ok ValueResult => new(Value);

    public static ResultType<DummyValue, DummyError>.Error ErrorResult => new(Error);

    public static ResultType<Func<DummyValue, DummyNewValue>, DummyError> MappingErrorResult =>
        new ResultType<Func<DummyValue, DummyNewValue>, DummyError>.Error(MappingError);

    public static ResultType<DummyNewValue, DummyError>.Ok NewValueResult => new(NewValue);
    public static ResultType<DummyNewValue, DummyError>.Error NewErrorResult => new(Error);

    public static ResultType<DummyNewValue, DummyError>.Error NewMappingErrorResult =>
        new(MappingError);

    public static IApplicative<ResultType<DummyValue, DummyError>> ValueResulApplicative =>
        new Applicative<ResultType<DummyValue, DummyError>>(
            new ResultType<DummyValue, DummyError>.Ok(Value)
        );

    public static IApplicative<ResultType<DummyValue, DummyError>> ErrorResultApplicative =>
        new Applicative<ResultType<DummyValue, DummyError>>(
            new ResultType<DummyValue, DummyError>.Error(Error)
        );

    public static IApplicative<
        ResultType<Func<DummyValue, DummyNewValue>, DummyError>
    > OkApplicativeMapping =>
        new Result<Func<DummyValue, DummyNewValue>, DummyError>(
            new ResultType<Func<DummyValue, DummyNewValue>, DummyError>.Ok(TestFunctions.Mapping)
        );

    public static IApplicative<
        ResultType<Func<DummyValue, DummyNewValue>, DummyError>
    > ErrorApplicativeMapping =>
        new Result<Func<DummyValue, DummyNewValue>, DummyError>(
            new ResultType<Func<DummyValue, DummyNewValue>, DummyError>.Error(MappingError)
        );

    public static Task<
        IApplicative<ResultType<DummyValue, DummyError>>
    > ValueResulApplicativeAsync => Task.FromResult(ValueResulApplicative);

    public static Task<
        IApplicative<ResultType<DummyValue, DummyError>>
    > ErrorResultApplicativeAsync => Task.FromResult(ErrorResultApplicative);

    public static Task<
        IApplicative<ResultType<Func<DummyValue, DummyNewValue>, DummyError>>
    > OkApplicativeMappingAsync => Task.FromResult(OkApplicativeMapping);

    public static Task<
        IApplicative<ResultType<Func<DummyValue, DummyNewValue>, DummyError>>
    > ErrorApplicativeMappingAsync => Task.FromResult(ErrorApplicativeMapping);
}

file static class TestFunctions
{
    public static DummyNewValue Mapping(DummyValue value)
    {
        return new DummyNewValue { NameAllCaps = value.Name.ToUpper() };
    }
}
