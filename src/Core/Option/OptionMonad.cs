using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace SharperExtensions;

public static partial class Option
{
    public static class Monad
    {
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TNew> Bind<T, TNew>(
            Option<T> option,
            Func<T, Option<TNew>> binder
        )
            where T : notnull
            where TNew : notnull =>
            option switch
            {
                Some<T> some => binder(some.Value),
                _ => new None<TNew>(),
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TNew> Bind<T1, T2, TNew>(
            (Option<T1>, Option<T2>) optionTuple,
            Func<T1, T2, TNew> mapper
        )
            where T1 : notnull
            where T2 : notnull
            where TNew : notnull =>
            optionTuple switch
            {
                (Some<T1> { Value: var value1 }, Some<T2> { Value: var value2 }) =>
                    Some(mapper(value1, value2)),
                _ => None<TNew>(),
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TNew> Bind<T1, T2, TNew>(
            Option<(T1, T2)> tupleOption,
            Func<T1, T2, Option<TNew>> mapper
        )
            where T1 : notnull
            where T2 : notnull
            where TNew : notnull =>
            Bind(tupleOption, tuple => mapper(tuple.Item1, tuple.Item2));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TNew> Bind<T1, T2, T3, TNew>(
            Option<(T1, T2, T3)> optionTuple,
            Func<T1, T2, T3, Option<TNew>> mapper
        )
            where T1 : notnull
            where T2 : notnull
            where T3 : notnull
            where TNew : notnull =>
            Bind(optionTuple, tuple => mapper(tuple.Item1, tuple.Item2, tuple.Item3));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TNew> Bind<T1, T2, T3, T4, TNew>(
            Option<(T1, T2, T3, T4)> optionTuple,
            Func<T1, T2, T3, T4, Option<TNew>> mapper
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
    }
}