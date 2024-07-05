using System.Diagnostics.CodeAnalysis;
using Monads.Traits;

namespace Monads.ResultMonad;

public static class ResultTraitExtensions
{
    internal static TConstructable Construct<T, TE, TConstructable>(T? value, TE nullError)
        where TConstructable : IConstructableTrait<ResultType<T, TE>> where T : notnull where TE : notnull
        => value is not null
            ? ConstructOk<T, TE, TConstructable>(value)
            : ConstructError<T, TE, TConstructable>(nullError);

    internal static TConstructable ConstructOk<T, TE, TConstructable>(T value)
        where TConstructable : IConstructableTrait<ResultType<T, TE>> where T : notnull where TE : notnull
        => (TConstructable)TConstructable.Construct(new ResultType<T, TE>.Ok(value));

    internal static TConstructable ConstructError<T, TE, TConstructable>(TE error)
        where TConstructable : IConstructableTrait<ResultType<T, TE>> where T : notnull where TE : notnull
        => (TConstructable)TConstructable.Construct(new ResultType<T, TE>.Error(error));

    public static bool IsOk<T, TE>(this ITrait<ResultType<T, TE>> resultTrait) where T : notnull where TE : notnull
        => resultTrait.Type is ResultType<T, TE>.Ok;

    public static bool IsError<T, TE>(this ITrait<ResultType<T, TE>> resultTrait) where T : notnull where TE : notnull
        => resultTrait.Type is ResultType<T, TE>.Error;

    public static bool TryGetValue<T, TE>(this ITrait<ResultType<T, TE>> resultTrait,
        [NotNullWhen(returnValue: true)] out T? value)
        where T : notnull where TE : notnull
    {
        value = resultTrait.IsOk() ? ((ResultType<T, TE>.Ok)resultTrait.Type).Value : default;

        return value is not null;
    }


    public static bool TryGetError<T, TE>(this ITrait<ResultType<T, TE>> resultTrait,
        [NotNullWhen(returnValue: true)] out TE? error)
        where T : notnull where TE : notnull
    {
        error = resultTrait.IsError() ? ((ResultType<T, TE>.Error)resultTrait.Type).Err : default;

        return error is not null;
    }
}