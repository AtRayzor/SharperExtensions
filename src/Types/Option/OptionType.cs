using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using DotNetCoreFunctional.Option.Serializaion;
using DotNetCoreFunctional.UnionTypes;

namespace DotNetCoreFunctional.Option;

[JsonConverter(typeof(OptionJsonConverterFactory))]
[Closed]
public abstract record Option<T> : IOption<T>
    where T : notnull
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly object Lock = new();
    private static None<T>? _none;

    public static Option<T> None
    {
        get
        {
            lock (Lock)
            {
                return _none ??= new None<T>();
            }
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        if (this is Some<T> some)
            yield return some.Value;
    }

    public static Option<T> Some(T value)
    {
        return new Some<T>(value);
    }

    public static implicit operator Option<T>(T? value) => Option.Return(value);
}

[JsonConverter(typeof(OptionJsonConverterFactory))]
public record Some<T>(T Value) : Option<T>
    where T : notnull
{
    public static implicit operator T(Some<T> some) => some.Value;
}

[JsonConverter(typeof(OptionJsonConverterFactory))]
public record None<T> : Option<T>
    where T : notnull;

public static partial class Option
{
    public static Option<T> Return<T>(T? value)
        where T : notnull => value is not null ? Some(value) : None<T>();

    public static Option<T> Some<T>(T value)
        where T : notnull => new Some<T>(value);

    public static Option<T> None<T>()
        where T : notnull => new None<T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSome<T>(Option<T> option)
        where T : notnull => option is Some<T>;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNone<T>(Option<T> option)
        where T : notnull => option is None<T>;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Or<T>(Option<T> option, Option<T> fallback)
        where T : notnull =>
        option switch
        {
            Some<T> some => some,
            _ => fallback,
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Or<T>(Option<T> option, T fallback)
        where T : notnull =>
        option switch
        {
            Some<T> some => some,
            _ => new Some<T>(fallback),
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T OrValue<T>(Option<T> option, T fallback)
        where T : notnull =>
        option switch
        {
            Some<T> some => some.Value,
            _ => fallback,
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> OrInsert<T>(Option<T> option, Option<T> fallbackOption)
        where T : notnull =>
        option switch
        {
            Some<T> some => some,
            _ => fallbackOption,
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> OrInsert<T>(Option<T> option, T fallbackValue)
        where T : notnull =>
        option switch
        {
            Some<T> some => some,
            _ => new Some<T>(fallbackValue),
        };

    public static partial class Unsafe
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? GetValueOrDefault<T>(Option<T> option)
            where T : notnull
        {
            return option switch
            {
                Some<T> someValue => someValue.Value,
                _ => default,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetValue<T>(Option<T> option, [NotNullWhen(true)] out T? value)
            where T : notnull
        {
            switch (option)
            {
                case Some<T> someValue:
                    value = someValue.Value;
                    return true;
                default:
                    value = default;
                    return false;
            }
        }

        public static void Do<T>(Option<T> option, Action<T> action)
            where T : notnull
        {
            if (!TryGetValue(option, out var value))
            {
                return;
            }

            action(value);
        }
    }
}

public static class OptionExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSome<T>(this Option<T> option)
        where T : notnull => Option.IsSome(option);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNone<T>(this Option<T> option)
        where T : notnull => Option.IsNone(option);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T? GetValueOrDefault<T>(this Option<T> option)
        where T : notnull => Option.Unsafe.GetValueOrDefault(option);

    public static bool TryGetValue<T>(this Option<T> option, [NotNullWhen(true)] out T? value)
        where T : notnull => Option.Unsafe.TryGetValue(option, out value);

    public static void Do<T>(this Option<T> option, Action<T> action)
        where T : notnull
    {
        Option.Unsafe.Do(option, action);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Or<T>(this Option<T> option, Option<T> fallback)
        where T : notnull => Option.Or(option, fallback);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Or<T>(this Option<T> option, T fallback)
        where T : notnull => Option.Or(option, fallback);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ValueOr<T>(this Option<T> option, T fallback)
        where T : notnull => Option.OrValue(option, fallback);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> OrInsert<T>(this Option<T> option, Option<T> fallbackOption)
        where T : notnull => Option.OrInsert(option, fallbackOption);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> OrInsert<T>(this Option<T> option, T fallbackValue)
        where T : notnull => Option.OrInsert(option, fallbackValue);
}
