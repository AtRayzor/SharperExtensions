using System.Diagnostics.CodeAnalysis;

namespace Monads.ResultMonad;

public static class Result
{
    public static Result<TValue, TError> ReturnOk<TValue, TError>(TValue value)
        where TValue : notnull where TError : notnull => new Result<TValue, TError>.Ok(value);

    public static Result<TValue, TError> ReturnError<TValue, TError>(TError error)
        where TError : notnull where TValue : notnull => new Result<TValue, TError>.Error(error);

    public static Result<TValue> ReturnOk<TValue>(TValue value) where TValue : notnull => new Result<TValue>.Ok(value);

    public static Result<TValue> ReturnError<TValue>(string message) where TValue : notnull =>
        new Result<TValue>.Error(message);

    public static Result<TValue, TError> Return<TValue, TError>(TValue? value, TError error)
        where TValue : notnull where TError : notnull => value is not null
        ? ReturnOk<TValue, TError>(value)
        : ReturnError<TValue, TError>(error);

    public static Result<TValue> Return<TValue>(TValue? value, string errorMessage) where TValue : notnull
        => value is not null ? ReturnOk(value) : ReturnError<TValue>(errorMessage);

    public static Result<TValue> Return<TValue>(TValue? value) where TValue : notnull =>
        Return(value, "Error! Null value.");
}

public abstract record Result<TValue, TError> where TValue : notnull where TError : notnull
{
    public record Ok(TValue Value) : Result<TValue, TError>
    {
        public static implicit operator TValue(Ok okResult) => okResult.Value;
    }


    public record Error(TError ErrorOutput) : Result<TValue, TError>
    {
        public static implicit operator TError(Error errorResult) => errorResult.ErrorOutput;
    }

    public static implicit operator Result<TValue, TError>(TValue value) => new Ok(value);
    public static implicit operator Result<TValue, TError>(TError error) => new Error(error);

    public Result<TNewValue, TNewError> Map<TNewValue, TNewError>(
        Func<TValue, Result<TNewValue, TNewError>> fValue,
        Func<TError, Result<TNewValue, TNewError>> fError) where TNewValue : notnull where TNewError : notnull =>
        this switch
        {
            Ok okValue => fValue(okValue),
            Error error => fError(error)
        };

    public Result<TNewValue, TError> Map<TNewValue>(Func<TValue, Result<TNewValue, TError>> fValue)
        where TNewValue : notnull =>
        Map(fValue, Result.ReturnError<TNewValue, TError>);

    public Result<TValue, TNewError> Map<TNewError>(Func<TError, Result<TValue, TNewError>> fError)
        where TNewError : notnull => Map(Result.ReturnOk<TValue, TNewError>, fError);

    public Result<TNewValue> Map<TNewValue>(Func<TValue, Result<TNewValue>> fValue,
        Func<TError, Result<TNewValue>> fError) where TNewValue : notnull => this switch
    {
        Ok okValue => fValue(okValue),
        Error error => fError(error)
    };

    public bool GetValueIfOk([NotNullWhen(returnValue: true)] out TValue? value)
    {
        if (this is Ok okResult)
        {
            value = okResult;

            return true;
        }

        value = default;
        return false;
    }

    public bool GetOutputIfError([NotNullWhen(returnValue: true)] out TError? errorOutput)
    {
        if (this is Error errorResult)
        {
            errorOutput = errorResult;
            return true;
        }

        errorOutput = default;
        return false;
    }

    public TValue GetValueOrThrow<TException>() where TException : Exception, new() => this switch
    {
        Ok okValue => okValue,
        Error => ThrowException<TException>(null)
    };

    public TValue GetValueOrThrow<TException>(string message) where TException : Exception
        => this switch
        {
            Ok okValue => okValue,
            Error error => ThrowException<TException>(message)
        };


    public TValue GetValueOrThrow<TException>(Func<TError, string> createErrorMessage)
        where TException : Exception
        => this switch
        {
            Ok okValue => okValue,
            Error error => ThrowException<TException>(createErrorMessage(error))
        };

    private TValue ThrowException<TException>(string? message)
        where TException : Exception
    {
        var exception = message is null
            ? Activator.CreateInstance<TException>()
            : (TException)Activator.CreateInstance(typeof(TException), [message])!;

        throw exception;
    }


    public void Execute(Action<TValue> action, Action<TError> fallback)
    {
        switch (this)
        {
            case Ok okValue:
                action(okValue);
                break;
            case Error error:
                fallback(error);
                break;
        }
    }

    public void Execute(Action<TValue> action) => Execute(action, _ => { });

    public TReturn Execute<TReturn>(Func<TValue, TReturn> func, Func<TError, TReturn> fallback)
        => this switch
        {
            Ok okValue => func(okValue),
            Error error => fallback(error)
        };
}

public abstract record Result<TValue>(Result<TValue, string> ResultBase) where TValue : notnull
{
    public record Ok(TValue Value) : Result<TValue>((Result<TValue, string>)new Result<TValue, string>.Ok(Value))
    {
        public static implicit operator TValue(Ok okResult) => okResult.Value;
        public static implicit operator Ok(TValue value) => new(value);
    }

    public record Error(string Message)
        : Result<TValue>((Result<TValue, string>)new Result<TValue, string>.Error(Message))
    {
        public static implicit operator string(Error errorResult) => errorResult.Message;
        public static implicit operator Error(string error) => new(error);
    }

    private static Result<T> FromBase<T>(Result<T, string> baseResult) where T : notnull => baseResult switch
    {
        Result<T, string>.Ok okResult => new Result<T>.Ok(okResult.Value),
        Result<T, string>.Error errorResult => new Result<T>.Error(errorResult.ErrorOutput)
    };

    public static implicit operator Result<TValue>(Result<TValue, string> baseResult) => FromBase(baseResult);
    public static implicit operator Result<TValue, string>(Result<TValue> result) => result.ResultBase;
    public static implicit operator Result<TValue>(TValue value) => new Ok(value);
    public static implicit operator Result<TValue>(string message) => new Error(message);


    public Result<TNewValue, TNewError> Map<TNewValue, TNewError>(
        Func<TValue, Result<TNewValue, TNewError>> fValue,
        Func<string, Result<TNewValue, TNewError>> fError) where TNewValue : notnull where TNewError : notnull =>
        this switch
        {
            Ok okValue => fValue(okValue),
            Error error => fError(error)
        };

    public Result<TNewValue> Map<TNewValue>(Func<TValue, Result<TNewValue>> fValue,
        Func<string, Result<TNewValue>> fError)
        where TNewValue : notnull => this switch
    {
        Ok okValue => fValue(okValue),
        Error message => fError(message)
    };

    public Result<TNewValue> Map<TNewValue>(Func<TValue, Result<TNewValue>> fValue)
        where TNewValue : notnull => Map(fValue, Result.ReturnError<TNewValue>);

    public Result<TValue> Map(Func<string, Result<TValue>> fError) => Map(Result.ReturnOk, fError);


    public bool GetValueIfOk([NotNullWhen(returnValue: true)] out TValue? value)
        => ResultBase.GetValueIfOk(out value);

    public bool GetMessageIfError([NotNullWhen(returnValue: true)] out string? message)
        => ResultBase.GetOutputIfError(out message);

    public void Execute(Action<TValue> action, Action<string> fallback)
        => ResultBase.Execute(action, fallback);

    public void Execute(Action<TValue> action)
        => ResultBase.Execute(action);

    public TReturn Execute<TReturn>(Func<TValue, TReturn> func, Func<string, TReturn> fallback)
        => ResultBase.Execute(func, fallback);

    public TValue GetValueOrThrow<TException>() where TException : Exception, new() =>
        ResultBase.GetValueOrThrow<TException>();

    public TValue GetValueOrThrow<TException>(string message) where TException : Exception, new()
        => ResultBase.GetValueOrThrow<TException>(message);


    public TValue GetValueOrThrow<TException>(Func<string, string> createErrorMessage)
        where TException : Exception, new()
        => ResultBase.GetValueOrThrow<TException>(createErrorMessage);
}