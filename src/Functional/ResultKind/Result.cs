using System.Diagnostics.CodeAnalysis;
using Monads.Traits;

namespace Monads.ResultMonad;

public interface IResult<T, TE> : IFunctor<ResultType<T, TE>>, IMonad<ResultType<T, TE>>,
    IApplicative<ResultType<T, TE>>
    where T : notnull where TE : notnull
{
}

public interface IResult<T> : IFunctor<ResultType<T>>, IMonad<ResultType<T>>, IApplicative<ResultType<T>>
    where T : notnull
{
}

public abstract class ResultFactory<T, TE> : TraitFactory<ResultType<T, TE>, IResult<T, TE>>
    where T : notnull where TE : notnull;

public abstract class ResultFactory<T> : TraitFactory<ResultType<T>, IResult<T>>
    where T : notnull;

public static partial class Result
{
    public static IResult<T, TE> Construct<T, TE>(T? value, TE nullError) where T : notnull where TE : notnull =>
        value is not null ? Ok<T, TE>(value) : Error<T, TE>(nullError);

    public static IResult<T, TE> Ok<T, TE>(T value) where T : notnull where TE : notnull =>
        ResultFactory<T, TE>.Construct<Result<T, TE>>(new ResultType<T, TE>.Ok(value));

    public static IResult<T, TE> Error<T, TE>(TE error) where T : notnull where TE : notnull =>
        ResultFactory<T, TE>.Construct<Result<T, TE>>(new ResultType<T, TE>.Error(error));

    public static IResult<T> Construct<T>(T? value, string nullError) where T : notnull =>
        value is not null ? Ok(value) : Error<T>(nullError);

    public static IResult<T> Ok<T>(T value) where T : notnull =>
        ResultFactory<T>.Construct<Result<T>>(new ResultType<T>.Ok(value));

    public static IResult<T> Error<T>(string error) where T : notnull =>
        ResultFactory<T>.Construct<Result<T>>(new ResultType<T>.Error(error));
}

public record struct Result<T, TE>(ResultType<T, TE> Type) : IResult<T, TE> where T : notnull where TE : notnull;

public record struct Result<T>(ResultType<T> Type) : IResult<T> where T : notnull;