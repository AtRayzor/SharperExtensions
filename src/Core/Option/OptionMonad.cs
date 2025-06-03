using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace SharperExtensions;

public static partial class Option
{
    /// <summary>
    /// Provides monadic operations for the Option type, enabling functional composition and chaining of optional values.
    /// </summary>
    /// <remarks>
    /// This static class contains extension methods and utility functions for working with Option types in a functional programming style.
    /// </remarks>
    public static class Monad
    {
        /// <summary>
        /// Binds an Option of type T to a new Option of type TNew using the provided binder function.
        /// </summary>
        /// <typeparam name="T">The type of the source Option's value, constrained to be non-null.</typeparam>
        /// <typeparam name="TNew">The type of the resulting Option's value, constrained to be non-null.</typeparam>
        /// <param name="option">The source Option to bind.</param>
        /// <param name="binder">A function that transforms the Option's value into a new Option.</param>
        /// <returns>A new Option of type TNew, which is either Some or None based on the binder function's result.</returns>
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
                { Type: OptionType.Some, Value: var value } => binder(value!),
                _ => None<TNew>(),
            };

        /// <summary>
        /// Binds two Options together by applying a mapping function when both Options are Some.
        /// </summary>
        /// <typeparam name="T1">The type of the first Option's value, constrained to be non-null.</typeparam>
        /// <typeparam name="T2">The type of the second Option's value, constrained to be non-null.</typeparam>
        /// <typeparam name="TNew">The type of the resulting value, constrained to be non-null.</typeparam>
        /// <param name="optionTuple">A tuple of two Options to be bound together.</param>
        /// <param name="mapper">A function that transforms the values of both Options into a new value.</param>
        /// <returns>An Option containing the result of the mapper function if both input Options are Some, otherwise None.</returns>
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
                ({ Type: OptionType.Some, Value: var value1 }, {
                        Type: OptionType.Some, Value: var value2
                    }) =>
                    Some(mapper(value1!, value2!)),
                _ => None<TNew>(),
            };

        /// <summary>
        /// Binds an Option containing a tuple of two values to a new Option using a mapper function.
        /// </summary>
        /// <typeparam name="T1">The type of the first tuple element, constrained to be non-null.</typeparam>
        /// <typeparam name="T2">The type of the second tuple element, constrained to be non-null.</typeparam>
        /// <typeparam name="TNew">The type of the resulting Option's value, constrained to be non-null.</typeparam>
        /// <param name="tupleOption">An Option containing a tuple of two values.</param>
        /// <param name="mapper">A function that transforms the tuple's values into a new Option.</param>
        /// <returns>An Option containing the result of the mapper function if the input Option is Some, otherwise None.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TNew> Bind<T1, T2, TNew>(
            Option<(T1, T2)> tupleOption,
            Func<T1, T2, Option<TNew>> mapper
        )
            where T1 : notnull
            where T2 : notnull
            where TNew : notnull =>
            Bind(tupleOption, tuple => mapper(tuple.Item1, tuple.Item2));

        /// <summary>
        /// Binds an Option containing a tuple of three values to a new Option using a mapper function.
        /// </summary>
        /// <typeparam name="T1">The type of the first tuple element, constrained to be non-null.</typeparam>
        /// <typeparam name="T2">The type of the second tuple element, constrained to be non-null.</typeparam>
        /// <typeparam name="T3">The type of the third tuple element, constrained to be non-null.</typeparam>
        /// <typeparam name="TNew">The type of the resulting Option's value, constrained to be non-null.</typeparam>
        /// <param name="optionTuple">An Option containing a tuple of three values.</param>
        /// <param name="mapper">A function that transforms the tuple's values into a new Option.</param>
        /// <returns>An Option containing the result of the mapper function if the input Option is Some, otherwise None.</returns>
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

        /// <summary>
        /// Binds an Option containing a tuple of four values to a new Option using a mapper function.
        /// </summary>
        /// <typeparam name="T1">The type of the first tuple element, constrained to be non-null.</typeparam>
        /// <typeparam name="T2">The type of the second tuple element, constrained to be non-null.</typeparam>
        /// <typeparam name="T3">The type of the third tuple element, constrained to be non-null.</typeparam>
        /// <typeparam name="T4">The type of the fourth tuple element, constrained to be non-null.</typeparam>
        /// <typeparam name="TNew">The type of the resulting Option's value, constrained to be non-null.</typeparam>
        /// <param name="optionTuple">An Option containing a tuple of four values.</param>
        /// <param name="mapper">A function that transforms the tuple's values into a new Option.</param>
        /// <returns>An Option containing the result of the mapper function if the input Option is Some, otherwise None.</returns>
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
        
        /// <summary>
        /// Flattens a nested Option, returning the inner Option if the outer Option is Some, otherwise returning None.
        /// </summary>
        /// <typeparam name="T">The type of the Option's value, constrained to be non-null.</typeparam>
        /// <param name="wrappedOption">An Option containing another Option.</param>
        /// <returns>The inner Option if the outer Option is Some, otherwise None.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<T> Flatten<T>(Option<Option<T>> wrappedOption)
            where T : notnull
        {
            return wrappedOption switch
            {
                { Type: OptionType.Some, Value: var inner } => inner,
                _ => None<T>(),
            };
        }
    }
}