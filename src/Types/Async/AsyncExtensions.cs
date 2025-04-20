namespace DotNetCoreFunctional.Async;

public static class AsyncExtensions
{
    public static Async<T> ToAsync<T>(this Task<T> task, CancellationToken cancellationToken)
        where T : notnull => Async.FromTask(task, cancellationToken);

    public static Async<T> ToAsync<T>(this Task<T> task)
        where T : notnull => Async.FromTask(task);

    public static Async<T> ToAsync<T>(this ValueTask<T> task, CancellationToken cancellationToken)
        where T : notnull => Async.FromValueTask(task, cancellationToken);

    public static Async<T> ToAsync<T>(this ValueTask<T> task)
        where T : notnull => Async.FromValueTask(task);

    public static Async<TNew> Map<T, TNew>(
        this Async<T> async,
        Func<T, CancellationToken, TNew> map
    )
        where T : notnull
        where TNew : notnull => Async.Map(async, map);

    public static Async<TNew> Map<T, TNew>(
        this Async<T> async,
        Func<T, CancellationToken, TNew> map,
        CancellationToken cancellationToken
    )
        where T : notnull
        where TNew : notnull => Async.Map(async, map, cancellationToken);

    public static Async<TNew> Map<T, TNew>(this Async<T> async, Func<T, TNew> map)
        where T : notnull
        where TNew : notnull => Async.Map(async, map);

    public static Async<TNew> Bind<T, TNew>(
        this Async<T> async,
        Func<T, CancellationToken, Async<TNew>> binder
    )
        where T : notnull
        where TNew : notnull => Async.Bind(async, binder);

    public static Async<TNew> Bind<T, TNew>(
        this Async<T> async,
        Func<T, CancellationToken, Async<TNew>> binder,
        CancellationToken cancellationToken
    )
        where T : notnull
        where TNew : notnull => Async.Bind(async, binder, cancellationToken);

    public static Async<TNew> Bind<T, TNew>(this Async<T> async, Func<T, Async<TNew>> binder)
        where T : notnull
        where TNew : notnull => Async.Bind(async, binder);

    public static Async<T> Flatten<T>(this Async<Async<T>> nestedAsync)
        where T : notnull => Async.Flatten(nestedAsync);

    public static Async<TNew> Apply<T, TNew>(
        this Async<T> async,
        Async<Func<T, CancellationToken, TNew>> wrappedMap,
        CancellationToken cancellationToken
    )
        where T : notnull
        where TNew : notnull => Async.Apply(async, wrappedMap, cancellationToken);

    public static Async<TNew> Apply<T, TNew>(
        this Async<T> async,
        Async<Func<T, CancellationToken, TNew>> wrappedMap
    )
        where T : notnull
        where TNew : notnull => Async.Apply(async, wrappedMap);

    public static Async<TNew> Apply<T, TNew>(this Async<T> async, Async<Func<T, TNew>> wrappedMap)
        where T : notnull
        where TNew : notnull => Async.Apply(async, wrappedMap);

    public static async Task DoAsync<T>(this Async<T> async, Func<T, Task> actionAsync)
        where T : notnull
    {
        await actionAsync(await async.ConfigureAwait()).ConfigureAwait(ConfigureAwaitOptions.None);
    }
}
