using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using DotNetCoreFunctional.UnionTypes;

namespace DotNetCoreFunctional.Option;

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

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static Option<T> Some(T value)
    {
        return new Some<T>(value);
    }

    public static implicit operator Option<T>(T? value) => Option.Return(value);
}

public record Some<T>(T Value) : Option<T>
    where T : notnull
{
    public static implicit operator T(Some<T> some) => some.Value;
}

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

    public static partial class Unsafe
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? GetValueOrDefault<T>(Option<T> option)
            where T : notnull => option.SingleOrDefault();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetValue<T>(Option<T> option, [NotNullWhen(true)] out T? value)
            where T : notnull => (value = GetValueOrDefault(option)) is not null;

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
}
