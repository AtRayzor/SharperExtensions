using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NetFunctional.Types;

public static partial class Option
{
    public static class Functor
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TNew> Map<T, TNew>(Option<T> option, Func<T, TNew> mapping)
            where T : notnull
            where TNew : notnull
        {
            return option switch
            {
                Some<T> some => Some(mapping(some.Value)),
                _ => None<TNew>()
            };
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TNew> Match<T, TNew>(
            Option<T> option,
            Func<T, TNew> someMapping,
            Func<TNew> noneMapping
        )
            where T : notnull
            where TNew : notnull
        {
            return option switch
            {
                Some<T> some => new Some<TNew>(someMapping(some.Value)),
                _ => new Some<TNew>(noneMapping())
            };
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Option<TNew>> MapAsync<T, TNew>(
            Task<Option<T>> optionTask,
            Func<T, TNew> mapping
        )
            where T : notnull
            where TNew : notnull
        {
            return Map(await optionTask, mapping);
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Option<TNew>> MatchAsync<T, TNew>(
            Task<Option<T>> optionTask,
            Func<T, TNew> someMapping,
            Func<TNew> noneMapping
        )
            where T : notnull
            where TNew : notnull
        {
            return Match(await optionTask, someMapping, noneMapping);
        }
    }
}

public static class OptionFunctor
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<TNew> Map<T, TNew>(this Option<T> option, Func<T, TNew> mapping)
        where T : notnull
        where TNew : notnull
    {
        return Option.Functor.Map(option, mapping);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<TNew> Match<T, TNew>(
        this Option<T> option,
        Func<T, TNew> someMapping,
        Func<TNew> noneMapping
    )
        where T : notnull
        where TNew : notnull
    {
        return Option.Functor.Match(option, someMapping, noneMapping);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Option<TNew>> MapAsync<T, TNew>(
        this Task<Option<T>> optionTask,
        Func<T, TNew> mapping
    )
        where T : notnull
        where TNew : notnull
    {
        return Option.Functor.MapAsync(optionTask, mapping);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<Option<TNew>> MatchAsync<T, TNew>(
        this Task<Option<T>> optionTask,
        Func<T, TNew> someMapping,
        Func<TNew> noneMapping
    )
        where T : notnull
        where TNew : notnull
    {
        return Option.Functor.MatchAsync(optionTask, someMapping, noneMapping);
    }
}
