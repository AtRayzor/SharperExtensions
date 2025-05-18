using System.Runtime.CompilerServices;
using DotNetCoreFunctional.Option;

namespace DotNetCoreFunctional.Async;

[AsyncMethodBuilder(typeof(OptionAsyncMethodBuilder<>))]
public struct OptionAsync<T>
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
}

public static class OptionAsync
{
    public static OptionAsync<T> Create<T>(T? value)
        where T : notnull => new(value);

    public static OptionAsync<T> Create<T>(Option<T> value)
        where T : notnull => new(value);

    public static OptionAsync<T> Lift<T>(Async<T> async)
        where T : notnull => new(async.Bind(value => Async.New(Option<T>.Some(value))));

    public static OptionAsync<TNew> Map<T, TNew>(
        OptionAsync<T> optionAsync,
        Func<T, TNew> mapper
    )
        where T : notnull
        where TNew : notnull =>
        new(optionAsync.WrappedOption.Map(option => option.Map(mapper)));

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

    public static OptionAsync<TNew> Apply<T, TNew>(
        OptionAsync<T> optionAsync,
        OptionAsync<Func<T, TNew>> wrappedMapper
    )
        where T : notnull
        where TNew : notnull =>
        Bind(wrappedMapper, mapper => Map(optionAsync, mapper));

    public static class Unsafe
    {
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

        public static async ValueTask<T?> GetValueOrDefaultAsync<T>(
            OptionAsync<T> optionAsync
        )
            where T : notnull => (await optionAsync).ValueOrDefault;

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