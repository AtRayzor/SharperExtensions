using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using DotNetCoreFunctional.UnionTypes;

namespace DotNetCoreFunctional.Result;

[Closed]
public abstract record Result<T, TError>
    where T : notnull
    where TError : notnull
{
    public static Result<T, TError> Ok(T value) => new Ok<T, TError>(value);

    public static Result<T, TError> Error(TError error) => new Error<T, TError>(error);
}

public record Ok<T, TError>(T Value) : Result<T, TError>
    where T : notnull
    where TError : notnull
{
    public static implicit operator Ok<T, TError>(T value) => new(value);

    public static implicit operator T(Ok<T, TError> okResult) => okResult.Value;
}

public record Error<T, TError>(TError Err) : Result<T, TError>
    where T : notnull
    where TError : notnull
{
    public static implicit operator Error<T, TError>(TError error) => new(error);

    public static implicit operator TError(Error<T, TError> error) => error.Err;
}

public static partial class Result
{
    private const string DefaultErrorMessage = "Unknown error.";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, TError> Ok<T, TError>(T value)
        where T : notnull
        where TError : notnull => new Ok<T, TError>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, TError> Error<T, TError>(TError error)
        where T : notnull
        where TError : notnull => new Error<T, TError>(error);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOk<T, TError>(Result<T, TError> result)
        where T : notnull
        where TError : notnull => result is Ok<T, TError>;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsError<T, TError>(Result<T, TError> result)
        where T : notnull
        where TError : notnull => result is Error<T, TError>;

    public static partial class Unsafe
    {
        public static bool TryGetValue<T, TError>(
            Result<T, TError> result,
            [NotNullWhen(true)] out T? value
        )
            where T : notnull
            where TError : notnull
        {
            if (result is not Ok<T, TError> { Value: var val })
            {
                value = default;
                return false;
            }

            value = val;
            return true;
        }

        public static bool TryGetError<T, TError>(
            Result<T, TError> result,
            [NotNullWhen(true)] out TError? error
        )
            where T : notnull
            where TError : notnull
        {
            if (result is not Error<T, TError> { Err: var err })
            {
                error = default;
                return false;
            }

            error = err;
            return true;
        }

        public static void DoIfOk<T, TError>(Result<T, TError> result, Action<T> action)
            where T : notnull
            where TError : notnull
        {
            if (!TryGetValue(result, out var value))
                return;

            action(value);
        }

        public static void DoIfError<T, TError>(Result<T, TError> result, Action<TError> action)
            where T : notnull
            where TError : notnull
        {
            if (!TryGetError(result, out var error))
                return;

            action(error);
        }
    }
}

public static class ResultExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOk<T, TError>(this Result<T, TError> result)
        where T : notnull
        where TError : notnull
    {
        return Result.IsOk(result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsError<T, TError>(this Result<T, TError> result)
        where T : notnull
        where TError : notnull => Result.IsError(result);

    public static bool TryGetValue<T, TError>(
        this Result<T, TError> result,
        [NotNullWhen(true)] out T? value
    )
        where T : notnull
        where TError : notnull => Result.Unsafe.TryGetValue(result, out value);

    public static bool TryGetError<T, TError>(
        this Result<T, TError> result,
        [NotNullWhen(true)] out TError? error
    )
        where T : notnull
        where TError : notnull => Result.Unsafe.TryGetError(result, out error);

    public static void DoIfOk<T, TError>(this Result<T, TError> result, Action<T> action)
        where T : notnull
        where TError : notnull => Result.Unsafe.DoIfOk(result, action);

    public static void DoIfError<T, TError>(this Result<T, TError> result, Action<TError> action)
        where T : notnull
        where TError : notnull => Result.Unsafe.DoIfError(result, action);
}
