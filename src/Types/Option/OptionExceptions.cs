using DotNetCoreFunctional.Helpers;

namespace DotNetCoreFunctional.Option;

public static partial class Option
{
    public static partial class Unsafe
    {
        public static T GetValueOrThrow<T, TException>(
            Option<T> option,
            params object[] constructorArgs
        )
            where T : notnull
            where TException : Exception
        {
            return GetValueOrThrowRequestedException<T, TException>(option, constructorArgs);
        }

        public static void DoOrThrow<T, TException>(
            Option<T> option,
            Action<T> action,
            params object[] constructorArgs
        )
            where T : notnull
            where TException : Exception
        {
            action(GetValueOrThrowRequestedException<T, TException>(option, constructorArgs));
        }

        private static T GetValueOrThrowRequestedException<T, TException>(
            Option<T> option,
            object[]? constructorArgs = default
        )
            where T : notnull
            where TException : Exception
        {
            var activatedException = ExceptionHelpers.TryToConstructException<TException>(
                constructorArgs
            );

            return option switch
            {
                Some<T> some => some.Value,
                _ => throw activatedException
            };
        }
    }
}

public static class OptionExceptionExtensions
{
    public static T GetValueOrThrow<T, TException>(
        this Option<T> option,
        params object[] constructorArgs
    )
        where T : notnull
        where TException : Exception =>
        Option.Unsafe.GetValueOrThrow<T, TException>(option, constructorArgs);

    public static void DoOrThrow<T, TException>(
        this Option<T> option,
        Action<T> action,
        params object[] constructorArgs
    )
        where T : notnull
        where TException : Exception
    {
        Option.Unsafe.DoOrThrow<T, TException>(option, action, constructorArgs);
    }
}
