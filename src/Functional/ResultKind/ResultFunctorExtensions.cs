using Monads.Traits;

namespace Monads.ResultMonad;

public static class ResultFunctorExtensions
{
    public static IFunctor<ResultType<TNew, TE>> MapOk<T, TNew, TE>(this IFunctor<ResultType<T, TE>> resultFunctor,
        Func<T, TNew> map) where T : notnull where TE : notnull where TNew : notnull => resultFunctor.Type switch
    {
        ResultType<T, TE>.Ok okResult =>
            new Functor<ResultType<TNew, TE>>(new ResultType<TNew, TE>.Ok(map(okResult.Value))),
        ResultType<T, TE>.Error errorResult => new Functor<ResultType<TNew, TE>>(
            new ResultType<TNew, TE>.Error(errorResult.Err))
    };

    public static IFunctor<ResultType<T, TENew>> MapError<T, TE, TENew>(this IFunctor<ResultType<T, TE>> resultFunctor,
        Func<TE, TENew> mapError)
        where T : notnull where TE : notnull where TENew : notnull => resultFunctor.Type switch
    {
        ResultType<T, TE>.Error errorResult => new Functor<ResultType<T, TENew>>(
            new ResultType<T, TENew>.Error(mapError(errorResult.Err))),
        ResultType<T, TE>.Ok okResult => new Functor<ResultType<T, TENew>>(new ResultType<T, TENew>.Ok(okResult.Value))
    };

    public static IFunctor<ResultType<TNew>> MapOk<T, TNew>(this IFunctor<ResultType<T>> resultFunctor,
        Func<T, TNew> map) where T : notnull where TNew : notnull => resultFunctor.Type switch
    {
        ResultType<T>.Ok okResult =>
            new Functor<ResultType<TNew>>(new ResultType<TNew>.Ok(map(okResult.Value))),
        ResultType<T>.Error errorResult => new Functor<ResultType<TNew>>(
            new ResultType<TNew>.Error(errorResult.Message))
    };

    public static IFunctor<ResultType<T>> MapError<T>(this IFunctor<ResultType<T>> resultFunctor,
        Func<string, string> mapError)
        where T : notnull => resultFunctor.Type switch
    {
        ResultType<T>.Error errorResult => new Functor<ResultType<T>>(
            new ResultType<T>.Error(mapError(errorResult.Message))),
        ResultType<T>.Ok okResult => new Functor<ResultType<T>>(new ResultType<T>.Ok(okResult.Value))
    };

    public static IFunctor<ResultType<TNew, TENew>>
        Match<T, TE, TNew, TENew>(
            this IFunctor<ResultType<T, TE>> resultFunctor,
            Func<T, TNew> matchOk,
            Func<TE, TENew> matchError
        ) where T : notnull
        where TE : notnull
        where TNew : notnull
        where TENew : notnull => resultFunctor.Type switch
    {
        ResultType<T, TE>.Ok okResult => new Functor<ResultType<TNew, TENew>>(
            new ResultType<TNew, TENew>.Ok(matchOk(okResult.Value))),
        ResultType<T, TE>.Error errorResult => new Functor<ResultType<TNew, TENew>>(
            new ResultType<TNew, TENew>.Error(matchError(errorResult.Err)))
    };

    public static IFunctor<ResultType<TNew>>
        Match<T, TNew>(this IFunctor<ResultType<T>> resultFunctor, 
            Func<T, TNew> matchOk,
            Func<string, string> matchError) where T : notnull
        where TNew : notnull => resultFunctor.Type switch
    {
        ResultType<T>.Ok okResult => new Functor<ResultType<TNew>>(
            new ResultType<TNew>.Ok(matchOk(okResult.Value))),
        ResultType<T>.Error errorResult => new Functor<ResultType<TNew>>(
            new ResultType<TNew>.Error(matchError(errorResult.Message)))
    };

    public static async Task<IFunctor<ResultType<TNew>>> MapOkAsync<T, TNew>(
        this Task<IFunctor<ResultType<T>>> resultFunctorTask,
        Func<T, TNew> map) where T : notnull where TNew : notnull =>
        (await resultFunctorTask).Type switch
        {
            ResultType<T>.Ok okResult =>
                new Functor<ResultType<TNew>>(new ResultType<TNew>.Ok(map(okResult.Value))),
            ResultType<T>.Error errorResult => new Functor<ResultType<TNew>>(
                new ResultType<TNew>.Error(errorResult.Message))
        };

    public static async Task<IFunctor<ResultType<T>>> MapErrorAsync<T>(
        this Task<IFunctor<ResultType<T>>> resultFunctorTask,
        Func<string, string> mapError)
        where T : notnull => (await resultFunctorTask).Type switch
    {
        ResultType<T>.Error errorResult => new Functor<ResultType<T>>(
            new ResultType<T>.Error(mapError(errorResult.Message))),
        ResultType<T>.Ok okResult => new Functor<ResultType<T>>(new ResultType<T>.Ok(okResult.Value))
    };

    public static async Task<IFunctor<ResultType<TNew>>>
        MatchAsync<T, TNew>(this Task<IFunctor<ResultType<T>>> resultFunctorTask, Func<T, TNew> matchOk,
            Func<string, string> matchError) where T : notnull
        where TNew : notnull
        => (await resultFunctorTask).Type switch
        {
            ResultType<T>.Ok okResult => new Functor<ResultType<TNew>>(
                new ResultType<TNew>.Ok(matchOk(okResult.Value))),
            ResultType<T>.Error errorResult => new Functor<ResultType<TNew>>(
                new ResultType<TNew>.Error(matchError(errorResult.Message)))
        };

    public static async Task<IFunctor<ResultType<TNew>>> MapOkAsync<T, TNew>(
        this Task<IFunctor<ResultType<T>>> resultFunctorTask,
        Func<T, CancellationToken, Task<TNew>> map, CancellationToken cancellationToken = default)
        where T : notnull where TNew : notnull =>
        (await resultFunctorTask).Type switch
        {
            ResultType<T>.Ok okResult =>
                new Functor<ResultType<TNew>>(
                    new ResultType<TNew>.Ok(await map(okResult.Value, cancellationToken))),
            ResultType<T>.Error errorResult => new Functor<ResultType<TNew>>(
                new ResultType<TNew>.Error(errorResult.Message))
        };

    public static async Task<IFunctor<ResultType<T>>> MapErrorAsync<T>(
        this Task<IFunctor<ResultType<T>>> resultFunctorTask,
        Func<string, CancellationToken, Task<string>> mapError, CancellationToken cancellationToken)
        where T : notnull => (await resultFunctorTask).Type switch
    {
        ResultType<T>.Error errorResult => new Functor<ResultType<T>>(
            new ResultType<T>.Error(await mapError(errorResult.Message, cancellationToken))),
        ResultType<T>.Ok okResult => new Functor<ResultType<T>>(new ResultType<T>.Ok(okResult.Value))
    };

    public static async Task<IFunctor<ResultType<TNew, TE>>> MapOkAsync<T, TNew, TE>(
        this Task<IFunctor<ResultType<T, TE>>> resultFunctorTask,
        Func<T, TNew> map) where T : notnull where TE : notnull where TNew : notnull =>
        (await resultFunctorTask).Type switch
        {
            ResultType<T, TE>.Ok okResult =>
                new Functor<ResultType<TNew, TE>>(new ResultType<TNew, TE>.Ok(map(okResult.Value))),
            ResultType<T, TE>.Error errorResult => new Functor<ResultType<TNew, TE>>(
                new ResultType<TNew, TE>.Error(errorResult.Err))
        };

    public static async Task<IFunctor<ResultType<T, TENew>>> MapErrorAsync<T, TE, TENew>(
        this Task<IFunctor<ResultType<T, TE>>> resultFunctorTask,
        Func<TE, TENew> mapError)
        where T : notnull where TE : notnull where TENew : notnull => (await resultFunctorTask).Type switch
    {
        ResultType<T, TE>.Error errorResult => new Functor<ResultType<T, TENew>>(
            new ResultType<T, TENew>.Error(mapError(errorResult.Err))),
        ResultType<T, TE>.Ok okResult => new Functor<ResultType<T, TENew>>(new ResultType<T, TENew>.Ok(okResult.Value))
    };

    public static async Task<IFunctor<ResultType<TNew, TENew>>>
        MatchAsync<T, TE, TNew, TENew>(this Task<IFunctor<ResultType<T, TE>>> resultFunctorTask, Func<T, TNew> matchOk,
            Func<TE, TENew> matchError) where T : notnull
        where TE : notnull
        where TNew : notnull
        where TENew : notnull => (await resultFunctorTask).Type switch
    {
        ResultType<T, TE>.Ok okResult => new Functor<ResultType<TNew, TENew>>(
            new ResultType<TNew, TENew>.Ok(matchOk(okResult.Value))),
        ResultType<T, TE>.Error errorResult => new Functor<ResultType<TNew, TENew>>(
            new ResultType<TNew, TENew>.Error(matchError(errorResult.Err)))
    };

    public static async Task<IFunctor<ResultType<TNew, TE>>> MapOkAsync<T, TNew, TE>(
        this Task<IFunctor<ResultType<T, TE>>> resultFunctorTask,
        Func<T, CancellationToken, Task<TNew>> map, CancellationToken cancellationToken = default)
        where T : notnull where TE : notnull where TNew : notnull =>
        (await resultFunctorTask).Type switch
        {
            ResultType<T, TE>.Ok okResult =>
                new Functor<ResultType<TNew, TE>>(
                    new ResultType<TNew, TE>.Ok(await map(okResult.Value, cancellationToken))),
            ResultType<T, TE>.Error errorResult => new Functor<ResultType<TNew, TE>>(
                new ResultType<TNew, TE>.Error(errorResult.Err))
        };

    public static async Task<IFunctor<ResultType<T, TENew>>> MapErrorAsync<T, TE, TENew>(
        this Task<IFunctor<ResultType<T, TE>>> resultFunctorTask,
        Func<TE, CancellationToken, Task<TENew>> mapError, CancellationToken cancellationToken)
        where T : notnull where TE : notnull where TENew : notnull => (await resultFunctorTask).Type switch
    {
        ResultType<T, TE>.Error errorResult => new Functor<ResultType<T, TENew>>(
            new ResultType<T, TENew>.Error(await mapError(errorResult.Err, cancellationToken))),
        ResultType<T, TE>.Ok okResult => new Functor<ResultType<T, TENew>>(new ResultType<T, TENew>.Ok(okResult.Value))
    };

    public static async Task<IFunctor<ResultType<TNew, TENew>>>
        MatchAsync<T, TE, TNew, TENew>(this Task<IFunctor<ResultType<T, TE>>> resultFunctorTask,
            Func<T, CancellationToken, Task<TNew>> matchOk,
            Func<TE, CancellationToken, Task<TENew>> matchError,
            CancellationToken cancellationToken) where T : notnull
        where TE : notnull
        where TNew : notnull
        where TENew : notnull => (await resultFunctorTask).Type switch
    {
        ResultType<T, TE>.Ok okResult => new Functor<ResultType<TNew, TENew>>(
            new ResultType<TNew, TENew>.Ok(await matchOk(okResult.Value, cancellationToken))),
        ResultType<T, TE>.Error errorResult => new Functor<ResultType<TNew, TENew>>(
            new ResultType<TNew, TENew>.Error(await matchError(errorResult.Err, cancellationToken)))
    };

    public static Task<IFunctor<ResultType<TNew, TE>>> MapOkAsync<T, TNew, TE>(
        this Task<IFunctor<ResultType<T, TE>>> resultFunctorTask,
        Func<T, Task<TNew>> map) where T : notnull where TE : notnull where TNew : notnull =>
        resultFunctorTask.MapOkAsync<T, TNew, TE>((value, _) => map(value));

    public static Task<IFunctor<ResultType<T, TENew>>> MapErrorAsync<T, TE, TENew>(
        this Task<IFunctor<ResultType<T, TE>>> resultFunctorTask,
        Func<TE, Task<TENew>> mapError)
        where T : notnull where TE : notnull where TENew : notnull =>
        resultFunctorTask.MapErrorAsync((error, _) => mapError(error), default);

    public static Task<IFunctor<ResultType<TNew, TENew>>>
        MatchAsync<T, TE, TNew, TENew>(this Task<IFunctor<ResultType<T, TE>>> resultFunctorTask,
            Func<T, Task<TNew>> matchOk,
            Func<TE, Task<TENew>> matchError,
            CancellationToken cancellationToken) where T : notnull
        where TE : notnull
        where TNew : notnull
        where TENew : notnull => resultFunctorTask.MatchAsync(
        (value, _) => matchOk(value),
        (error, _) => matchError(error), default);
}