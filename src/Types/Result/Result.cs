using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Xml.XPath;
using DotNetCoreFunctional.UnionTypes;

namespace DotNetCoreFunctional.Result;

#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).

/// <summary>
/// Represents the result of an operation, either successful with a value of type <typeparamref name="T"/> or failed with an error of type <typeparamref name="TError"/>.
/// </summary>
/// <typeparam name="T">The type of the value in case of success.</typeparam>
/// <typeparam name="TError">The type of the error in case of failure.</typeparam>
[Closed]
public abstract record Result<T, TError>
    where T : notnull
    where TError : notnull
{
    /// <summary>
    /// Creates a successful result with the given value.
    /// </summary>
    /// <param name="value">The value of the successful result.</param>
    /// <returns>A <see cref="Result{T, TError}"/> representing a successful operation.</returns>
    public static Result<T, TError> Ok(T value) => new Ok<T, TError>(value);

    /// <summary>
    /// Creates a failed result with the given error.
    /// </summary>
    /// <param name="error">The error of the failed result.</param>
    /// <returns>A <see cref="Result{T, TError}"/> representing a failed operation.</returns>
    public static Result<T, TError> Error(TError error) => new Error<T, TError>(error);
}

/// <summary>
/// Represents a successful result with a value.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
/// <typeparam name="TError">The type of the error (not used in this case).</typeparam>
public record Ok<T, TError>(T Value) : Result<T, TError>
    where T : notnull
    where TError : notnull
{
    /// <summary>
    /// Implicitly converts a value of type <typeparamref name="T"/> to an <see cref="Ok{T, TError}"/> result.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator Ok<T, TError>(T value) => new(value);

    /// <summary>
    /// Implicitly converts an <see cref="Ok{T, TError}"/> result to its underlying value of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="okResult">The result to convert.</param>
    public static implicit operator T(Ok<T, TError> okResult) => okResult.Value;
}

/// <summary>
/// Represents a failed result with an error.
/// </summary>
/// <typeparam name="T">The type of the value (not used in this case).</typeparam>
/// <typeparam name="TError">The type of the error.</typeparam>
public record Error<T, TError>(TError Err) : Result<T, TError>
    where T : notnull
    where TError : notnull
{
    /// <summary>
    /// Implicitly converts an error of type <typeparamref name="TError"/> to an <see cref="Error{T, TError}"/> result.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator Error<T, TError>(TError error) => new(error);

    /// <summary>
    /// Implicitly converts an <see cref="Error{T, TError}"/> result to its underlying error of type <typeparamref name="TError"/>.
    /// </summary>
    /// <param name="error">The result to convert.</param>
    public static implicit operator TError(Error<T, TError> error) => error.Err;
}

/// <summary>
/// Provides static methods for working with <see cref="Result{T, TError}"/> types.
/// </summary>
public static partial class Result
{
    private const string DefaultErrorMessage = "Unknown error.";

    /// <summary>
    /// Creates a successful result with the given value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="value">The value of the successful result.</param>
    /// <returns>A <see cref="Result{T, TError}"/> representing a successful operation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, TError> Ok<T, TError>(T value)
        where T : notnull
        where TError : notnull => new Ok<T, TError>(value);

    /// <summary>
    /// Creates a <see cref="Result{T, TError}"/> from a potentially null value, returning an error if the value is null.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="value">The value to convert to a result.</param>
    /// <param name="nullError">The error to use if the value is null.</param>
    /// <returns>A successful result containing the value if not null, or an error result.</returns>
    public static Result<T, TError> Create<T, TError>(T? value, TError nullError)
        where T : notnull
        where TError : notnull =>
        value is not null ? Ok<T, TError>(value) : Error<T, TError>(nullError);

    /// <summary>
    /// Creates a failed result with the given error.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="error">The error of the failed result.</param>
    /// <returns>A <see cref="Result{T, TError}"/> representing a failed operation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, TError> Error<T, TError>(TError error)
        where T : notnull
        where TError : notnull => new Error<T, TError>(error);

    /// <summary>
    /// Checks if a <see cref="Result{T, TError}"/> is successful (Ok).
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="result">The result to check.</param>
    /// <returns><c>true</c> if the result is successful, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOk<T, TError>(Result<T, TError> result)
        where T : notnull
        where TError : notnull => result is Ok<T, TError>;

    /// <summary>
    /// Checks if a <see cref="Result{T, TError}"/> is failed (Error).
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="result">The result to check.</param>
    /// <returns><c>true</c> if the result is failed, <c>false</c> otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsError<T, TError>(Result<T, TError> result)
        where T : notnull
        where TError : notnull => result is Error<T, TError>;

    public static Result<(T1, T2), TError> Combine<T1, T2, TError>(
        Result<T1, TError> result1,
        Result<T2, TError> result2,
        Func<TError, TError, TError> errorCollisionHandler
    )
        where T1 : notnull
        where T2 : notnull
        where TError : notnull =>
        result1.Match(
            matchOk: value1 =>
                result2.Match(
                    matchOk: value2 => Result<(T1, T2), TError>.Ok((value1, value2)),
                    matchError: Result<(T1, T2), TError>.Error
                ),
            matchError: error1 =>
                result2.Match(
                    matchOk: _ => Result<(T1, T2), TError>.Error(error1),
                    matchError: error2 =>
                        Result<(T1, T2), TError>.Error(errorCollisionHandler(error1, error2))
                )
        );

    /// <summary>
    /// Provides unsafe utility methods for working with <see cref="Result{T, TError}"/> types.
    /// </summary>
    public static partial class Unsafe
    {
        /// <summary>
        /// Retrieves the value from a <see cref="Result{T, TError}"/> if it is successful, otherwise returns the default value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="result">The result to extract the value from.</param>
        /// <returns>The value of the result if successful, or <c>default</c> if the result is an error.</returns>
        public static T? GetValueOrDefault<T, TError>(Result<T, TError> result)
            where T : notnull
            where TError : notnull
        {
            return result is not Ok<T, TError> { Value: var value } ? default : value;
        }

        /// <summary>
        /// Retrieves the error from a <see cref="Result{T, TError}"/> if it is an error, otherwise returns the default value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="result">The result to extract the error from.</param>
        /// <returns>The error of the result if it is an error, or <c>default</c> if the result is successful.</returns>
        public static TError? GetErrorOrDefault<T, TError>(Result<T, TError> result)
            where T : notnull
            where TError : notnull
        {
            return result is not Error<T, TError> { Err: var error } ? default : error;
        }

        /// <summary>
        /// Retrieves the value from a <see cref="Result{T, TError}"/> if it is successful, otherwise returns a specified default value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="result">The result to extract the value from.</param>
        /// <param name="defaultValue">The default value to return if the result is an error.</param>
        /// <returns>The value of the result if successful, or the specified <paramref name="defaultValue"/> if the result is an error.</returns>
        public static T GetValueOrDefault<T, TError>(Result<T, TError> result, T defaultValue)
            where T : notnull
            where TError : notnull =>
            result is Ok<T, TError> { Value: var value } ? value : defaultValue;

        /// <summary>
        /// Retrieves the error from a <see cref="Result{T, TError}"/> if it is an error, otherwise returns a specified default error.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="result">The result to extract the error from.</param>
        /// <param name="defaultError">The default error to return if the result is not an error.</param>
        /// <returns>The error of the result if it is an error, or the specified <paramref name="defaultError"/> if the result is successful.</returns>
        public static TError GetErrorOrDefault<T, TError>(
            Result<T, TError> result,
            TError defaultError
        )
            where T : notnull
            where TError : notnull =>
            result is Error<T, TError> { Err: var error } ? error : defaultError;

        /// <summary>
        /// Tries to get the value from a <see cref="Result{T, TError}"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="result">The result to get the value from.</param>
        /// <param name="value">When this method returns, contains the value of the result if it is Ok, or the default value of type <typeparamref name="T"/> if it is an error. This parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if the result is Ok and the value was successfully retrieved; otherwise, <c>false</c>.</returns>
        public static bool TryGetValue<T, TError>(
            Result<T, TError> result,
            [MaybeNullWhen(false)] out T value
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

        /// <summary>
        /// Tries to get the error from a <see cref="Result{T, TError}"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="result">The result to get the error from.</param>
        /// <param name="error">When this method returns, contains the error of the result if it is an error, or the default value of type <typeparamref name="TError"/> if it is Ok. This parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if the result is an error and the error was successfully retrieved; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// Executes an action if the <see cref="Result{T, TError}"/> is Ok.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="result">The result to check.</param>
        /// <param name="action">The action to execute if the result is Ok.</param>
        public static void DoIfOk<T, TError>(Result<T, TError> result, Action<T> action)
            where T : notnull
            where TError : notnull
        {
            if (!TryGetValue(result, out var value))
                return;

            action(value);
        }

        /// <summary>
        /// Executes an action if the <see cref="Result{T, TError}"/> is an error.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="result">The result to check.</param>
        /// <param name="action">The action to execute if the result is an error.</param>
        public static void DoIfError<T, TError>(Result<T, TError> result, Action<TError> action)
            where T : notnull
            where TError : notnull
        {
            if (!TryGetError(result, out var error))
                return;

            action(error);
        }

        /// <summary>
        /// Executes an action based on whether the <see cref="Result{T, TError}"/> is Ok or an error.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="result">The result to check.</param>
        /// <param name="ifOk">The action to execute if the result is Ok.</param>
        /// <param name="ifError">The action to execute if the result is an error.</param>
        public static void Do<T, TError>(
            Result<T, TError> result,
            Action<T> ifOk,
            Action<TError> ifError
        )
            where T : notnull
            where TError : notnull
        {
            switch (result)
            {
                case Ok<T, TError> { Value: var value }:
                {
                    ifOk(value);
                    break;
                }
                case Error<T, TError> { Err: var error }:
                {
                    ifError(error);
                    break;
                }
            }
        }
    }
}
