using Monads.Traits;

namespace Monads.ResultMonad;

public static class ResultMonadExtensions
{
    public static IMonad<ResultType<TNew, TE>> Bind<T, TE, TNew>(
        this IMonad<ResultType<T, TE>> resultMonad,
        Func<T, IMonad<ResultType<TNew, TE>>> binder)
        where T : notnull where TE : notnull where TNew : notnull => resultMonad.Type switch
    {
        ResultType<T, TE>.Ok okResult => binder(okResult.Value),
        ResultType<T, TE>.Error errorResult => new Monad<ResultType<TNew, TE>>(
            new ResultType<TNew, TE>.Error(errorResult.Err))
    };

    public static async Task<IMonad<ResultType<TNew, TE>>> BindAsync<T, TE, TNew>(
        this Task<IMonad<ResultType<T, TE>>> resultMonadTask,
        Func<T, CancellationToken, Task<IMonad<ResultType<TNew, TE>>>> binder, CancellationToken cancellationToken)
        where T : notnull where TE : notnull where TNew : notnull => (await resultMonadTask).Type switch
    {
        ResultType<T, TE>.Ok okResult => await binder(okResult.Value, cancellationToken),
        ResultType<T, TE>.Error errorResult => new Monad<ResultType<TNew, TE>>(
            new ResultType<TNew, TE>.Error(errorResult.Err))
    };

    public static Task<IMonad<ResultType<TNew, TE>>> BindAsync<T, TE, TNew>(
        this Task<IMonad<ResultType<T, TE>>> resultMonadTask,
        Func<T, Task<IMonad<ResultType<TNew, TE>>>> fOk)
        where T : notnull where TE : notnull where TNew : notnull =>
        resultMonadTask.BindAsync((value, _) => fOk(value), default);
}