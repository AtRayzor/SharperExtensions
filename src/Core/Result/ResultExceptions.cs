namespace SharperExtensions;

#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
public static partial class Result
{
    public static partial class Unsafe
    {
        public static T GetValueOrThrow<T, TError, TException>(Result<T, TError> result)
            where T : notnull
            where TError : notnull
            where TException : Exception
        {
            return GetValueOrThrowConstructedException(
                result,
                DefaultExceptionFactory<TError, TException>
            );
        }

        public static T GetValueOrThrow<T, TError, TException>(
            Result<T, TError> result,
            string errorMessage
        )
            where T : notnull
            where TError : notnull
            where TException : Exception
        {
            return GetValueOrThrowConstructedException(
                result,
                _ => ExceptionHelpers.TryToConstructException<TException>([errorMessage])
            );
        }

        public static T GetValueOrThrow<T, TError, TException>(
            Result<T, TError> result,
            Func<TError, TException> exceptionFactory
        )
            where T : notnull
            where TError : notnull
            where TException : Exception
        {
            return GetValueOrThrowConstructedException(result, exceptionFactory);
        }

        private static T GetValueOrThrowConstructedException<T, TError, TException>(
            Result<T, TError> result,
            Func<TError, TException> exceptionFactory
        )
            where T : notnull
            where TError : notnull
            where TException : Exception
        {
            return result switch
            {
                { IsOk: true, Value: var value } => value,
                { IsError: true, ErrorValue: var error } => throw exceptionFactory(error)
            };
        }

        private static Option<string> ConstructErrorMessage<TError>(TError error)
            where TError : notnull
        {
            if (error is string message)
                return message.ToOption();

            if (error as IErrorWithMessage is { } errorWithMessage)
                return errorWithMessage.Message.ToOption();

            if (
                error
                        .GetType()
                        .GetProperties()
                        .FirstOrDefault(type => type.Name == "Message")
                    is { } messagePropertyInfo
                && messagePropertyInfo.GetValue(error) is string errorMessage
            )
                return errorMessage.ToOption();

            return Option.None<string>();
        }

        private static TException DefaultExceptionFactory<TError, TException>(
            TError error
        )
            where TError : notnull
            where TException : Exception
        {
            return ConstructErrorMessage(error) switch
            {
                { IsSome: true, Value: var message } => ExceptionHelpers
                    .TryToConstructException<TException>([message]),
                _ => ExceptionHelpers.TryToConstructException<TException>(),
            };
        }
    }
}

public static class ResultExceptionExtensions
{
    public static T GetValueOrThrow<T, TError, TException>(this Result<T, TError> result)
        where T : notnull
        where TError : notnull
        where TException : Exception
    {
        return Result.Unsafe.GetValueOrThrow<T, TError, TException>(result);
    }

    public static T GetValueOrThrow<T, TError, TException>(
        this Result<T, TError> result,
        string errorMessage
    )
        where T : notnull
        where TError : notnull
        where TException : Exception
    {
        return Result.Unsafe.GetValueOrThrow<T, TError, TException>(result, errorMessage);
    }

    public static T GetValueOrThrow<T, TError, TException>(
        this Result<T, TError> result,
        Func<TError, TException> exceptionFactory
    )
        where T : notnull
        where TError : notnull
        where TException : Exception
    {
        return Result.Unsafe.GetValueOrThrow(result, exceptionFactory);
    }
}