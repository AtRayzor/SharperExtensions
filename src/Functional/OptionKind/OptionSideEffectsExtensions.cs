using Monads.Traits;

namespace Monads.OptionKind;

public static class OptionSideEffectsExtensions
{
    public static void ThrowIfNone<T, TException>(this ISideEffects<OptionType<T>> option)
        where T : notnull where TException : Exception
        => ThrowExceptionIfIsNone<T, TException>(option.Type, []);

    public static void ThrowIfNone<T, TException>(this ISideEffects<OptionType<T>> option, string message)
        where T : notnull where TException : Exception
        => ThrowExceptionIfIsNone<T, TException>(option.Type, [message]);

    public static void ThrowIfNone<T, TException>(this ISideEffects<OptionType<T>> option, object[] args)
        where T : notnull where TException : Exception
        => ThrowExceptionIfIsNone<T, TException>(option.Type, args);

    private static void ThrowExceptionIfIsNone<T, TException>(OptionType<T> optionType,
        object[] constructorArgs) where T : notnull where TException : Exception
    {
        if (optionType is not OptionType<T>.None) return;

        var exception = Activator.CreateInstance(typeof(TException), constructorArgs) as TException
                        ?? throw new InvalidOperationException(
                            "No suitable constructor was found for the given exception type.");

        throw exception;
    }

    public static T GetValueOrThrow<T, TException>(this ISideEffects<OptionType<T>> option)
        where T : notnull where TException : Exception
    {
        option.ThrowIfNone<T, TException>();

        return (OptionType<T>.Some)option.Type;
    }

    public static T GetValueOrThrow<T, TException>(this ISideEffects<OptionType<T>> option, string message)
        where T : notnull where TException : Exception
    {
        option.ThrowIfNone<T, TException>(message);

        return (OptionType<T>.Some)option.Type;
    }

    public static T GetValueOrThrow<T, TException>(this ISideEffects<OptionType<T>> option, object[] args)
        where T : notnull where TException : Exception
    {
        option.ThrowIfNone<T, TException>(args);

        return (OptionType<T>.Some)option.Type;
    }

    public static void DoOrThrow<T, TException>(this ISideEffects<OptionType<T>> option, Action<T> action)
        where T : notnull where TException : Exception
    {
        option.ThrowIfNone<T, TException>();

        action((OptionType<T>.Some)option.Type);
    }

    public static void DoOrThrow<T, TException>(this ISideEffects<OptionType<T>> option, Action<T> action,
        string message)
        where T : notnull where TException : Exception
    {
        option.ThrowIfNone<T, TException>(message);

        action((OptionType<T>.Some)option.Type);
    }

    public static void DoOrThrow<T, TException>(this ISideEffects<OptionType<T>> option, Action<T> action,
        object[] args)
        where T : notnull where TException : Exception
    {
        option.ThrowIfNone<T, TException>(args);

        action((OptionType<T>.Some)option.Type);
    }
}