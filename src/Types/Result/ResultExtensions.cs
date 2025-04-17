using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace DotNetCoreFunctional.Result;

#pragma warning disable CS8509
/// <summary>
/// Provides extension methods for working with the Result type, offering convenient methods for handling and transforming Result instances.
/// </summary>
/// <remarks>
/// These extension methods simplify common operations on Result types, such as checking status, inserting values, and chaining operations.
/// </remarks>
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

    public static T? GetValueOrDefault<T, TError>(this Result<T, TError> result)
        where T : notnull
        where TError : notnull => Result.Unsafe.GetValueOrDefault(result);

    public static TError? GetErrorOrDefault<T, TError>(this Result<T, TError> result)
        where T : notnull
        where TError : notnull => Result.Unsafe.GetErrorOrDefault(result);

    public static T GetValueOrDefault<T, TError>(this Result<T, TError> result, T defaultValue)
        where T : notnull
        where TError : notnull => Result.Unsafe.GetValueOrDefault(result, defaultValue);

    public static TError GetErrorOrDefault<T, TError>(
        this Result<T, TError> result,
        TError defaultError
    )
        where T : notnull
        where TError : notnull => Result.Unsafe.GetErrorOrDefault(result, defaultError);

    public static void DoIfOk<T, TError>(this Result<T, TError> result, Action<T> action)
        where T : notnull
        where TError : notnull => Result.Unsafe.DoIfOk(result, action);

    public static void DoIfError<T, TError>(this Result<T, TError> result, Action<TError> action)
        where T : notnull
        where TError : notnull => Result.Unsafe.DoIfError(result, action);

    public static void Do<T, TError>(
        this Result<T, TError> result,
        Action<T> ifOk,
        Action<TError> ifError
    )
        where T : notnull
        where TError : notnull => Result.Unsafe.Do(result, ifOk, ifError);

}
