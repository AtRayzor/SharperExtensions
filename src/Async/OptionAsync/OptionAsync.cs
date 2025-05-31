using System.Runtime.CompilerServices;

namespace SharperExtensions.Async;

[AsyncMethodBuilder(typeof(OptionAsyncMethodBuilder<>))]
public readonly struct OptionAsync<T>
    where T : notnull
{
    internal Async<Option<T>> WrappedOption { get; }

    internal OptionAsync(
        Async<Option<T>> wrappedOption,
        CancellationToken token = default
    )
    {
        WrappedOption = wrappedOption;
    }

    internal OptionAsync(Option<T> option) : this(new Async<Option<T>>(option)) { }

    internal OptionAsync(T? value)
        : this(
            new Async<Option<T>>(
                value is not null ? Option<T>.Some(value) : Option<T>.None
            )
        ) { }

    public AsyncAwaiter<Option<T>> GetAwaiter() => WrappedOption.GetAwaiter();

    public static implicit operator OptionAsync<T>(Task<T?> task) =>
        OptionAsync.FromTask(task);

    public static implicit operator OptionAsync<T>(ValueTask<T?> valueTask) =>
        OptionAsync.FromValueTask(valueTask);

    public static implicit operator OptionAsync<T>(T? value) =>
        OptionAsync.Create(value);
}

public static class OptionAsync
{
    public static OptionAsync<T> Create<T>(T? value)
        where T : notnull => new(value);

    public static OptionAsync<T> Create<T>(Option<T> value)
        where T : notnull => new(value);

    public static OptionAsync<T> Lift<T>(Async<T> async)
        where T : notnull => new(async.Bind(value => Async.New(Option<T>.Some(value))));

    public static OptionAsync<T> FromTask<T>(Task<T?> task)
        where T : notnull
    {
        var wrappedOption = new Async<Option<T>>(
            () => task.Result,
            null
        );

        return new OptionAsync<T>(wrappedOption);
    }

    public static OptionAsync<T> FromValueTask<T>(ValueTask<T?> valueTask)
        where T : notnull
    {
        var wrappedOption = new Async<Option<T>>(
            () => valueTask.Result,
            null
        );

        return new OptionAsync<T>(wrappedOption);
    }

    public static OptionAsync<TNew> Map<T, TNew>(
        OptionAsync<T> optionAsync,
        Func<T, TNew> mapper
    )
        where T : notnull
        where TNew : notnull =>
        new(optionAsync.WrappedOption.Map(option => option.Map(mapper)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OptionAsync<TNew> Map<T1, T2, TNew>(
        OptionAsync<(T1, T2)> tupleOptionAsync,
        Func<T1, T2, TNew> mapper
    )
        where T1 : notnull
        where T2 : notnull
        where TNew : notnull =>
        Map(tupleOptionAsync, tuple => mapper(tuple.Item1, tuple.Item2));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OptionAsync<TNew> Map<T1, T2, T3, TNew>(
        OptionAsync<(T1, T2, T3)> tupleOptionAsync,
        Func<T1, T2, T3, TNew> mapper
    )
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where TNew : notnull =>
        Map(tupleOptionAsync, tuple => mapper(tuple.Item1, tuple.Item2, tuple.Item3));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OptionAsync<TNew> Map<T1, T2, T3, T4, TNew>(
        OptionAsync<(T1, T2, T3, T4)> tupleOptionAsync,
        Func<T1, T2, T3, T4, TNew> mapper
    )
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull
        where TNew : notnull =>
        Map(
            tupleOptionAsync,
            tuple => mapper(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4)
        );

    public static OptionAsync<TNew> Bind<T, TNew>(
        OptionAsync<T> optionAsync,
        Func<T, OptionAsync<TNew>> binder
    )
        where T : notnull
        where TNew : notnull =>
        new(
            optionAsync.WrappedOption.Bind(option =>
                option.Match(
                    value =>
                        binder(value).WrappedOption,
                    () => Async.New(Option<TNew>.None)
                )
            )
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OptionAsync<TNew> Bind<T1, T2, TNew>(
        OptionAsync<(T1, T2)> tupleOption,
        Func<T1, T2, OptionAsync<TNew>> mapper
    )
        where T1 : notnull
        where T2 : notnull
        where TNew : notnull =>
        Bind(tupleOption, tuple => mapper(tuple.Item1, tuple.Item2));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OptionAsync<TNew> Bind<T1, T2, T3, TNew>(
        OptionAsync<(T1, T2, T3)> optionTuple,
        Func<T1, T2, T3, OptionAsync<TNew>> mapper
    )
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where TNew : notnull =>
        Bind(optionTuple, tuple => mapper(tuple.Item1, tuple.Item2, tuple.Item3));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OptionAsync<TNew> Bind<T1, T2, T3, T4, TNew>(
        OptionAsync<(T1, T2, T3, T4)> optionTuple,
        Func<T1, T2, T3, T4, OptionAsync<TNew>> mapper
    )
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull
        where TNew : notnull =>
        Bind(
            optionTuple,
            tuple => mapper(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4)
        );

    public static OptionAsync<TNew> Apply<T, TNew>(
        OptionAsync<T> optionAsync,
        OptionAsync<Func<T, TNew>> wrappedMapper
    )
        where T : notnull
        where TNew : notnull =>
        Bind(wrappedMapper, mapper => Map(optionAsync, mapper));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OptionAsync<T> OrElseAsync<T>(params OptionAsync<T>[] optionAsyncs)
        where T : notnull
    {
        var wrappedOption = new Async<Option<T>>(
            () => Option.OrElse(
                optionAsyncs.Select(oa => oa.WrappedOption.Result).ToArray()
            ),
            null
        );

        return new OptionAsync<T>(wrappedOption);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OptionAsync<T> OrElseAsync<T>(
        params Func<OptionAsync<T>>[] optionAsyncFuncs
    )
        where T : notnull
    {
        var wrappedOption = new Async<Option<T>>(
            () => Option.OrElse(
                optionAsyncFuncs.Select(f => f().WrappedOption.Result).ToArray()
            ),
            null
        );

        return new OptionAsync<T>(wrappedOption);
    }

    public static class Unsafe
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<TResult> MatchAsync<T, TResult>(
            OptionAsync<T> optionAsync,
            Func<T, Task<TResult>> matchSomeAsync,
            Func<Task<TResult>> matchNoneAsync
        )
            where T : notnull
            where TResult : notnull
        {
            return await (await optionAsync)
                .Match(
                    value => matchSomeAsync(value),
                    () => matchNoneAsync()
                )
                .ConfigureAwait(ConfigureAwaitOptions.None);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask<T?> GetValueOrDefaultAsync<T>(
            OptionAsync<T> optionAsync
        )
            where T : notnull => (await optionAsync).ValueOrDefault;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task GetValueOrThrowAsync<T, TException>(
            OptionAsync<T> optionAsync
        )
            where T : notnull where TException : Exception =>
            (await optionAsync).GetValueOrThrow<T, TException>();

        public static async Task IfSomeAsync<T>(
            OptionAsync<T> optionAsync,
            Func<T, Task> asyncFunc
        )
            where T : notnull
        {
            if (!(await optionAsync).TryGetValue(out var value))
            {
                return;
            }

            await asyncFunc(value).ConfigureAwait(ConfigureAwaitOptions.None);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task IfNoneAsync<T>(
            OptionAsync<T> optionAsync,
            Func<Task> asyncFunc
        ) where T : notnull
        {
            if (!(await optionAsync).IsNone)
            {
                return;
            }

            await asyncFunc().ConfigureAwait(ConfigureAwaitOptions.None);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task DoAsync<T>(
            OptionAsync<T> optionAsync,
            Func<T, Task> someFunc,
            Func<Task> noneFunc
        ) where T : notnull
        {
            switch (await optionAsync)
            {
                case Some<T> { Value: var value }:
                {
                    await someFunc(value).ConfigureAwait(ConfigureAwaitOptions.None);

                    break;
                }
                case None<T>:
                {
                    await noneFunc().ConfigureAwait(ConfigureAwaitOptions.None);
                    break;
                }
            }
        }
    }
}