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
    public static T GetValueOr<T>(Option<T> option, T fallback)
        where T : notnull =>
        option switch
        {
            Some<T> some => some.Value,
            _ => fallback,
        };

    public static Option<(T1, T2)> Combine<T1, T2>(
        Option<T1> option1,
        Option<T2> option2
    )
        where T1 : notnull
        where T2 : notnull => (option1, option2)
        is (Some<T1> { Value: var value1 }, Some<T2> { Value: var value2 })
            ? Option<(T1, T2)>.Some((value1, value2))
            : Option<(T1, T2)>.None;

    public static Option<(T1, T2, T3)> Combine<T1, T2, T3>(
        Option<T1> option1,
        Option<T2> option2,
        Option<T3> option3
    )
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull => (option1, option2, option3)
        is (
        Some<T1> { Value: var value1 },
        Some<T2> { Value: var value2 },
        Some<T3> { Value: var value3 }
        )
            ? Option<(T1, T2, T3)>.Some((value1, value2, value3))
            : Option<(T1, T2, T3)>.None;

    public static Option<(T1, T2, T3)> Combine<T1, T2, T3>(
        Option<(T1, T2)> tuple,
        Option<T3> option3
    )
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull => (tuple, option3)
        is (
        Some<(T1, T2)> { Value: var (value1, value2) },
        Some<T3> { Value: var value3 }
        )
            ? Option<(T1, T2, T3)>.Some((value1, value2, value3))
            : Option<(T1, T2, T3)>.None;

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
        Some<T1> { Value: var value1 },
        Some<T2> { Value: var value2 },
        Some<T3> { Value: var value3 },
        Some<T4> { Value: var value4 }
        )
            ? Option<(T1, T2, T3, T4)>.Some((value1, value2, value3, value4))
            : Option<(T1, T2, T3, T4)>.None;

    public static Option<(T1, T2, T3, T4)> Combine<T1, T2, T3, T4>(
        Option<(T1, T2, T3)> tuple,
        Option<T4> option3
    )
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull => (tuple, option3)
        is (
        Some<(T1, T2, T3)> { Value: var (value1, value2, value3) },
        Some<T4> { Value: var value4 }
        )
            ? Option<(T1, T2, T3, T4)>.Some((value1, value2, value3, value4))
            : Option<(T1, T2, T3, T4)>.None;

    public static Option<T> OrElse<T>(params Option<T>[] options)
        where T : notnull
    {
        foreach (var opt in options)
        {
            if (opt is not Some<T> some)
            {
                continue;
            }

            return some;
        }
        
        return Option<T>.None;
    }
    
    public static Option<T> OrElse<T>(params Func<Option<T>>[] optionFuncs)
        where T : notnull
    {
        foreach (var func in optionFuncs)
        {
            if (func() is not Some<T> some)
            {
                continue;
            }

            return some;
        }
        
        return Option<T>.None;
    }

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
        public static bool TryGetValue<T>(
            Option<T> option, [NotNullWhen(true)] out T? value
        )
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

        public static void IfSome<T>(Option<T> option, Action<T> action)
            where T : notnull
        {
            if (!TryGetValue(option, out var value))
            {
                return;
            }

            action(value);
        }

        public static void IfNone<T>(Option<T> option, Action action)
            where T : notnull
        {
            if (!option.IsNone)
            {
                return;
            }

            action();
        }

        public static void Do<T>(
            Option<T> option,
            Action<T> someAction,
            Action noneAction
        )
            where T : notnull
        {
            switch (option)
            {
                case Some<T> { Value: var value }:
                {
                    someAction(value);
                    break;
                }
                case None<T>:
                {
                    noneAction();
                    break;
                }
            }
        }
    }
}