using FluentAssertions;
using NetFunction.Types.Tests.DummyTypes;
using NetFunctional.Types.ResultKind;
using NetFunctional.Types.Traits;

namespace NetFunction.Types.Tests;

public class ResultFunctorTests
{
    public static DummyValue Value =>
        new() { Name = "Jack Black", Email = "jack.black@example.com" };

    public static DummyNewValue NewValue => new() { NameAllCaps = "JACK BLACK" };
    public static string ErrorMessage => "error message";
    public static DummyError Error => new() { Message = "error message" };
    public static DummyNewError NewError => new() { Message = "new error", Count = 2 };

    public static IFunctor<ResultType<DummyValue, DummyError>> OkResultFunctor =>
        new Functor<ResultType<DummyValue, DummyError>>(
            new ResultType<DummyValue, DummyError>.Ok(Value)
        );

    public static IFunctor<ResultType<DummyValue>> StringOkResultFunctor =>
        new Functor<ResultType<DummyValue>>(new ResultType<DummyValue>.Ok(Value));

    public static IFunctor<ResultType<DummyValue, DummyError>> ErrorResultFunctor =>
        new Functor<ResultType<DummyValue, DummyError>>(
            new ResultType<DummyValue, DummyError>.Error(Error)
        );

    public static IFunctor<ResultType<DummyValue>> StringErrorResultFunctor =>
        new Functor<ResultType<DummyValue>>(new ResultType<DummyValue>.Error(ErrorMessage));

    public static ResultType<DummyNewValue, DummyError> NewOkResult =>
        new ResultType<DummyNewValue, DummyError>.Ok(NewValue);

    public static ResultType<DummyValue, DummyNewError> NewErrorResult =>
        new ResultType<DummyValue, DummyNewError>.Error(NewError);

    [Fact]
    public async Task MapOkAsync_CallOnOkResult_ReturnsNewOkResultFunctor()
    {
        var expected = new Functor<ResultType<DummyNewValue, DummyError>>(NewOkResult);
        var resultTask = Task.FromResult(OkResultFunctor);
        (await resultTask.MapOkAsync(OkMap)).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task MapOkAsync_CallOnErrorResult_ReturnsErrorResult()
    {
        var expected = new Functor<ResultType<DummyNewValue, DummyError>>(
            new ResultType<DummyNewValue, DummyError>.Error(Error)
        );
        var resultTask = Task.FromResult(ErrorResultFunctor);
        (await resultTask.MapOkAsync(OkMap)).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task MapErrorAsync_CallOnErrorResult_ReturnsNewErrorResultFunctor()
    {
        var expected = new Functor<ResultType<DummyValue, DummyNewError>>(NewErrorResult);
        var resulTask = Task.FromResult(ErrorResultFunctor);
        (await resulTask.MapErrorAsync(ErrorMap)).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task MapErrorAsync_CallOnOkResult_ReturnsOkResultFunctor()
    {
        var expected = new Functor<ResultType<DummyValue, DummyNewError>>(
            new ResultType<DummyValue, DummyNewError>.Ok(Value)
        );
        var resultTask = Task.FromResult(OkResultFunctor);
        (await resultTask.MapErrorAsync(ErrorMap)).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task MapOkAsync_CallOnOkResult_ReturnsNewOkStringErrorTypeResultFunctor()
    {
        var expected = new Functor<ResultType<DummyNewValue>>(
            new ResultType<DummyNewValue>.Ok(NewValue)
        );

        var resultTask = Task.FromResult(StringOkResultFunctor);
        (await resultTask.MapOkAsync(OkMap)).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task MapOkAsync_CallErrorResult_ReturnsStringErrorResultFunctor()
    {
        var expected = new Functor<ResultType<DummyNewValue>>(
            new ResultType<DummyNewValue>.Error(ErrorMessage)
        );
        var resultTask = Task.FromResult(StringErrorResultFunctor);
        (await resultTask.MapOkAsync(OkMap)).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task MapErrorAsync_CallOnErrorResult_ReturnsNewStringErrorResultFunctor()
    {
        var expected = new Functor<ResultType<DummyValue>>(
            new ResultType<DummyValue>.Error($"new {ErrorMessage}")
        );
        var resultTask = Task.FromResult(StringErrorResultFunctor);
        (await resultTask.MapErrorAsync(StringErrorMap)).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task MapErrorAsync_CallOnOkResult_ReturnsStringErrorOkResultFunctor()
    {
        var expected = new Functor<ResultType<DummyValue>>(new ResultType<DummyValue>.Ok(Value));
        var resultTask = Task.FromResult(StringOkResultFunctor);
        (await resultTask.MapErrorAsync(StringErrorMap)).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Match_CallOnOkResultFunctor_ReturnsNewOkResult()
    {
        var expected = new Functor<ResultType<DummyNewValue, DummyNewerError>>(
            new ResultType<DummyNewValue, DummyNewerError>.Ok(NewValue)
        );
        OkResultFunctor.Match(OkMap, ErrorMap).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Match_CallOnOkResultFunctor_ReturnsNewErrorResult()
    {
        var expected = new Functor<ResultType<DummyNewValue, DummyNewError>>(
            new ResultType<DummyNewValue, DummyNewError>.Error(NewError)
        );
        ErrorResultFunctor.Match(OkMap, ErrorMap).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Match_CallOnOkResultFunctor_ReturnsNewStringErrorTypeOkResult()
    {
        var expected = new Functor<ResultType<DummyNewValue>>(
            new ResultType<DummyNewValue>.Ok(NewValue)
        );
        OkResultFunctor.Match(OkMap, ErrorMap).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Match_CallOnOkResultFunctor_ReturnsNewStringErrorResult()
    {
        var expected = new Functor<ResultType<DummyNewValue>>(
            new ResultType<DummyNewValue>.Error($"new {ErrorMessage}")
        );
        ErrorResultFunctor.Match(OkMap, ErrorMap).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void MapOk_CallOnOkResult_ReturnsNewOkResultFunctor()
    {
        var expected = new Functor<ResultType<DummyNewValue, DummyError>>(NewOkResult);
        OkResultFunctor.MapOk(OkMap).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void MapOk_CallOnErrorResult_ReturnsErrorResult()
    {
        var expected = new Functor<ResultType<DummyNewValue, DummyError>>(
            new ResultType<DummyNewValue, DummyError>.Error(Error)
        );
        ErrorResultFunctor.MapOk(OkMap).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void MapError_CallOnErrorResult_ReturnsNewErrorResultFunctor()
    {
        var expected = new Functor<ResultType<DummyValue, DummyNewError>>(NewErrorResult);
        ErrorResultFunctor.MapError(ErrorMap).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void MapError_CallOnOkResult_ReturnsOkResultFunctor()
    {
        var expected = new Functor<ResultType<DummyValue, DummyNewError>>(
            new ResultType<DummyValue, DummyNewError>.Ok(Value)
        );
        OkResultFunctor.MapError(ErrorMap).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void MapOk_CallOnOkResult_ReturnsNewOkStringErrorTypeResultFunctor()
    {
        var expected = new Functor<ResultType<DummyNewValue>>(
            new ResultType<DummyNewValue>.Ok(NewValue)
        );
        StringOkResultFunctor.MapOk(OkMap).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void MapOk_CallErrorResult_ReturnsStringErrorResultFunctor()
    {
        var expected = new Functor<ResultType<DummyNewValue>>(
            new ResultType<DummyNewValue>.Error(ErrorMessage)
        );
        StringErrorResultFunctor.MapOk(OkMap).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void MapError_CallOnErrorResult_ReturnsNewStringErrorResultFunctor()
    {
        var expected = new Functor<ResultType<DummyValue>>(
            new ResultType<DummyValue>.Error($"new {ErrorMessage}")
        );
        StringErrorResultFunctor.MapError(StringErrorMap).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void MapError_CallOnOkResult_ReturnsStringErrorOkResultFunctor()
    {
        var expected = new Functor<ResultType<DummyValue>>(new ResultType<DummyValue>.Ok(Value));
        StringOkResultFunctor.MapError(StringErrorMap).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task MatchAsync_CallOnOkResultFunctor_ReturnsNewOkResult()
    {
        var expected = new Functor<ResultType<DummyNewValue, DummyNewerError>>(
            new ResultType<DummyNewValue, DummyNewerError>.Ok(NewValue)
        );
        (await Task.FromResult(OkResultFunctor).MatchAsync(OkMap, ErrorMap))
            .Should()
            .BeEquivalentTo(expected);
    }

    [Fact]
    public async Task MatchAsync_CallOnOkResultFunctor_ReturnsNewErrorResult()
    {
        var expected = new Functor<ResultType<DummyNewValue, DummyNewError>>(
            new ResultType<DummyNewValue, DummyNewError>.Error(NewError)
        );
        (await Task.FromResult(ErrorResultFunctor).MatchAsync(OkMap, ErrorMap))
            .Should()
            .BeEquivalentTo(expected);
    }

    [Fact]
    public async Task MatchAsync_CallOnOkResultFunctor_ReturnsNewStringErrorTypeOkResult()
    {
        var expected = new Functor<ResultType<DummyNewValue>>(
            new ResultType<DummyNewValue>.Ok(NewValue)
        );
        (await Task.FromResult(OkResultFunctor).MatchAsync(OkMap, ErrorMap))
            .Should()
            .BeEquivalentTo(expected);
    }

    [Fact]
    public async Task MatchAsync_CallOnOkResultFunctor_ReturnsNewStringErrorResult()
    {
        var expected = new Functor<ResultType<DummyNewValue>>(
            new ResultType<DummyNewValue>.Error($"new {ErrorMessage}")
        );
        (await Task.FromResult(ErrorResultFunctor).MatchAsync(OkMap, ErrorMap))
            .Should()
            .BeEquivalentTo(expected);
    }

    public static DummyNewValue OkMap(DummyValue value)
    {
        return new DummyNewValue { NameAllCaps = value.Name.ToUpper() };
    }

    public static DummyNewError ErrorMap(DummyError error)
    {
        return new DummyNewError { Message = "new " + error.Message, Count = 2 };
    }

    public static string StringErrorMap(string message)
    {
        return "new " + message;
    }
}
