using Monads.Traits;

namespace Monads.ResultMonad;

public static class ResultMonadExtensions
{
    public static IMonad<ResultType<TNew, TE>> BindOk<T, TE, TNew>(this IMonad<ResultType<T, TE>> resultMonad,
        Func<T, IMonad<ResultType<TNew, TE>>> fOk)
        where T : notnull where TE : notnull where TNew : notnull => resultMonad.Type switch
    {
        ResultType<T, TE>.Ok okResult => fOk(okResult.Value),
        ResultType<T, TE>.Error errorResult => new Monad<ResultType<TNew, TE>>(
            new ResultType<TNew, TE>.Error(errorResult.Err))
    };

    public static IMonad<ResultType<T, TENew>> BindError<T, TE, TENew>(this IMonad<ResultType<T, TE>> resultMonad,
        Func<TE, IMonad<ResultType<T, TENew>>> fError)
        where T : notnull where TE : notnull where TENew : notnull => resultMonad.Type switch
    {
        ResultType<T, TE>.Error errorResult => fError(errorResult.Err),
        ResultType<T, TE>.Ok okResult => new Monad<ResultType<T, TENew>>(
            new ResultType<T, TENew>.Ok(okResult.Value))
    };

    public static IMonad<ResultType<TNew, TENew>> Match<T, TE, TNew, TENew>(this IMonad<ResultType<T, TE>> resultMonad,
        Func<T, IMonad<ResultType<TNew, TENew>>> matchOk, Func<TE, IMonad<ResultType<TNew, TENew>>> matchError
    )
        where T : notnull where TE : notnull where TNew : notnull where TENew : notnull => resultMonad.Type switch
    {
        ResultType<T, TE>.Ok okResult => matchOk(okResult.Value),
        ResultType<T, TE>.Error errorResult => matchError(errorResult.Err)
    };

    public static async Task<IMonad<ResultType<TNew, TE>>> BindOkAsync<T, TE, TNew>(
        this Task<IMonad<ResultType<T, TE>>> resultMonadTask,
        Func<T, CancellationToken, Task<IMonad<ResultType<TNew, TE>>>> fOk, CancellationToken cancellationToken)
        where T : notnull where TE : notnull where TNew : notnull => (await resultMonadTask).Type switch
    {
        ResultType<T, TE>.Ok okResult => await fOk(okResult.Value, cancellationToken),
        ResultType<T, TE>.Error errorResult => new Monad<ResultType<TNew, TE>>(
            new ResultType<TNew, TE>.Error(errorResult.Err))
    };

    public static async Task<IMonad<ResultType<T, TENew>>> BindErrorAsync<T, TE, TENew>(
        this Task<IMonad<ResultType<T, TE>>> resultMonadTask,
        Func<TE, CancellationToken, Task<IMonad<ResultType<T, TENew>>>> fError, CancellationToken cancellationToken)
        where T : notnull where TE : notnull where TENew : notnull => (await resultMonadTask).Type switch
    {
        ResultType<T, TE>.Error errorResult => await fError(errorResult.Err, cancellationToken),
        ResultType<T, TE>.Ok okResult => new Monad<ResultType<T, TENew>>(
            new ResultType<T, TENew>.Ok(okResult.Value))
    };

    public static async Task<IMonad<ResultType<TNew, TENew>>> MatchAsync<T, TE, TNew, TENew>(
        this Task<IMonad<ResultType<T, TE>>> resultMonadTask,
        Func<T, CancellationToken, Task<IMonad<ResultType<TNew, TENew>>>> matchOk,
        Func<TE, CancellationToken, Task<IMonad<ResultType<TNew, TENew>>>> matchError,
        CancellationToken cancellationToken
    )
        where T : notnull where TE : notnull where TNew : notnull where TENew : notnull =>
        (await resultMonadTask).Type switch
        {
            ResultType<T, TE>.Ok okResult => await matchOk(okResult.Value, cancellationToken),
            ResultType<T, TE>.Error errorResult => await matchError(errorResult.Err, cancellationToken)
        };

    public static Task<IMonad<ResultType<TNew, TE>>> BindOkAsync<T, TE, TNew>(
        this Task<IMonad<ResultType<T, TE>>> resultMonadTask,
        Func<T, Task<IMonad<ResultType<TNew, TE>>>> fOk)
        where T : notnull where TE : notnull where TNew : notnull =>
        resultMonadTask.BindOkAsync((value, _) => fOk(value), default);

    public static Task<IMonad<ResultType<T, TENew>>> BindErrorAsync<T, TE, TENew>(
        this Task<IMonad<ResultType<T, TE>>> resultMonadTask,
        Func<TE, Task<IMonad<ResultType<T, TENew>>>> fError)
        where T : notnull where TE : notnull where TENew : notnull =>
        resultMonadTask.BindErrorAsync((error, _) => fError(error), default);

    public static Task<IMonad<ResultType<TNew, TENew>>> MatchAsync<T, TE, TNew, TENew>(
        this Task<IMonad<ResultType<T, TE>>> resultMonadTask,
        Func<T, Task<IMonad<ResultType<TNew, TENew>>>> matchOk,
        Func<TE, Task<IMonad<ResultType<TNew, TENew>>>> matchError
    )
        where T : notnull where TE : notnull where TNew : notnull where TENew : notnull =>
        resultMonadTask.MatchAsync((value, _) => matchOk(value), (error, _) => matchError(error), default);
}