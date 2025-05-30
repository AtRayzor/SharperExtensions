using DotNetCoreFunctional.Option;

namespace DotNetCoreFunctional.Async;

public static class OptionAsyncExtensions
{
    extension<T>(T? value)
        where T : notnull
    {
        /// <summary>
        /// Converts a nullable value to an <see cref="OptionAsync{T}"/>.
        /// </summary>
        /// <returns>An <see cref="OptionAsync{T}"/> representing the nullable value.</returns>
        public OptionAsync<T> ToOptionAsync() => OptionAsync.Create(value);
    }

    extension<T>(Option<T> option)
        where T : notnull
    {
        /// <summary>
        /// Converts an <see cref="Option{T}"/> to an <see cref="OptionAsync{T}"/>.
        /// </summary>
        /// <returns>An <see cref="OptionAsync{T}"/> representing the converted option.</returns>
        public OptionAsync<T> ToOptionAsync() => OptionAsync.Create(option);
    }

    extension<T>(Async<T> async)
        where T : notnull
    {
        /// <summary>
        /// Converts an <see cref="Async{T}"/> to an <see cref="OptionAsync{T}"/>.
        /// </summary>
        /// <returns>An <see cref="OptionAsync{T}"/> representing the converted async value.</returns>
        public OptionAsync<T> ToOptionAsync() => OptionAsync.Lift(async);
    }

    extension<T>(Async<Option<T>> asyncOption)
        where T : notnull
    {
        /// <summary>
        /// Converts an <see cref="Async{Option{T}}"/> to an <see cref="OptionAsync{T}"/>.
        /// </summary>
        /// <returns>An <see cref="OptionAsync{T}"/> representing the converted async option.</returns>
        public OptionAsync<T> AsOptionAsync() => new(asyncOption);
    }
    
    extension<T>(Task<T?> task)
        where T : notnull
    {
        /// <summary>
        /// Converts a <see cref="Task{T?}"/> to an <see cref="OptionAsync{T}"/>.
        /// </summary>
        /// <returns>An <see cref="OptionAsync{T}"/> representing the converted task.</returns>
        public OptionAsync<T> ToOptionAsync() => OptionAsync.FromTask(task);
    }
    
    
    extension<T>(ValueTask<T?> valueTask)
        where T : notnull
    {
        /// <summary>
        /// Converts a <see cref="ValueTask{T?}"/> to an <see cref="OptionAsync{T}"/>.
        /// </summary>
        /// <returns>An <see cref="OptionAsync{T}"/> representing the converted value task.</returns>
        public OptionAsync<T> ToOptionAsync() => OptionAsync.FromValueTask(valueTask);
    }
    
    extension<T>(OptionAsync<T> optionAsync)
        where T : notnull
    {
        /// <summary>
        /// Maps the value of an <see cref="OptionAsync{T}"/> to a new value using the provided mapper function.
        /// </summary>
        /// <typeparam name="TNew">The type of the mapped value.</typeparam>
        /// <param name="mapper">A function to transform the contained value.</param>
        /// <returns>An <see cref="OptionAsync{TNew}"/> with the mapped value.</returns>
        public OptionAsync<TNew> Map<TNew>(Func<T, TNew> mapper)
            where TNew : notnull => OptionAsync.Map(optionAsync, mapper);


        /// <summary>
        /// Binds the value of an <see cref="OptionAsync{T}"/> to a new <see cref="OptionAsync{TNew}"/> using the provided binder function.
        /// </summary>
        /// <typeparam name="TNew">The type of the bound value.</typeparam>
        /// <param name="binder">A function that transforms the contained value into a new <see cref="OptionAsync{TNew}"/>.</param>
        /// <returns>An <see cref="OptionAsync{TNew}"/> resulting from applying the binder function.</returns>
        public OptionAsync<TNew> Bind<TNew>(Func<T, OptionAsync<TNew>> binder)
            where TNew : notnull => OptionAsync.Bind(optionAsync, binder);


        /// <summary>
        /// Applies a wrapped mapper function to the current <see cref="OptionAsync{T}"/>.
        /// </summary>
        /// <typeparam name="TNew">The type of the result after applying the mapper.</typeparam>
        /// <param name="wrappedMapper">An <see cref="OptionAsync{Func{T, TNew}}"/> containing the mapping function.</param>
        /// <returns>An <see cref="OptionAsync{TNew}"/> resulting from applying the mapper function.</returns>
        public OptionAsync<TNew> Apply<TNew>(OptionAsync<Func<T, TNew>> wrappedMapper)
            where TNew : notnull => OptionAsync.Apply(optionAsync, wrappedMapper);

        /// <summary>
        /// Matches the value of an <see cref="OptionAsync{T}"/> by applying either a mapping function for the some case or a default function for the none case.
        /// </summary>
        /// <typeparam name="TResult">The type of the result returned by both mapping functions.</typeparam>
        /// <param name="matchSomeAsync">A function to transform the contained value when the option is some.</param>
        /// <param name="matchNoneAsync">A function to provide a default value when the option is none.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of matching the option.</returns>
        public Task<TResult> MatchAsync<TResult>(
            Func<T, Task<TResult>> matchSomeAsync,
            Func<Task<TResult>> matchNoneAsync
        )
            where TResult : notnull =>
            OptionAsync.Unsafe.MatchAsync(optionAsync, matchSomeAsync, matchNoneAsync);

    /// <summary>
    /// Retrieves the value of the <see cref="OptionAsync{T}" /> as a nullable
    /// value, or returns the default value if the option is none.
    /// </summary>
    /// <returns>
    /// A <see cref="ValueTask{T}" /> representing the nullable value or
    /// default.
    /// </returns>
    public ValueTask<T?> ValueOrDefaultAsync =>
        OptionAsync.Unsafe.GetValueOrDefaultAsync(optionAsync);

    /// <summary>
    /// Retrieves the value of the <see cref="OptionAsync{T}" /> or throws a
    /// specified exception if the option is none.
    /// </summary>
    /// <typeparam name="TException">
    /// The type of exception to throw when the
    /// option is none.
    /// </typeparam>
    /// <returns>
    /// A <see cref="Task" /> representing the asynchronous operation of
    /// retrieving the value.
    /// </returns>
    /// <exception cref="TException">Thrown when the option contains no value.</exception>
    public Task GetValueOrThrowAsync<TException>()
        where TException : Exception =>
        OptionAsync.Unsafe.GetValueOrThrowAsync<T, TException>(optionAsync);

    /// <summary>
    /// Executes an asynchronous function if the
    /// <see cref="OptionAsync{T}" /> is some.
    /// </summary>
    /// <param name="asyncFunc">
    /// An asynchronous function to execute when the
    /// option contains a value.
    /// </param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    public Task IfSomeAsync(Func<T, Task> asyncFunc) =>
        OptionAsync.Unsafe.IfSomeAsync(optionAsync, asyncFunc);

    /// <summary>
    /// Executes an asynchronous function if the
    /// <see cref="OptionAsync{T}" /> is none.
    /// </summary>
    /// <param name="asyncFunc">
    /// An asynchronous function to execute when the
    /// option is none.
    /// </param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    public Task IfNoneAsync(Func<Task> asyncFunc) =>
        OptionAsync.Unsafe.IfNoneAsync(optionAsync, asyncFunc);

    /// <summary>
    /// Executes an asynchronous function based on whether the
    /// <see cref="OptionAsync{T}" /> is some or none.
    /// </summary>
    /// <param name="someFunc">
    /// An asynchronous function to execute when the option
    /// contains a value.
    /// </param>
    /// <param name="noneFunc">
    /// An asynchronous function to execute when the option
    /// is none.
    /// </param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    public Task DoAsync(Func<T, Task> someFunc, Func<Task> noneFunc) =>
        OptionAsync.Unsafe.DoAsync(optionAsync, someFunc, noneFunc);
}

    extension<T>(OptionAsync<T>)
        where T : notnull
    {
        public static OptionAsync<T> FromTask(Task<T?> task) =>
            OptionAsync.FromTask(task);

        public static OptionAsync<T> FromValueTask(ValueTask<T?> task) =>
            OptionAsync.FromValueTask(task);
    }

    extension<T1, T2>(OptionAsync<(T1, T2)> tupleOptionAsync)
        where T1 : notnull
        where T2 : notnull
    {
        /// <summary>
        /// Maps the values of a two-element tuple option to a new option using the provided mapping function.
        /// </summary>
        /// <typeparam name="TNew">The type of the resulting option's value.</typeparam>
        /// <param name="mapper">A function that transforms the two tuple elements into a new value.</param>
        /// <returns>A new <see cref="OptionAsync{TNew}"/> with the mapped value.</returns>
        public OptionAsync<TNew> Map<TNew>(Func<T1, T2, TNew> mapper)
            where TNew : notnull => OptionAsync.Map(tupleOptionAsync, mapper);
            
        /// <summary>
        /// Binds the values of a two-element tuple option to a new option using the provided binding function.
        /// </summary>
        /// <typeparam name="TNew">The type of the resulting option's value.</typeparam>
        /// <param name="binder">A function that transforms the two tuple elements into a new option.</param>
        /// <returns>A new <see cref="OptionAsync{TNew}"/> with the bound value.</returns>
        public OptionAsync<TNew> Bind<TNew>(
            Func<T1, T2, OptionAsync<TNew>> binder
            )
            where TNew : notnull => OptionAsync.Bind(tupleOptionAsync, binder);
        }
    
    extension<T1, T2, T3>(OptionAsync<(T1, T2, T3)> tupleOptionAsync)
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
    {
        /// <summary>
        /// Maps the values of a three-element tuple option to a new option using the provided mapping function.
        /// </summary>
        /// <typeparam name="TNew">The type of the resulting option's value.</typeparam>
        /// <param name="mapper">A function that transforms the three tuple elements into a new value.</param>
        /// <returns>A new <see cref="OptionAsync{TNew}"/> with the mapped value.</returns>
        public OptionAsync<TNew> Map<TNew>(Func<T1, T2, T3, TNew> mapper)
            where TNew : notnull => OptionAsync.Map(tupleOptionAsync, mapper);
            
        /// <summary>
        /// Binds the values of a three-element tuple option to a new option using the provided binding function.
        /// </summary>
        /// <typeparam name="TNew">The type of the resulting option's value.</typeparam>
        /// <param name="binder">A function that transforms the three tuple elements into a new option.</param>
        /// <returns>A new <see cref="OptionAsync{TNew}"/> with the bound value.</returns>
        public OptionAsync<TNew> Bind<TNew>(
            Func<T1, T2, T3, OptionAsync<TNew>> binder
        )
            where TNew : notnull => OptionAsync.Bind(tupleOptionAsync, binder);   
    }

    extension<T1, T2, T3, T4>(OptionAsync<(T1, T2, T3, T4)> tupleOptionAsync)
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull
    {
        /// <summary>
        /// Maps the values of a four-element tuple option to a new option using the provided mapping function.
        /// </summary>
        /// <typeparam name="TNew">The type of the resulting option's value.</typeparam>
        /// <param name="mapper">A function that transforms the four tuple elements into a new value.</param>
        /// <returns>A new <see cref="OptionAsync{TNew}"/> with the mapped value.</returns>
        public OptionAsync<TNew> Map<TNew>(Func<T1, T2, T3, T4, TNew> mapper)
            where TNew : notnull => OptionAsync.Map(tupleOptionAsync, mapper);
            
        /// <summary>
        /// Binds the values of a four-element tuple option to a new option using the provided binding function.
        /// </summary>
        /// <typeparam name="TNew">The type of the resulting option's value.</typeparam>
        /// <param name="binder">A function that transforms the four tuple elements into a new option.</param>
        /// <returns>A new <see cref="OptionAsync{TNew}"/> with the bound value.</returns>
        public OptionAsync<TNew> Bind<TNew>(
            Func<T1, T2, T3, T4, OptionAsync<TNew>> binder
        )
            where TNew : notnull => OptionAsync.Bind(tupleOptionAsync, binder);      
    }
}