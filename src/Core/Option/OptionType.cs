using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using SharperExtensions.Serializaion;

namespace SharperExtensions;

internal enum OptionType : byte
{
    Some,
    None
}

/// <summary>
/// Represents an optional value of type <typeparamref name="T"/> that can either contain a value or be empty.
/// </summary>
/// <typeparam name="T">The type of the optional value, constrained to non-nullable types.</typeparam>
/// <remarks>
/// This struct provides a type-safe way to handle optional values, with explicit Some and None states.
/// It supports JSON serialization and implements equality comparison.
/// </remarks>
[JsonConverter(typeof(OptionJsonConverterFactory))]
[DebuggerDisplay("{DebuggerDisplay, nq}")]
public readonly struct Option<T>() : IOption<T>, IEquatable<Option<T>>
    where T : notnull
{
    internal T? Value { get; } = default;

    internal OptionType Type
    {
        get =>
            (Value, field) is (null, OptionType.Some)
            or (_, not (OptionType.Some or OptionType.None))
                ? OptionType.None
                : field;
    } = OptionType.None;

    /// <summary>
    /// Determines whether the current option contains a value.
    /// </summary>
    /// <returns><see langword="true"/> if the option has a value; otherwise, <see langword="false"/>.</returns>
    [MemberNotNullWhen(true, nameof(Option<>.Value))]
    public bool IsSome => Type is OptionType.Some;

    /// <summary>
    /// Determines whether the current option does not contain a value.
    /// </summary>
    /// <returns><see langword="true"/> if the option is empty; otherwise, <see langword="false"/>.</returns>
    public bool IsNone => Type is OptionType.None;

    private Option(T value) : this()
    {
        Value = value;
        Type = OptionType.Some;
    }

    /// <summary>
    /// Gets a static <see cref="Option{T}"/> representing an empty option with no value.
    /// </summary>
    /// <returns>An <see cref="Option{T}"/> in the <see cref="OptionType.None"/> state.</returns>
    public static Option<T> None => new();

    public IEnumerator<T> GetEnumerator()
    {
        if (this is { Type: not OptionType.Some })
        {
            yield break;
        }

        Debug.Assert(
            Value is not null,
            "The value should never be null when the state is some"
        );

        yield return Value;
    }

    /// <summary>
    /// Creates and returns a new <see cref="Option{T}"/> instance with the specified value in the <see cref="OptionType.Some"/> state.
    /// </summary>
    /// <param name="value">The value to be wrapped in the option.</param>
    /// <returns>An <see cref="Option{T}"/> containing the specified value.</returns>
    /// <remarks>This method constructs an option with a non-null value, representing the presence of a value.</remarks>
    public static Option<T> Some(T value) => new(value);

    public static implicit operator Option<T>(T? value) => Option.Return(value);

    /// <inheritdoc />
    public bool Equals(Option<T> other)
    {
        return EqualityComparer<T?>.Default.Equals(Value, other.Value)
               && Type == other.Type;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is Option<T> other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Value, (int)Type);
    }

    public static bool operator ==(Option<T> left, Option<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Option<T> left, Option<T> right)
    {
        return !(left == right);
    }

    private string DebuggerDisplay
    {
        get
        {
            var typeName = $"Option<{typeof(T).Name}>";
            var value = !IsSome ? "None" : $"Some - {Value}";

            return $"{typeName}: {value}";
        }
    }
}

/// <summary>
/// Provides static utility methods and extension methods for working with <see cref="Option{T}"/> types.
/// </summary>
/// <remarks>
/// This partial class contains helper methods for creating, manipulating, and checking Option types,
/// which represent optional values that can either be present (Some) or absent (None).
/// </remarks>
public static partial class Option
{
    /// <summary>
    /// Converts a nullable value to an <see cref="Option{T}"/> instance, creating a <see cref="OptionType.Some"/> option if the value is not null, or a <see cref="OptionType.None"/> option otherwise.
    /// </summary>
    /// <typeparam name="T">The type of the value, constrained to non-nullable types.</typeparam>
    /// <param name="value">The nullable value to convert to an option.</param>
    /// <returns>An <see cref="Option{T}"/> representing the presence or absence of the value.</returns>
    /// <remarks>This method provides a convenient way to transform nullable values into Option types.</remarks>
    public static Option<T> Return<T>(T? value)
        where T : notnull => value is not null ? Some(value) : None<T>();

    /// <summary>
    /// Creates an <see cref="Option{T}"/> instance representing a Some value with the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the value, constrained to non-nullable types.</typeparam>
    /// <param name="value">The value to wrap in a Some option.</param>
    /// <returns>An <see cref="Option{T}"/> with <see cref="OptionType.Some"/> type containing the provided value.</returns>
    /// <remarks>This method provides a convenient way to create an Option with a present value.</remarks>
    public static Option<T> Some<T>(T value)
        where T : notnull => Option<T>.Some(value);

    /// <summary>
    /// Creates an <see cref="Option{T}"/> instance representing a None value for the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the option value, constrained to non-nullable types.</typeparam>
    /// <returns>An <see cref="Option{T}"/> with <see cref="OptionType.None"/> type.</returns>
    /// <remarks>This method provides a convenient way to create an Option with no value present.</remarks>
    public static Option<T> None<T>()
        where T : notnull => Option<T>.None;

    /// <summary>
    /// Creates an <see cref="Option{T}"/> by applying a factory function to a source value and converting the result.
    /// </summary>
    /// <typeparam name="TSource">The type of the source value.</typeparam>
    /// <typeparam name="T">The type of the resulting option value, constrained to non-nullable types.</typeparam>
    /// <param name="source">The source value to transform.</param>
    /// <param name="factory">A function that transforms the source value into a potential option value.</param>
    /// <returns>An <see cref="Option{T}"/> representing the result of the factory function.</returns>
    /// <remarks>This method provides a flexible way to create an Option by transforming a source value through a factory function.</remarks>
    public static Option<T> Create<TSource, T>(
        TSource? source,
        Func<TSource?, T?> factory
    )
        where T : notnull => Return(factory(source));

    /// <summary>
    /// Checks if an <see cref="Option{T}"/> instance represents a Some value.
    /// </summary>
    /// <typeparam name="T">The type of the option value, constrained to non-nullable types.</typeparam>
    /// <param name="option">The option to check for a Some value.</param>
    /// <returns>True if the option is a Some value, otherwise false.</returns>
    /// <remarks>This method provides a convenient way to determine if an Option contains a value.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSome<T>(Option<T> option)
        where T : notnull => option.IsSome;

    /// <summary>
    /// Checks if an <see cref="Option{T}"/> instance represents a None value.
    /// </summary>
    /// <typeparam name="T">The type of the option value, constrained to non-nullable types.</typeparam>
    /// <param name="option">The option to check for a None value.</param>
    /// <returns>True if the option is a None value, otherwise false.</returns>
    /// <remarks>This method provides a convenient way to determine if an Option does not contain a value.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNone<T>(Option<T> option)
        where T : notnull => option.IsNone;

    /// <summary>
    /// Returns the first option if it is a Some value, otherwise returns the fallback option.
    /// </summary>
    /// <typeparam name="T">The type of the option value, constrained to non-nullable types.</typeparam>
    /// <param name="option">The primary option to evaluate.</param>
    /// <param name="fallback">The fallback option to return if the primary option is None.</param>
    /// <returns>An <see cref="Option{T}"/> representing the first non-None option.</returns>
    /// <remarks>This method provides a convenient way to select between two options, preferring the first option if it has a value.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Or<T>(Option<T> option, Option<T> fallback)
        where T : notnull =>
        option switch
        {
            { Type: OptionType.Some, Value: var value } => value,
            _ => fallback,
        };

    /// <summary>
    /// Returns the first option if it is a Some value, otherwise returns the fallback value wrapped in a Some option.
    /// </summary>
    /// <typeparam name="T">The type of the option value, constrained to non-nullable types.</typeparam>
    /// <param name="option">The primary option to evaluate.</param>
    /// <param name="fallback">The fallback value to return if the primary option is None.</param>
    /// <returns>An <see cref="Option{T}"/> representing the first non-None option or the fallback value.</returns>
    /// <remarks>This method provides a convenient way to select between an option and a fallback value, wrapping the fallback in a Some option if needed.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Or<T>(Option<T> option, T fallback)
        where T : notnull =>
        option switch
        {
            { Type: OptionType.Some, Value: var value } => value,
            _ => Option<T>.Some(fallback),
        };

    /// <summary>
    /// Returns the first option if it is a Some value, otherwise returns the result of the fallback function wrapped in a Some option.
    /// </summary>
    /// <typeparam name="T">The type of the option value, constrained to non-nullable types.</typeparam>
    /// <param name="option">The primary option to evaluate.</param>
    /// <param name="fallbackFunc">A function that provides a fallback value if the primary option is None.</param>
    /// <returns>An <see cref="Option{T}"/> representing the first non-None option or the result of the fallback function.</returns>
    /// <remarks>This method provides a convenient way to select between an option and a lazily evaluated fallback value, wrapping the fallback in a Some option if needed.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Or<T>(Option<T> option, Func<T> fallbackFunc)
        where T : notnull =>
        option switch
        {
            { Type: OptionType.Some, Value: var value } => value,
            _ => Option<T>.Some(fallbackFunc()),
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> OrApply<T>(
        Option<T> option,
        Option<Func<T>> wrappedFallbackMapper
    )
        where T : notnull =>
        (option, wrappedFallbackMapper) switch
        {
            ({ IsSome: true }, _) => option,
            ({ IsNone: true }, { IsSome: true, Value: var fallbackFunc }) =>
                Option<T>.Some(fallbackFunc!()),
            _ => Option<T>.None
        };

    /// <summary>
    /// Returns the value of an <see cref="Option{T}"/> if it is a Some value, otherwise returns the provided fallback value.
    /// </summary>
    /// <typeparam name="T">The type of the option value, constrained to non-nullable types.</typeparam>
    /// <param name="option">The option to extract the value from.</param>
    /// <param name="fallback">The default value to return if the option is None.</param>
    /// <returns>The option's value if it is Some, otherwise the fallback value.</returns>
    /// <remarks>This method provides a convenient way to safely extract a value from an Option or use a default.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetValueOr<T>(Option<T> option, T fallback)
        where T : notnull =>
        option switch
        {
            { Type: OptionType.Some, Value: var value } => value!,
            _ => fallback,
        };

    /// <summary>
    /// Combines two options into a single option containing a tuple of their values if both options are Some.
    /// </summary>
    /// <typeparam name="T1">The type of the first option value, constrained to non-nullable types.</typeparam>
    /// <typeparam name="T2">The type of the second option value, constrained to non-nullable types.</typeparam>
    /// <param name="option1">The first option to combine.</param>
    /// <param name="option2">The second option to combine.</param>
    /// <returns>An <see cref="Option{T}"/> containing a tuple of values if both input options are Some, otherwise None.</returns>
    /// <remarks>This method provides a way to combine multiple options, returning a Some value only if all input options have values.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<(T1, T2)> Combine<T1, T2>(
        Option<T1> option1,
        Option<T2> option2
    )
        where T1 : notnull
        where T2 : notnull => (option1, option2)
        is (
        { Type: OptionType.Some, Value: var value1 },
        { Type: OptionType.Some, Value: var value2 }
        )
            ? Option<(T1, T2)>.Some((value1!, value2!))
            : Option<(T1, T2)>.None;

    /// <summary>
    /// Combines three options into a single option containing a tuple of their values if all options are Some.
    /// </summary>
    /// <typeparam name="T1">The type of the first option value, constrained to non-nullable types.</typeparam>
    /// <typeparam name="T2">The type of the second option value, constrained to non-nullable types.</typeparam>
    /// <typeparam name="T3">The type of the third option value, constrained to non-nullable types.</typeparam>
    /// <param name="option1">The first option to combine.</param>
    /// <param name="option2">The second option to combine.</param>
    /// <param name="option3">The third option to combine.</param>
    /// <returns>An <see cref="Option{T}"/> containing a tuple of values if all input options are Some, otherwise None.</returns>
    /// <remarks>This method provides a way to combine multiple options, returning a Some value only if all input options have values.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<(T1, T2, T3)> Combine<T1, T2, T3>(
        Option<T1> option1,
        Option<T2> option2,
        Option<T3> option3
    )
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull => (option1, option2, option3)
        is (
        { Type: OptionType.Some, Value: var value1 },
        { Type: OptionType.Some, Value: var value2 },
        { Type: OptionType.Some, Value: var value3 }
        )
            ? Option<(T1, T2, T3)>.Some((value1!, value2!, value3!))
            : Option<(T1, T2, T3)>.None;

    public static Option<(T1, T2, T3)> Combine<T1, T2, T3>(
        Option<(T1, T2)> tuple,
        Option<T3> option3
    )
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull => (tuple, option3)
        is (
        { Type: OptionType.Some, Value: var (value1, value2) },
        { Type: OptionType.Some, Value: var value3 }
        )
            ? Option<(T1, T2, T3)>.Some((value1, value2, value3!))
            : Option<(T1, T2, T3)>.None;

    /// <summary>
    /// Combines four options into a single option containing a tuple of their values if all options are Some.
    /// </summary>
    /// <typeparam name="T1">The type of the first option value, constrained to non-nullable types.</typeparam>
    /// <typeparam name="T2">The type of the second option value, constrained to non-nullable types.</typeparam>
    /// <typeparam name="T3">The type of the third option value, constrained to non-nullable types.</typeparam>
    /// <typeparam name="T4">The type of the fourth option value, constrained to non-nullable types.</typeparam>
    /// <param name="option1">The first option to combine.</param>
    /// <param name="option2">The second option to combine.</param>
    /// <param name="option3">The third option to combine.</param>
    /// <param name="option4">The fourth option to combine.</param>
    /// <returns>An <see cref="Option{T}"/> containing a tuple of values if all input options are Some, otherwise None.</returns>
    /// <remarks>This method provides a way to combine multiple options, returning a Some value only if all input options have values.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<(T1, T2, T3, T4)> Combine<T1, T2, T3, T4>(
        Option<T1> option1,
        Option<T2> option2,
        Option<T3> option3,
        Option<T4> option4
    )
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull => (option1, option2, option3, option4)
        is (
        { Type: OptionType.Some, Value: var value1 },
        { Type: OptionType.Some, Value: var value2 },
        { Type: OptionType.Some, Value: var value3 },
        { Type: OptionType.Some, Value: var value4 }
        )
            ? Option<(T1, T2, T3, T4)>.Some((value1!, value2!, value3!, value4!))
            : Option<(T1, T2, T3, T4)>.None;

    /// <summary>
    /// Combines an option containing a tuple of three values with a fourth option into a single option containing a tuple of four values if all options are Some.
    /// </summary>
    /// <typeparam name="T1">The type of the first value, constrained to non-nullable types.</typeparam>
    /// <typeparam name="T2">The type of the second value, constrained to non-nullable types.</typeparam>
    /// <typeparam name="T3">The type of the third value, constrained to non-nullable types.</typeparam>
    /// <typeparam name="T4">The type of the fourth value, constrained to non-nullable types.</typeparam>
    /// <param name="tuple">An option containing a tuple of three values.</param>
    /// <param name="option3">An option containing the fourth value.</param>
    /// <returns>An <see cref="Option{T}"/> containing a tuple of four values if all input options are Some, otherwise None.</returns>
    /// <remarks>This method provides a way to combine an existing three-value tuple option with a fourth option, returning a Some value only if all input options have values.</remarks>
    public static Option<(T1, T2, T3, T4)> Combine<T1, T2, T3, T4>(
        Option<(T1, T2, T3)> tuple,
        Option<T4> option3
    )
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull => (tuple, option3)
        is (
        { Type: OptionType.Some, Value: var (value1, value2, value3) },
        { Type: OptionType.Some, Value: var value4 }
        )
            ? Option<(T1, T2, T3, T4)>.Some((value1, value2, value3, value4!))
            : Option<(T1, T2, T3, T4)>.None;

    /// <summary>
    /// Attempts to cast an option of one type to an option of another type.
    /// </summary>
    /// <typeparam name="T">The source type of the option, constrained to non-nullable types.</typeparam>
    /// <typeparam name="TNew">The target type to cast to, constrained to non-nullable types.</typeparam>
    /// <param name="option">The option to be cast.</param>
    /// <returns>An <see cref="Option{TNew}"/> containing the cast value if successful, otherwise None.</returns>
    /// <remarks>This method provides a safe way to convert an option to a different type, handling potential casting failures by returning None.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<TNew> Cast<T, TNew>(Option<T> option)
        where T : notnull
        where TNew : notnull
    {
        try
        {
            if (!option.IsSome)
            {
                return Option<TNew>.None;
            }

            return (TNew)(object)option.Value;
        }
        catch (Exception)
        {
            return Option<TNew>.None;
        }
    }

    /// <summary>
    /// Attempts to cast a value to an option of a different type.
    /// </summary>
    /// <typeparam name="T">The source type of the value, constrained to non-nullable types.</typeparam>
    /// <typeparam name="TNew">The target type to cast to, constrained to non-nullable types.</typeparam>
    /// <param name="value">The value to be cast.</param>
    /// <returns>An <see cref="Option{TNew}"/> containing the cast value if successful, otherwise None.</returns>
    /// <remarks>This method provides a safe way to convert a value to a different type within an Option context.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<TNew> CastToOption<T, TNew>(T? value)
        where T : notnull
        where TNew : notnull => Cast<T, TNew>(Return(value));

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

    [Obsolete(
        "Operations involving option collections are located in the options assembly."
    )]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> OrElse<T>(params Option<T>[] options)
        where T : notnull
    {
        foreach (var opt in options)
        {
            if (opt.Type is not OptionType.Some)
            {
                continue;
            }

            return opt.Value;
        }

        return Option<T>.None;
    }

    [Obsolete(
        "Operations involving option collections are located in the options assembly."
    )]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> OrElse<T>(params Func<Option<T>>[] optionFuncs)
        where T : notnull
    {
        foreach (var func in optionFuncs)
        {
            if (func() is not { Type: OptionType.Some } option)
            {
                continue;
            }

            return option;
        }

        return Option<T>.None;
    }

    /// <summary>
    /// Provides unsafe operations and extensions for the Option type.
    /// </summary>
    public static partial class Unsafe
    {
        /// <summary>
        /// Retrieves the value of an Option<T> if it is in the Some state, otherwise returns the default value.
        /// </summary>
        /// <typeparam name="T">The type of value contained in the Option, which must be a non-null type.</typeparam>
        /// <param name="option">The Option<T> to extract a value from.</param>
        /// <returns>The value of the Option<T> if it is in the Some state; otherwise, the default value of T.</returns>
        /// <remarks>
        /// This method provides a safe way to extract a value from an Option<T> without throwing an exception if the option is empty.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? GetValueOrDefault<T>(Option<T> option)
            where T : notnull
        {
            return option switch
            {
                { Type: OptionType.Some, Value: var value } => value!,
                _ => default,
            };
        }

        /// <summary>
        /// Attempts to retrieve the value from an Option<T> if it is in the Some state.
        /// </summary>
        /// <typeparam name="T">The type of value contained in the Option, which must be a non-null type.</typeparam>
        /// <param name="option">The Option<T> to attempt to extract a value from.</param>
        /// <param name="value">When this method returns true, contains the value of the Option<T>; otherwise, contains the default value of T.</param>
        /// <returns>true if the Option<T> is in the Some state and a value was successfully extracted; otherwise, false.</returns>
        /// <remarks>
        /// This method provides a safe way to extract a value from an Option<T> without throwing an exception if the option is empty.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetValue<T>(
            Option<T> option,
            [NotNullWhen(true)] out T? value
        )
            where T : notnull
        {
            value = default;

            if (option is not { Type: OptionType.Some, Value: var val })
            {
                return false;
            }

            value = val!;

            return true;
        }

        /// <summary>
        /// Executes an action if the Option<T> is in the Some state.
        /// </summary>
        /// <typeparam name="T">The type of value contained in the Option, which must be a non-null type.</typeparam>
        /// <param name="option">The Option<T> to inspect.</param>
        /// <param name="action">The action to execute with the contained value.</param>
        /// <remarks>
        /// This method provides a convenient way to perform an action when an Option<T> contains a value.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IfSome<T>(Option<T> option, Action<T> action)
            where T : notnull
        {
            if (!TryGetValue(option, out var value))
            {
                return;
            }

            action(value);
        }

        /// <summary>
        /// Executes an action if the Option<T> is in the None state.
        /// </summary>
        /// <typeparam name="T">The type of value contained in the Option, which must be a non-null type.</typeparam>
        /// <param name="option">The Option<T> to inspect.</param>
        /// <param name="action">The action to execute if the option is empty.</param>
        /// <remarks>
        /// This method provides a convenient way to perform an action when an Option<T> has no value.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IfNone<T>(Option<T> option, Action action)
            where T : notnull
        {
            if (!option.IsNone)
            {
                return;
            }

            action();
        }

        /// <summary>
        /// Executes an action based on the state of an Option<T>, providing separate actions for Some and None cases.
        /// </summary>
        /// <typeparam name="T">The type of value contained in the Option, which must be a non-null type.</typeparam>
        /// <param name="option">The Option<T> to inspect.</param>
        /// <param name="someAction">The action to execute if the option contains a value.</param>
        /// <param name="noneAction">The action to execute if the option is empty.</param>
        /// <remarks>
        /// This method allows pattern matching-like behavior for handling Option<T> instances with separate logic for Some and None states.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Do<T>(
            Option<T> option,
            Action<T> someAction,
            Action noneAction
        )
            where T : notnull
        {
            switch (option)
            {
                case { Type: OptionType.Some, Value: var value }:
                {
                    someAction(value!);
                    break;
                }
                case { Type: OptionType.None }:
                {
                    noneAction();
                    break;
                }
            }
        }
    }
}