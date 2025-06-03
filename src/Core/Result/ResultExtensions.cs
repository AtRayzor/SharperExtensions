using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;

namespace SharperExtensions;

/// <summary>
/// Provides extension methods for working with the Result type, offering
/// convenient methods for handling and transforming Result instances.
/// </summary>
/// <remarks>
/// These extension methods simplify common operations on Result types, such
/// as checking status, inserting values, and chaining operations.
/// </remarks>
public static class ResultExtensions
{
    extension<T>(T? value)
        where T : notnull
    {
        /// <summary>
        /// Converts a nullable value to a Result, using the provided error if the value is null.
        /// </summary>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="nullError">The error to use if the value is null.</param>
        /// <returns>A Result representing the conversion of the value.</returns>
        public Result<T, TError> ToResult<TError>(TError nullError)
            where TError : notnull => Result.Create(value, nullError);
    }

    extension<T, TError>(Result<T, TError> result)
        where T : notnull
        where TError : notnull
    {
        /// <summary>
        /// Determines whether the current Result instance represents a successful outcome.
        /// </summary>
        /// <returns>True if the Result contains a value, otherwise false.</returns>
        public bool IsOk() => Result.IsOk(result);

        /// <summary>
        /// Determines whether the current Result instance represents an error outcome.
        /// </summary>
        /// <returns>True if the Result contains an error, otherwise false.</returns>
        public bool IsError() => Result.IsError(result);


        public Result<(T, T2), TError> Combine<T2>(
            Result<T2, TError> result2,
            Func<TError, TError, TError> errorCollisionHandler
        ) where T2 : notnull => Result.Combine(result, result2, errorCollisionHandler);

        /// <summary>
        /// Attempts to retrieve the value from the Result if it represents a successful outcome.
        /// </summary>
        /// <param name="value">When this method returns true, contains the value of the Result; otherwise, contains the default value of the type.</param>
        /// <returns>True if the Result contains a value, otherwise false.</returns>
        public bool TryGetValue([NotNullWhen(true)] out T? value) =>
            Result.Unsafe.TryGetValue(result, out value);

        /// <summary>
        /// Attempts to retrieve the error from the Result if it represents an error outcome.
        /// </summary>
        /// <param name="error">When this method returns true, contains the error of the Result; otherwise, contains the default value of the type.</param>
        /// <returns>True if the Result contains an error, otherwise false.</returns>
        public bool TryGetError([NotNullWhen(true)] out TError? error) =>
            Result.Unsafe.TryGetError(result, out error);

        /// <summary>
        /// Retrieves the value from the Result if it represents a successful outcome, otherwise returns the default value of the type.
        /// </summary>
        /// <returns>The value of the Result if successful, or the default value of the type if the Result represents an error.</returns>
        public T? GetValueOrDefault() =>
            Result.Unsafe.GetValueOrDefault(result);

        /// <summary>
        /// Retrieves the error from the Result if it represents an error outcome, otherwise returns the default value of the type.
        /// </summary>
        /// <returns>The error of the Result if an error occurred, or the default value of the type if the Result represents a successful outcome.</returns>
        public TError? GetErrorOrDefault() =>
            Result.Unsafe.GetErrorOrDefault(result);

        /// <summary>
        /// Retrieves the value from the Result if it represents a successful outcome, otherwise returns the specified default value.
        /// </summary>
        /// <param name="defaultValue">The default value to return if the Result represents an error.</param>
        /// <returns>The value of the Result if successful, or the specified default value if the Result represents an error.</returns>
        public T GetValueOrDefault(T defaultValue) =>
            Result.Unsafe.GetValueOrDefault(result, defaultValue);

        /// <summary>
        /// Retrieves the error from the Result if it represents an error outcome, otherwise returns the specified default error.
        /// </summary>
        /// <param name="defaultError">The default error to return if the Result represents a successful outcome.</param>
        /// <returns>The error of the Result if an error occurred, or the specified default error if the Result represents a successful outcome.</returns>
        public TError GetErrorOrDefault(TError defaultError) =>
            Result.Unsafe.GetErrorOrDefault(result, defaultError);

        /// <summary>
        /// Executes the specified action if the Result represents a successful outcome.
        /// </summary>
        /// <param name="action">The action to execute when the Result contains a value.</param>
        public void DoIfOk(Action<T> action) =>
            Result.Unsafe.DoIfOk(result, action);

        /// <summary>
        /// Executes the specified action if the Result represents an error outcome.
        /// </summary>
        /// <param name="action">The action to execute when the Result contains an error.</param>
        public void DoIfError(Action<TError> action) =>
            Result.Unsafe.DoIfError(result, action);

        /// <summary>
        /// Executes the specified actions based on the Result's state.
        /// </summary>
        /// <param name="ifOk">The action to execute when the Result contains a value.</param>
        /// <param name="ifError">The action to execute when the Result contains an error.</param>
        public void Do(Action<T> ifOk, Action<TError> ifError) =>
            Result.Unsafe.Do(result, ifOk, ifError);

        /// <summary>
        /// Transforms the value of the Result using the provided mapper function if the Result represents a successful outcome.
        /// </summary>
        /// <typeparam name="TNew">The type of the transformed value.</typeparam>
        /// <param name="mapper">A function that transforms the value of type T to a new value of type TNew.</param>
        /// <returns>A new Result containing the transformed value if the original Result was successful, otherwise returns the original error.</returns>
        public Result<TNew, TError> Map<TNew>(Func<T, TNew> mapper)
            where TNew : notnull => Result.Functor.Map(result, mapper);

        /// <summary>
        /// Transforms the error of the Result using the provided mapper function if the Result represents an error outcome.
        /// </summary>
        /// <typeparam name="TNewError">The type of the transformed error.</typeparam>
        /// <param name="mapper">A function that transforms the error of type TError to a new error of type TNewError.</param>
        /// <returns>A new Result containing the transformed error if the original Result was an error, otherwise returns the original value.</returns>
        public Result<T, TNewError> MapError<TNewError>(Func<TError, TNewError> mapper)
            where TNewError : notnull => Result.Functor.MapError(result, mapper);


        /// <summary>
        /// Transforms the value of the Result using a binder function that returns a new Result if the current Result represents a successful outcome.
        /// </summary>
        /// <typeparam name="TNew">The type of the new Result's value.</typeparam>
        /// <param name="binder">A function that transforms the current value into a new Result.</param>
        /// <returns>A new Result containing the transformed value if the original Result was successful, otherwise returns the original error.</returns>
        public TResult Match<TResult>(
            Func<T, TResult> matchOk,
            Func<TError, TResult> matchError
        )
            where TResult : notnull => Result.Functor.Match(result, matchOk, matchError);

        /// <summary>
        /// Transforms the value of the Result using a binder function that returns a new Result if the current Result represents a successful outcome.
        /// </summary>
        /// <typeparam name="TNew">The type of the new Result's value.</typeparam>
        /// <param name="binder">A function that transforms the current value into a new Result.</param>
        /// <returns>A new Result containing the transformed value if the original Result was successful, otherwise returns the original error.</returns>
        public Result<TNew, TError> Bind<TNew>(Func<T, Result<TNew, TError>> binder)
            where TNew : notnull => Result.Monad.Bind(result, binder);

        /// <summary>
        /// Transforms the error of the Result using a binder function that returns a new Result if the current Result represents an error outcome.
        /// </summary>
        /// <typeparam name="TNewError">The type of the transformed error.</typeparam>
        /// <param name="binder">A function that transforms the current error into a new Result.</param>
        /// <returns>A new Result containing the transformed error if the original Result was an error, otherwise returns the original value.</returns>
        public Result<T, TNewError> BindError<TNewError>(
            Func<TError, Result<T, TNewError>> binder
        )
            where TNewError : notnull => Result.Monad.BindError(result, binder);

        /// <summary>
        /// Applies a wrapped mapper function to the current Result, transforming the value if both the mapper and the current Result are successful.
        /// </summary>
        /// <typeparam name="TNew">The type of the transformed value.</typeparam>
        /// <param name="wrapperMapper">A Result containing a mapper function that transforms the value of type T to a new value of type TNew.</param>
        /// <returns>A new Result containing the transformed value if both the current Result and the mapper are successful, otherwise returns the first encountered error.</returns>
        public Result<TNew, TError> Apply<TNew>(
            Result<Func<T, TNew>, TError> wrapperMapper
        )
            where TNew : notnull => Result.Applicative.Apply(result, wrapperMapper);
    }

    extension<T, TError>(Result<Result<T, TError>, TError> nestedResult)
        where T : notnull
        where TError : notnull
    {
        /// <summary>
        /// Flattens a nested Result by unwrapping the inner Result if the outer Result is successful.
        /// </summary>
        /// <returns>A Result containing the inner value if both the outer and inner Results are successful, otherwise returns the first encountered error.</returns>
        public Result<T, TError> Flatten() => Result.Monad.Flatten(nestedResult);
    }

    extension<T1, T2, TError>(Result<(T1, T2), TError> result)
        where T1 : notnull
        where T2 : notnull
        where TError : notnull
    {
        public Result<TNew, TError> Map<TNew>(Func<T1, T2, TNew> mapper)
            where TNew : notnull => Result.Functor.Map(result, mapper);

        public Result<TNew, TError> Bind<TNew>(
            Func<T1, T2, Result<TNew, TError>> binder
        )
            where TNew : notnull => Result.Monad.Bind(result, binder);
    }

    extension<T>(Result<T, Exception> result)
        where T : notnull
    {
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
        public T ValueOrThrow => result switch
        {
            { IsOk: true, Value: var value } => value,
            { IsError: true, ErrorValue: var exception } => throw exception,
        };
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
    }
}