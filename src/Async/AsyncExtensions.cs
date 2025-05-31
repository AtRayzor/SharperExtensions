namespace SharperExtensions.Async;

public static class AsyncExtensions
{
    extension<T>(Task<T> task)
        where T : notnull
    {
        public Async<T> ToAsync(CancellationToken token) =>
            Async<T>.FromTask(task, token);

        public Async<T> ToAsync() => Async<T>.FromTask(task);
    }

    extension<T>(ValueTask<T> valueTask)
        where T : notnull
    {
        public Async<T> ToAsync(CancellationToken token) =>
            Async<T>.FromValueTask(valueTask, token);

        public Async<T> ToAsync() => Async<T>.FromValueTask(valueTask);
    }

    extension<T>(Async<T> async)
        where T : notnull
    {
        public Async<TNew> Map<TNew>(Func<T, CancellationToken, TNew> mapper)
            where TNew : notnull => Async.Map(async, mapper);


        public Async<TNew> Map<TNew>(
            Func<T, CancellationToken, TNew> mapper,
            CancellationToken token
        ) where TNew : notnull => Async.Map(async, mapper, token);

        public Async<TNew> Map<TNew>(Func<T, TNew> mapper)
            where TNew : notnull => Async.Map(async, mapper);

        public Async<TNew> Bind<TNew>(Func<T, CancellationToken, Async<TNew>> binder)
            where TNew : notnull => Async.Bind(async, binder);


        public Async<TNew> Bind<TNew>(
            Func<T, CancellationToken, Async<TNew>> binder,
            CancellationToken token
        ) where TNew : notnull => Async.Bind(async, binder, token);

        public Async<TNew> Bind<TNew>(Func<T, Async<TNew>> binder)
            where TNew : notnull => Async.Bind(async, binder);


        public Async<TNew> Apply<TNew>(
            Async<Func<T, CancellationToken, TNew>> wrappedMapper
        )
            where TNew : notnull => Async.Apply(async, wrappedMapper);


        public Async<TNew> Apply<TNew>(
            Async<Func<T, CancellationToken, TNew>> wrappedMapper,
            CancellationToken token
        ) where TNew : notnull => Async.Apply(async, wrappedMapper, token);

        public Async<TNew> Apply<TNew>(Async<Func<T, TNew>> wrappedMapper)
            where TNew : notnull => Async.Apply(async, wrappedMapper);

        public Task DoAsync(
            Func<T, CancellationToken, Task> asyncFunc,
            CancellationToken token
        ) =>
            Async.DoAsync(async, asyncFunc, token);

        public Task DoAsync(Func<T, CancellationToken, Task> asyncFunc) =>
            Async.DoAsync(async, asyncFunc, async.State.Token);

        public Task DoAsync(Func<T, Task> asyncFunc) =>
            Async.DoAsync(
                async,
                (value, _) => asyncFunc(value),
                CancellationToken.None
            );
    }

    extension<T>(Async<Async<T>> nestedAsync)
        where T : notnull
    {
        public Async<T> Flatten() => Async.Flatten(nestedAsync);
    }
}