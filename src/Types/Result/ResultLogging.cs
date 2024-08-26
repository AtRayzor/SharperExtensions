using Microsoft.Extensions.Logging;

namespace NetFunctional.Types;

public static partial class Result
{
    public static class Logging
    {
        public static Result<T, TError> LogIfOk<T, TError>(
            Result<T, TError> result,
            Func<T, string> createMessage,
            ILogger logger,
            LogLevel logLevel = LogLevel.None
        )
            where T : notnull
            where TError : notnull
        {
            if (result is Ok<T, TError> ok)
                logger.Log(logLevel, "{ }", createMessage(ok));

            return result;
        }

        public static Result<T, TError> LogIfOk<T, TError>(
            Result<T, TError> result,
            Func<T, string> createMessage,
            ILogger logger,
            EventId eventId,
            LogLevel logLevel = LogLevel.None
        )
            where T : notnull
            where TError : notnull
        {
            if (result is Ok<T, TError> ok)
                logger.Log(logLevel, eventId, "{ }", createMessage(ok));

            return result;
        }

        public static Result<T, TError> LogInformationIfOk<T, TError>(
            Result<T, TError> result,
            Func<T, string> createMessage,
            ILogger logger
        )
            where T : notnull
            where TError : notnull
        {
            return LogIfOk(result, createMessage, logger, LogLevel.Information);
        }

        public static Result<T, TError> LogIfError<T, TError>(
            Result<T, TError> result,
            Func<TError, string> createMessage,
            ILogger logger,
            LogLevel logLevel = LogLevel.Error
        )
            where T : notnull
            where TError : notnull
        {
            if (result is Error<T, TError> error)
                logger.Log(logLevel, "{ }", createMessage(error));

            return result;
        }

        public static Result<T, TError> LogIfError<T, TError>(
            Result<T, TError> result,
            EventId eventId,
            Func<TError, string> createMessage,
            ILogger logger,
            LogLevel logLevel = LogLevel.Error
        )
            where T : notnull
            where TError : notnull
        {
            if (result is Error<T, TError> error)
                logger.Log(logLevel, eventId, "{ }", createMessage(error));

            return result;
        }

        public static Result<T, TError> LogIfError<T, TError>(
            Result<T, TError> result,
            ILogger logger,
            LogLevel logLevel
        )
            where T : notnull
            where TError : IErrorWithMessage
        {
            if (result is Error<T, TError> { Err.Message: { } errorMessage })
                logger.Log(logLevel, "{ }", errorMessage);

            return result;
        }

        public static Result<T, TError> LogIfError<T, TError>(
            Result<T, TError> result,
            ILogger logger,
            EventId eventId,
            LogLevel logLevel = LogLevel.Error
        )
            where T : notnull
            where TError : IErrorWithMessage
        {
            if (result is Error<T, TError> { Err.Message: var errorMessage })
                logger.Log(logLevel, eventId, "{ }", errorMessage);

            return result;
        }

        public static Result<T, TError> LogIfError<T, TError>(
            Result<T, TError> result,
            ILogger logger
        )
            where T : notnull
            where TError : ILoggableError
        {
            if (
                result is Error<T, TError>
                {
                    Err: { Message: var errorMessage, LogLevel: var logLevel }
                }
            )
                logger.Log(logLevel, "{ }", errorMessage);

            return result;
        }
    }
}

public static class ResultLoggingExtensions
{
    public static Result<T, TError> LogIfOk<T, TError>(
        this Result<T, TError> result,
        Func<T, string> createMessage,
        ILogger logger,
        LogLevel logLevel = LogLevel.None
    )
        where T : notnull
        where TError : notnull
    {
        return Result.Logging.LogIfOk(result, createMessage, logger, logLevel);
    }

    public static Result<T, TError> LogIfOk<T, TError>(
        this Result<T, TError> result,
        Func<T, string> createMessage,
        ILogger logger,
        EventId eventId,
        LogLevel logLevel = LogLevel.None
    )
        where T : notnull
        where TError : notnull
    {
        return Result.Logging.LogIfOk(result, createMessage, logger, eventId, logLevel);
    }

    public static Result<T, TError> LogInformationIfOk<T, TError>(
        Result<T, TError> result,
        Func<T, string> createMessage,
        ILogger logger
    )
        where T : notnull
        where TError : notnull
    {
        return LogIfOk(result, createMessage, logger, LogLevel.Information);
    }

    public static Result<T, TError> LogIfError<T, TError>(
        this Result<T, TError> result,
        Func<TError, string> createMessage,
        ILogger logger,
        LogLevel logLevel = LogLevel.Error
    )
        where T : notnull
        where TError : notnull
    {
        return Result.Logging.LogIfError(result, createMessage, logger, logLevel);
    }

    public static Result<T, TError> LogIfError<T, TError>(
        this Result<T, TError> result,
        EventId eventId,
        Func<TError, string> createMessage,
        ILogger logger,
        LogLevel logLevel = LogLevel.Error
    )
        where T : notnull
        where TError : notnull
    {
        return Result.Logging.LogIfError(result, eventId, createMessage, logger, logLevel);
    }

    public static Result<T, TError> LogIfError<T, TError>(
        this Result<T, TError> result,
        ILogger logger,
        LogLevel logLevel
    )
        where T : notnull
        where TError : notnull, IErrorWithMessage
    {
        return Result.Logging.LogIfError(result, logger, logLevel);
    }

    public static Result<T, TError> LogIfError<T, TError>(
        this Result<T, TError> result,
        ILogger logger,
        EventId eventId,
        LogLevel logLevel = LogLevel.Error
    )
        where T : notnull
        where TError : IErrorWithMessage
    {
        return Result.Logging.LogIfError(result, logger, eventId, logLevel);
    }

    public static Result<T, TError> LogIfError<T, TError>(
        this Result<T, TError> result,
        ILogger logger
    )
        where T : notnull
        where TError : ILoggableError
    {
        return Result.Logging.LogIfError(result, logger);
    }
}
