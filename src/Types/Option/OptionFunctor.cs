using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace DotNetCoreFunctional.Option;

public static partial class Option
{
    public static class Functor
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TNew> Map<T, TNew>(Option<T> option, Func<T, TNew> mapping)
            where T : notnull
            where TNew : notnull =>
            option switch
            {
                Some<T> some => Some(mapping(some.Value)),
                _ => None<TNew>(),
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TNew> Map<T1, T2, TNew>(
            Option<(T1, T2)> tupleOption,
            Func<T1, T2, TNew> mapper
        )
            where T1 : notnull
            where T2 : notnull
            where TNew : notnull =>
            Map(tupleOption, tuple => mapper(tuple.Item1, tuple.Item2));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TNew> Map<T1, T2, T3, TNew>(
            Option<(T1, T2, T3)> optionTuple,
            Func<T1, T2, T3, TNew> mapper
        )
            where T1 : notnull
            where T2 : notnull
            where T3 : notnull
            where TNew : notnull =>
            Map(optionTuple, tuple => mapper(tuple.Item1, tuple.Item2, tuple.Item3));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TNew> Map<T1, T2, T3, T4, TNew>(
            Option<(T1, T2, T3, T4)> optionTuple,
            Func<T1, T2, T3, T4, TNew> mapper
        )
            where T1 : notnull
            where T2 : notnull
            where T3 : notnull
            where T4 : notnull
            where TNew : notnull =>
            Map(
                optionTuple,
                tuple => mapper(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4)
            );

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TNew Match<T, TNew>(
            Option<T> option,
            Func<T, TNew> someMapping,
            Func<TNew> noneMapping
        )
            where T : notnull
            where TNew : notnull =>
            option switch
            {
                Some<T> some => new Some<TNew>(someMapping(some.Value)),
                _ => new Some<TNew>(noneMapping()),
            };
    }
}