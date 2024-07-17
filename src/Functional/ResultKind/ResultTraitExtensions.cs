using System.Diagnostics.CodeAnalysis;
using Monads.OptionKind;
using Monads.Traits;

namespace Monads.ResultMonad;

public static class ResultTraitExtensions
{
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