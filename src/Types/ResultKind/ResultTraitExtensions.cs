using System.Diagnostics.CodeAnalysis;
using NetFunctional.Types.Traits;

namespace NetFunctional.Types.ResultKind;

public static class ResultTraitExtensions
{
    public static bool IsOk<T, TE>(this ITrait<ResultType<T, TE>> resultTrait)
        where T : notnull
        where TE : notnull
    {
        return resultTrait.Type is ResultType<T, TE>.Ok;
    }

    public static bool IsError<T, TE>(this ITrait<ResultType<T, TE>> resultTrait)
        where T : notnull
        where TE : notnull
    {
        return resultTrait.Type is ResultType<T, TE>.Error;
    }

    public static bool TryGetValue<T, TE>(
        this ITrait<ResultType<T, TE>> resultTrait,
        [NotNullWhen(true)] out T? value
    )
        where T : notnull
        where TE : notnull
    {
        value = resultTrait.IsOk() ? ((ResultType<T, TE>.Ok)resultTrait.Type).Value : default;

        return value is not null;
    }

    public static bool TryGetError<T, TE>(
        this ITrait<ResultType<T, TE>> resultTrait,
        [NotNullWhen(true)] out TE? error
    )
        where T : notnull
        where TE : notnull
    {
        error = resultTrait.IsError() ? ((ResultType<T, TE>.Error)resultTrait.Type).Err : default;

        return error is not null;
    }

    public static IResult<T, TE> AsResult<T, TE>(
        this IConstructableTrait<ResultType<T, TE>> resultTrait
    )
        where T : notnull
        where TE : notnull
    {
        return resultTrait.AsConstructableTrait<ResultType<T, TE>, IResult<T, TE>, Result<T, TE>>();
    }

    public static IFunctor<ResultType<T, TE>> AsFunctor<T, TE>(
        this IConstructableTrait<ResultType<T, TE>> resultTrait
    )
        where T : notnull
        where TE : notnull
    {
        return resultTrait.AsConstructableTrait<
            ResultType<T, TE>,
            IFunctor<ResultType<T, TE>>,
            Functor<ResultType<T, TE>>
        >();
    }

    public static IMonad<ResultType<T, TE>> AsMonad<T, TE>(
        this IConstructableTrait<ResultType<T, TE>> resultTrait
    )
        where T : notnull
        where TE : notnull
    {
        return resultTrait.AsConstructableTrait<
            ResultType<T, TE>,
            IMonad<ResultType<T, TE>>,
            Monad<ResultType<T, TE>>
        >();
    }

    public static IApplicative<ResultType<T, TE>> AsApplicative<T, TE>(
        this IConstructableTrait<ResultType<T, TE>> resultTrait
    )
        where T : notnull
        where TE : notnull
    {
        return resultTrait.AsConstructableTrait<
            ResultType<T, TE>,
            IApplicative<ResultType<T, TE>>,
            Applicative<ResultType<T, TE>>
        >();
    }
}
