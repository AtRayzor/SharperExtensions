using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace DotNetCoreFunctional.Option;

public static partial class Option
{
    public static class Monad
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TNew> Bind<T, TNew>(Option<T> option, Func<T, Option<TNew>> binder)
            where T : notnull
            where TNew : notnull =>
            option switch
            {
                Some<T> some => binder(some.Value),
                _ => new None<TNew>(),
            };

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<T> Flatten<T>(Option<Option<T>> wrappedOption)
            where T : notnull
        {
            return wrappedOption switch
            {
                Some<Option<T>> wrapped => wrapped.Value,
                _ => new None<T>(),
            };
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Option<TNew>> BindAsync<T, TNew>(
            Task<Option<T>> optionTask,
            Func<T, CancellationToken, Task<Option<TNew>>> binder,
            CancellationToken cancellationToken
        )
            where T : notnull
            where TNew : notnull =>
            await optionTask switch
            {
                Some<T> some => await binder(some.Value, cancellationToken),
                _ => new None<TNew>(),
            };

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Option<TNew>> BindAsync<T, TNew>(
            Task<Option<T>> optionTask,
            Func<T, Task<Option<TNew>>> binder
        )
            where T : notnull
            where TNew : notnull =>
            await optionTask switch
            {
                Some<T> some => await binder(some.Value),
                _ => new None<TNew>(),
            };
    }
}

public static class OptionMonad
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<TNew> Bind<T, TNew>(this Option<T> option, Func<T, Option<TNew>> binder)
        where T : notnull
        where TNew : notnull => Option.Monad.Bind(option, binder);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Flatten<T>(this Option<Option<T>> wrappedOption)
        where T : notnull => Option.Monad.Flatten(wrappedOption);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Option<TNew>> BindAsync<T, TNew>(
        this Task<Option<T>> optionTask,
        Func<T, CancellationToken, Task<Option<TNew>>> binder,
        CancellationToken cancellationToken
    )
        where T : notnull
        where TNew : notnull => Option.Monad.BindAsync(optionTask, binder, cancellationToken);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Option<TNew>> BindAsync<T, TNew>(
        this Task<Option<T>> optionTask,
        Func<T, Task<Option<TNew>>> binder
    )
        where T : notnull
        where TNew : notnull => Option.Monad.BindAsync(optionTask, binder);
}
