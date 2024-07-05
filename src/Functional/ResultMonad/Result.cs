using System.Diagnostics.CodeAnalysis;
using Monads.Traits;

namespace Monads.ResultMonad;

public interface IResult<T, TE> : IFunctor<ResultType<T, TE>>, IMonad<ResultType<T, TE>>
    where T : notnull where TE : notnull
{
    static IConstructableTrait<ResultType<T, TE>> IConstructableTrait<ResultType<T, TE>>.
        Construct(ResultType<T, TE> type) => Construct(type);

    new static IResult<T, TE> Construct(ResultType<T, TE> type) => new Result<T, TE>(type);
}

public interface IResult<T> : IResult<T, string>
    where T : notnull
{
}

public abstract partial class Result
{
    public static IResult<T, TE> Construct<T, TE>(T? value, TE nullError) where T : notnull where TE : notnull =>
        value is not null ? Ok<T, TE>(value) : Error<T, TE>(nullError);

    public static IResult<T, TE> Ok<T, TE>(T value) where T : notnull where TE : notnull =>
        IResult<T, TE>.Construct(new ResultType<T, TE>.Ok(value));

    public static IResult<T, TE> Error<T, TE>(TE error) where T : notnull where TE : notnull =>
        IResult<T, TE>.Construct(new ResultType<T, TE>.Error(error));

    public static IResult<T> Construct<T>(T? value,string nullError) where T : notnull =>
        value is not null ? Ok(value) : Error<T>(nullError);

    public static IResult<T> Ok<T>(T value) where T : notnull =>
        (IResult<T>)IResult<T>.Construct(new ResultType<T>.Ok(value));

    public static IResult<T> Error<T>(string error) where T : notnull =>
        (IResult<T>)IResult<T>.Construct(new ResultType<T>.Error(error));
}

public record struct Result<T, TE>(ResultType<T, TE> Type) : IResult<T, TE> where T : notnull where TE : notnull;
