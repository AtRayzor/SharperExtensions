using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace SharperExtensions;

public static partial class Option
{
    /// <summary>
    /// Provides functional mapping operations for Option types, enabling transformation of Option values.
    /// </summary>
    /// <remarks>
    /// This static class contains extension methods for mapping Option types, allowing safe and functional transformations of optional values.
    /// </remarks>
    public static class Functor
    {
        /// <summary>
        /// Maps an Option of type <typeparamref name="T"/> to an Option of type <typeparamref name="TNew"/> using the provided mapping function.
        /// </summary>
        /// <typeparam name="T">The type of the source Option's value, which must be non-nullable.</typeparam>
        /// <typeparam name="TNew">The type of the resulting Option's value, which must be non-nullable.</typeparam>
        /// <param name="option">The source Option to be mapped.</param>
        /// <param name="mapping">A function that transforms the value of the source Option.</param>
        /// <returns>A new Option containing the mapped value, or None if the source Option is None.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TNew> Map<T, TNew>(Option<T> option, Func<T, TNew> mapping)
            where T : notnull
            where TNew : notnull =>
            option switch
            {
                { Type: OptionType.Some, Value: var value } =>
                    Some(mapping(value!)),
                _ => None<TNew>(),
            };

        /// <summary>
        /// Maps an Option of a 2-tuple to a new Option by applying a mapping function to the tuple's elements.
        /// </summary>
        /// <typeparam name="T1">The type of the first tuple element, which must be non-nullable.</typeparam>
        /// <typeparam name="T2">The type of the second tuple element, which must be non-nullable.</typeparam>
        /// <typeparam name="TNew">The type of the resulting Option's value, which must be non-nullable.</typeparam>
        /// <param name="tupleOption">The source Option containing a 2-tuple to be mapped.</param>
        /// <param name="mapper">A function that transforms the tuple's elements into a new value.</param>
        /// <returns>A new Option containing the mapped value, or None if the source Option is None.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<TNew> Map<T1, T2, TNew>(
            Option<(T1, T2)> tupleOption,
            Func<T1, T2, TNew> mapper
        )
            where T1 : notnull
            where T2 : notnull
            where TNew : notnull =>
            Map(tupleOption, tuple => mapper(tuple.Item1, tuple.Item2));

        /// <summary>
        /// Maps an Option of a 3-tuple to a new Option by applying a mapping function to the tuple's elements.
        /// </summary>
        /// <typeparam name="T1">The type of the first tuple element, which must be non-nullable.</typeparam>
        /// <typeparam name="T2">The type of the second tuple element, which must be non-nullable.</typeparam>
        /// <typeparam name="T3">The type of the third tuple element, which must be non-nullable.</typeparam>
        /// <typeparam name="TNew">The type of the resulting Option's value, which must be non-nullable.</typeparam>
        /// <param name="optionTuple">The source Option containing a 3-tuple to be mapped.</param>
        /// <param name="mapper">A function that transforms the tuple's elements into a new value.</param>
        /// <returns>A new Option containing the mapped value, or None if the source Option is None.</returns>
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

        /// <summary>
        /// Maps an Option of a 4-tuple to a new Option by applying a mapping function to the tuple's elements.
        /// </summary>
        /// <typeparam name="T1">The type of the first tuple element, which must be non-nullable.</typeparam>
        /// <typeparam name="T2">The type of the second tuple element, which must be non-nullable.</typeparam>
        /// <typeparam name="T3">The type of the third tuple element, which must be non-nullable.</typeparam>
        /// <typeparam name="T4">The type of the fourth tuple element, which must be non-nullable.</typeparam>
        /// <typeparam name="TNew">The type of the resulting Option's value, which must be non-nullable.</typeparam>
        /// <param name="optionTuple">The source Option containing a 4-tuple to be mapped.</param>
        /// <param name="mapper">A function that transforms the tuple's elements into a new value.</param>
        /// <returns>A new Option containing the mapped value, or None if the source Option is None.</returns>
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

        /// <summary>
        /// Matches an option by applying a mapping function based on its state.
        /// </summary>
        /// <typeparam name="T">The type of the option's value, constrained to non-nullable types.</typeparam>
        /// <typeparam name="TNew">The return type of the mapping functions.</typeparam>
        /// <param name="option">The option to match.</param>
        /// <param name="someMapping">A function to transform the option's value when it is Some.</param>
        /// <param name="noneMapping">A function to provide a default value when the option is None.</param>
        /// <returns>The result of either the someMapping or noneMapping function.</returns>
        /// <remarks>Provides a safe way to handle both Some and None cases of an option.</remarks>
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
                { Type: OptionType.Some, Value: var value } => someMapping(value!),
                _ => noneMapping(),
            };
    }
}