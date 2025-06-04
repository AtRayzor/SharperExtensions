using System.Runtime.CompilerServices;

namespace SharperExtensions.Async;

/// <summary>
/// Represents an asynchronous optional value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the optional value, which must be a non-nullable type.</typeparam>
/// <remarks>
/// Provides a way to handle potentially absent values in an asynchronous context.
/// </remarks>
[AsyncMethodBuilder(typeof(OptionAsyncMethodBuilder<>))]
public readonly struct OptionAsync<T>
    where T : notnull
{
    /// <summary>
    /// Gets the result of the asynchronous option, returning the option if available or <see cref="Option{T}.None"/> if not.
    /// </summary>
    /// <returns>An <see cref="Option{T}"/> representing the result of the asynchronous operation.</returns>
    public Option<T> Result =>
        WrappedOption.Result.Match(option => option, _ => Option<T>.None);

    internal Async<Option<T>> WrappedOption { get; }

    internal OptionAsync(
        Async<Option<T>> wrappedOption,
        CancellationToken token = default
    )
    {
        WrappedOption = wrappedOption;
    }

    internal OptionAsync(Option<T> option) : this(new Async<Option<T>>(option)) { }

    internal OptionAsync(T? value)
        : this(
            new Async<Option<T>>(
                value is not null ? Option<T>.Some(value) : Option<T>.None
            )
        ) { }

    /// <summary>
    /// Returns an awaiter for the asynchronous optional value, allowing it to be awaited in an asynchronous context.
    /// </summary>
    /// <returns>An <see cref="AsyncAwaiter{TResult, T}"/> that can be used to await the optional value.</returns>
    public AsyncAwaiter<Option<T>, Option<T>> GetAwaiter() =>
        WrappedOption
            .ConfigureAwaitInternal(
                new OptionAsyncResultProvider<T>(WrappedOption.State)
            );

    public static implicit operator OptionAsync<T>(Task<T?> task) =>
        OptionAsync.FromTask(task);

    public static implicit operator OptionAsync<T>(ValueTask<T?> valueTask) =>
        OptionAsync.FromValueTask(valueTask);

    public static implicit operator OptionAsync<T>(T? value) =>
        OptionAsync.Create(value);
}

/// <summary>
/// Provides static utility methods for working with asynchronous optional values.
/// </summary>
/// <remarks>
/// This class contains extension and factory methods for creating, transforming, and manipulating <see cref="OptionAsync{T}"/> instances.
/// </remarks>
public static class OptionAsync
{
    /// <summary>
    /// Creates an <see cref="OptionAsync{T}"/> from a nullable value.
    /// </summary>
    /// <typeparam name="T">The type of the optional value, which must be non-nullable.</typeparam>
    /// <param name="value">The nullable value to convert to an <see cref="OptionAsync{T}"/>.</param>
    /// <returns>An <see cref="OptionAsync{T}"/> representing the provided value.</returns>
    public static OptionAsync<T> Create<T>(T? value)
        where T : notnull => new(value);

    /// <summary>
    /// Creates an <see cref="OptionAsync{T}"/> from an existing <see cref="Option{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the optional value, which must be non-nullable.</typeparam>
    /// <param name="value">The <see cref="Option{T}"/> to convert to an <see cref="OptionAsync{T}"/>.</param>
    /// <returns>An <see cref="OptionAsync{T}"/> representing the provided option.</returns>
    public static OptionAsync<T> Create<T>(Option<T> value)
        where T : notnull => new(value);

    /// <summary>
    /// Converts an <see cref="Async{T}"/> to an <see cref="OptionAsync{T}"/> by wrapping the value in an <see cref="Option{T}.Some"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value, which must be non-nullable.</typeparam>
    /// <param name="async">The asynchronous value to lift into an optional context.</param>
    /// <returns>An <see cref="OptionAsync{T}"/> containing the value from the input async.</returns>
    public static OptionAsync<T> Lift<T>(Async<T> async)
        where T : notnull => new(async.Bind(value => Async.New(Option<T>.Some(value))));


    /// <summary>
    /// Creates an <see cref="OptionAsync{T}"/> from a <see cref="Task{T}"/> by converting the task's result to an <see cref="Option{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the task's result, which must be non-nullable.</typeparam>
    /// <param name="task">The task to convert to an <see cref="OptionAsync{T}"/>.</param>
    /// <returns>An <see cref="OptionAsync{T}"/> representing the task's result.</returns>
    public static OptionAsync<T> FromTask<T>(Task<T?> task)
        where T : notnull =>
        new(Async<Option<T>>.FromTask(task.ContinueWith(t => t.Result.ToOption())));

    /// <summary>
    /// Creates an <see cref="OptionAsync{T}"/> from a <see cref="ValueTask{T}"/> by converting the task's result to an <see cref="Option{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value task's result, which must be non-nullable.</typeparam>
    /// <param name="valueTask">The value task to convert to an <see cref="OptionAsync{T}"/>.</param>
    /// <returns>An <see cref="OptionAsync{T}"/> representing the value task's result.</returns>
    public static OptionAsync<T> FromValueTask<T>(ValueTask<T?> valueTask)
        where T : notnull
    {
        var wrappedOption = valueTask switch
        {
            { IsCompleted: true } or { IsCanceled: true } =>
                Async.New<Option<T>>(valueTask.Result),
            _ => Async<Option<T>>
                .FromTask(
                    valueTask
                        .AsTask()
                        .ContinueWith(t => t.Result.ToOption())
                ),
        };

        return new OptionAsync<T>(wrappedOption);
    }

    /// <summary>
    /// Maps the value of an <see cref="OptionAsync{T}"/> to a new value using the provided mapper function.
    /// </summary>
    /// <typeparam name="T">The type of the source optional value, which must be non-nullable.</typeparam>
    /// <typeparam name="TNew">The type of the mapped optional value, which must be non-nullable.</typeparam>
    /// <param name="optionAsync">The source <see cref="OptionAsync{T}"/> to map.</param>
    /// <param name="mapper">A function to transform the optional value.</param>
    /// <returns>An <see cref="OptionAsync{TNew}"/> with the mapped value.</returns>
    public static OptionAsync<TNew> Map<T, TNew>(
        OptionAsync<T> optionAsync,
        Func<T, TNew> mapper
    )
        where T : notnull
        where TNew : notnull =>
        new(optionAsync.WrappedOption.Map(option => option.Map(mapper)));

    /// <summary>
    /// Maps the values of an <see cref="OptionAsync{T}"/> containing a 2-tuple to a new value using the provided mapper function.
    /// </summary>
    /// <typeparam name="T1">The type of the first tuple element, which must be non-nullable.</typeparam>
    /// <typeparam name="T2">The type of the second tuple element, which must be non-nullable.</typeparam>
    /// <typeparam name="TNew">The type of the mapped optional value, which must be non-nullable.</typeparam>
    /// <param name="tupleOptionAsync">The source <see cref="OptionAsync{T}"/> containing a 2-tuple to map.</param>
    /// <param name="mapper">A function to transform the tuple's elements.</param>
    /// <returns>An <see cref="OptionAsync{TNew}"/> with the mapped value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OptionAsync<TNew> Map<T1, T2, TNew>(
        OptionAsync<(T1, T2)> tupleOptionAsync,
        Func<T1, T2, TNew> mapper
    )
        where T1 : notnull
        where T2 : notnull
        where TNew : notnull =>
        Map(tupleOptionAsync, tuple => mapper(tuple.Item1, tuple.Item2));

    /// <summary>
    /// Maps the values of an <see cref="OptionAsync{T}"/> containing a 3-tuple to a new value using the provided mapper function.
    /// </summary>
    /// <typeparam name="T1">The type of the first tuple element, which must be non-nullable.</typeparam>
    /// <typeparam name="T2">The type of the second tuple element, which must be non-nullable.</typeparam>
    /// <typeparam name="T3">The type of the third tuple element, which must be non-nullable.</typeparam>
    /// <typeparam name="TNew">The type of the mapped optional value, which must be non-nullable.</typeparam>
    /// <param name="tupleOptionAsync">The source <see cref="OptionAsync{T}"/> containing a 3-tuple to map.</param>
    /// <param name="mapper">A function to transform the tuple's elements.</param>
    /// <returns>An <see cref="OptionAsync{TNew}"/> with the mapped value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OptionAsync<TNew> Map<T1, T2, T3, TNew>(
        OptionAsync<(T1, T2, T3)> tupleOptionAsync,
        Func<T1, T2, T3, TNew> mapper
    )
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where TNew : notnull =>
        Map(tupleOptionAsync, tuple => mapper(tuple.Item1, tuple.Item2, tuple.Item3));

    /// <summary>
    /// Maps the values of an <see cref="OptionAsync{T}"/> containing a 4-tuple to a new value using the provided mapper function.
    /// </summary>
    /// <typeparam name="T1">The type of the first tuple element, which must be non-nullable.</typeparam>
    /// <typeparam name="T2">The type of the second tuple element, which must be non-nullable.</typeparam>
    /// <typeparam name="T3">The type of the third tuple element, which must be non-nullable.</typeparam>
    /// <typeparam name="T4">The type of the fourth tuple element, which must be non-nullable.</typeparam>
    /// <typeparam name="TNew">The type of the mapped optional value, which must be non-nullable.</typeparam>
    /// <param name="tupleOptionAsync">The source <see cref="OptionAsync{T}"/> containing a 4-tuple to map.</param>
    /// <param name="mapper">A function to transform the tuple's elements.</param>
    /// <returns>An <see cref="OptionAsync{TNew}"/> with the mapped value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OptionAsync<TNew> Map<T1, T2, T3, T4, TNew>(
        OptionAsync<(T1, T2, T3, T4)> tupleOptionAsync,
        Func<T1, T2, T3, T4, TNew> mapper
    )
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull
        where TNew : notnull =>
        Map(
            tupleOptionAsync,
            tuple => mapper(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4)
        );

    /// <summary>
    /// Binds the value of an <see cref="OptionAsync{T}"/> to a new <see cref="OptionAsync{TNew}"/> using the provided binder function.
    /// </summary>
    /// <typeparam name="T">The type of the source optional value, which must be non-nullable.</typeparam>
    /// <typeparam name="TNew">The type of the bound optional value, which must be non-nullable.</typeparam>
    /// <param name="optionAsync">The source <see cref="OptionAsync{T}"/> to bind.</param>
    /// <param name="binder">A function to transform the source value into a new <see cref="OptionAsync{TNew}"/>.</param>
    /// <returns>An <see cref="OptionAsync{TNew}"/> with the bound value.</returns>
    public static OptionAsync<TNew> Bind<T, TNew>(
        OptionAsync<T> optionAsync,
        Func<T, OptionAsync<TNew>> binder
    )
        where T : notnull
        where TNew : notnull =>
        new(
            optionAsync.WrappedOption.Bind(option =>
                option.Match(
                    value =>
                        binder(value).WrappedOption,
                    () => Async.New(Option<TNew>.None)
                )
            )
        );

    /// <summary>
    /// Binds the values of an <see cref="OptionAsync{T}"/> containing a 2-tuple to a new <see cref="OptionAsync{TNew}"/> using the provided mapper function.
    /// </summary>
    /// <typeparam name="T1">The type of the first tuple element, which must be non-nullable.</typeparam>
    /// <typeparam name="T2">The type of the second tuple element, which must be non-nullable.</typeparam>
    /// <typeparam name="TNew">The type of the bound optional value, which must be non-nullable.</typeparam>
    /// <param name="tupleOption">The source <see cref="OptionAsync{T}"/> containing a 2-tuple to bind.</param>
    /// <param name="mapper">A function to transform the tuple's elements into a new <see cref="OptionAsync{TNew}"/>.</param>
    /// <returns>An <see cref="OptionAsync{TNew}"/> with the bound value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OptionAsync<TNew> Bind<T1, T2, TNew>(
        OptionAsync<(T1, T2)> tupleOption,
        Func<T1, T2, OptionAsync<TNew>> mapper
    )
        where T1 : notnull
        where T2 : notnull
        where TNew : notnull =>
        Bind(tupleOption, tuple => mapper(tuple.Item1, tuple.Item2));

    /// <summary>
    /// Binds the values of an <see cref="OptionAsync{T}"/> containing a 3-tuple to a new <see cref="OptionAsync{TNew}"/> using the provided mapper function.
    /// </summary>
    /// <typeparam name="T1">The type of the first tuple element, which must be non-nullable.</typeparam>
    /// <typeparam name="T2">The type of the second tuple element, which must be non-nullable.</typeparam>
    /// <typeparam name="T3">The type of the third tuple element, which must be non-nullable.</typeparam>
    /// <typeparam name="TNew">The type of the bound optional value, which must be non-nullable.</typeparam>
    /// <param name="optionTuple">The source <see cref="OptionAsync{T}"/> containing a 3-tuple to bind.</param>
    /// <param name="mapper">A function to transform the tuple's elements into a new <see cref="OptionAsync{TNew}"/>.</param>
    /// <returns>An <see cref="OptionAsync{TNew}"/> with the bound value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OptionAsync<TNew> Bind<T1, T2, T3, TNew>(
        OptionAsync<(T1, T2, T3)> optionTuple,
        Func<T1, T2, T3, OptionAsync<TNew>> mapper
    )
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where TNew : notnull =>
        Bind(optionTuple, tuple => mapper(tuple.Item1, tuple.Item2, tuple.Item3));

    /// <summary>
    /// Binds the values of an <see cref="OptionAsync{T}"/> containing a 4-tuple to a new <see cref="OptionAsync{TNew}"/> using the provided mapper function.
    /// </summary>
    /// <typeparam name="T1">The type of the first tuple element, which must be non-nullable.</typeparam>
    /// <typeparam name="T2">The type of the second tuple element, which must be non-nullable.</typeparam>
    /// <typeparam name="T3">The type of the third tuple element, which must be non-nullable.</typeparam>
    /// <typeparam name="T4">The type of the fourth tuple element, which must be non-nullable.</typeparam>
    /// <typeparam name="TNew">The type of the bound optional value, which must be non-nullable.</typeparam>
    /// <param name="optionTuple">The source <see cref="OptionAsync{T}"/> containing a 4-tuple to bind.</param>
    /// <param name="mapper">A function to transform the tuple's elements into a new <see cref="OptionAsync{TNew}"/>.</param>
    /// <returns>An <see cref="OptionAsync{TNew}"/> with the bound value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OptionAsync<TNew> Bind<T1, T2, T3, T4, TNew>(
        OptionAsync<(T1, T2, T3, T4)> optionTuple,
        Func<T1, T2, T3, T4, OptionAsync<TNew>> mapper
    )
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull
        where TNew : notnull =>
        Bind(
            optionTuple,
            tuple => mapper(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4)
        );

    /// <summary>
    /// Applies a wrapped mapper function to an asynchronous option, transforming the value if both the option and mapper are present.
    /// </summary>
    /// <typeparam name="T">The type of the source optional value, which must be non-nullable.</typeparam>
    /// <typeparam name="TNew">The type of the transformed optional value, which must be non-nullable.</typeparam>
    /// <param name="optionAsync">The source <see cref="OptionAsync{T}"/> to transform.</param>
    /// <param name="wrappedMapper">An <see cref="OptionAsync{T}"/> containing a mapping function.</param>
    /// <returns>An <see cref="OptionAsync{TNew}"/> with the transformed value, or None if either input is None.</returns>
    public static OptionAsync<TNew> Apply<T, TNew>(
        OptionAsync<T> optionAsync,
        OptionAsync<Func<T, TNew>> wrappedMapper
    )
        where T : notnull
        where TNew : notnull =>
            Bind(wrappedMapper, mapper => Map(optionAsync, mapper));

    /// <summary>
    /// Provides unsafe operations and methods for the <see cref="OptionAsync{T}"/> type that should be used with caution.
    /// </summary>
    /// <remarks>
    /// This class contains methods that bypass normal safety checks and may lead to runtime exceptions if used incorrectly.
    /// Use these methods only when you are absolutely certain about the state of the option.
    /// </remarks>
    public static class Unsafe
    {
        /// <summary>
        /// Asynchronously matches an <see cref="OptionAsync{T}"/> by applying either a mapping function for the Some case or a default function for the None case.
        /// </summary>
        /// <typeparam name="T">The type of the optional value, which must be non-nullable.</typeparam>
        /// <typeparam name="TResult">The result type of the mapping functions, which must be non-nullable.</typeparam>
        /// <param name="optionAsync">The source <see cref="OptionAsync{T}"/> to match.</param>
        /// <param name="matchSomeAsync">An asynchronous function to transform the Some value into a result.</param>
        /// <param name="matchNoneAsync">An asynchronous function to provide a default result when no value is present.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous result of matching the option.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<TResult> MatchAsync<T, TResult>(
            OptionAsync<T> optionAsync,
            Func<T, Task<TResult>> matchSomeAsync,
            Func<Task<TResult>> matchNoneAsync
        )
            where T : notnull
            where TResult : notnull
        {
            return await (await optionAsync)
                .Match(
                    value => matchSomeAsync(value),
                    () => matchNoneAsync()
                )
                .ConfigureAwait(ConfigureAwaitOptions.None);
        }

        /// <summary>
        /// Asynchronously retrieves the value from an <see cref="OptionAsync{T}"/> or returns the default value if no value is present.
        /// </summary>
        /// <typeparam name="T">The type of the optional value, which must be non-nullable.</typeparam>
        /// <param name="optionAsync">The source <see cref="OptionAsync{T}"/> to extract the value from.</param>
        /// <returns>A <see cref="ValueTask{T}"/> representing the asynchronous operation of retrieving the value or default.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask<T?> GetValueOrDefaultAsync<T>(
            OptionAsync<T> optionAsync
        )
            where T : notnull => (await optionAsync).ValueOrDefault;

        /// <summary>
        /// Asynchronously retrieves the value from an <see cref="OptionAsync{T}"/> or throws a specified exception if no value is present.
        /// </summary>
        /// <typeparam name="T">The type of the optional value, which must be non-nullable.</typeparam>
        /// <typeparam name="TException">The type of exception to throw when no value is present.</typeparam>
        /// <param name="optionAsync">The source <see cref="OptionAsync{T}"/> to extract the value from.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation of retrieving the value.</returns>
        /// <exception cref="TException">Thrown when the <see cref="OptionAsync{T}"/> contains no value.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task GetValueOrThrowAsync<T, TException>(
            OptionAsync<T> optionAsync
        )
            where T : notnull where TException : Exception =>
            (await optionAsync).GetValueOrThrow<T, TException>();

        /// <summary>
        /// Asynchronously executes a function if the <see cref="OptionAsync{T}"/> contains a value.
        /// </summary>
        /// <typeparam name="T">The type of the optional value, which must be non-nullable.</typeparam>
        /// <param name="optionAsync">The source <see cref="OptionAsync{T}"/> to check.</param>
        /// <param name="asyncFunc">An asynchronous function to execute when a value is present.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task IfSomeAsync<T>(
            OptionAsync<T> optionAsync,
            Func<T, Task> asyncFunc
        )
            where T : notnull
        {
            if (!(await optionAsync).TryGetValue(out var value))
            {
                return;
            }

            await asyncFunc(value).ConfigureAwait(ConfigureAwaitOptions.None);
        }

        /// <summary>
        /// Asynchronously executes a function if the <see cref="OptionAsync{T}"/> contains no value.
        /// </summary>
        /// <typeparam name="T">The type of the optional value, which must be non-nullable.</typeparam>
        /// <param name="optionAsync">The source <see cref="OptionAsync{T}"/> to check.</param>
        /// <param name="asyncFunc">An asynchronous function to execute when no value is present.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task IfNoneAsync<T>(
            OptionAsync<T> optionAsync,
            Func<Task> asyncFunc
        ) where T : notnull
        {
            if (!(await optionAsync).IsNone)
            {
                return;
            }

            await asyncFunc().ConfigureAwait(ConfigureAwaitOptions.None);
        }

        /// <summary>
        /// Asynchronously executes a function based on whether the <see cref="OptionAsync{T}"/> contains a value.
        /// </summary>
        /// <typeparam name="T">The type of the optional value, which must be non-nullable.</typeparam>
        /// <param name="optionAsync">The source <see cref="OptionAsync{T}"/> to check.</param>
        /// <param name="someFunc">An asynchronous function to execute when a value is present.</param>
        /// <param name="noneFunc">An asynchronous function to execute when no value is present.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task DoAsync<T>(
            OptionAsync<T> optionAsync,
            Func<T, Task> someFunc,
            Func<Task> noneFunc
        ) where T : notnull
        {
            if (!(await optionAsync).TryGetValue(out var value))
            {
                await noneFunc();
                return;
            }

            await someFunc(value);          
        }
    }
}