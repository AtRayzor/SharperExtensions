namespace DotNetCoreFunctional.Operations;

/// <summary>
/// Provides utility methods for partial application of functions.
/// </summary>
public static class Lambda
{
    /// <summary>
    /// Partially applies the second argument of a function with two parameters,
    /// returning a new function that takes the first argument and returns the result.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="TResult">The return type of the function.</typeparam>
    /// <param name="func">The original function with two parameters.</param>
    /// <param name="arg2">The value to partially apply as the second argument.</param>
    /// <returns>A function that takes the first argument and returns the result.</returns>
    public static Func<T1, TResult> Partial<T1, T2, TResult>(Func<T1, T2, TResult> func, T2 arg2) =>
        arg1 => func(arg1, arg2);

    /// <summary>
    /// Partially applies the second and third arguments of a function with three parameters,
    /// returning a new function that takes the first argument and returns the result.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="T3">The type of the third argument.</typeparam>
    /// <typeparam name="TResult">The return type of the function.</typeparam>
    /// <param name="func">The original function with three parameters.</param>
    /// <param name="arg2">The value to partially apply as the second argument.</param>
    /// <param name="arg3">The value to partially apply as the third argument.</param>
    /// <returns>A function that takes the first argument and returns the result.</returns>
    public static Func<T1, TResult> Partial<T1, T2, T3, TResult>(
        Func<T1, T2, T3, TResult> func,
        T2 arg2,
        T3 arg3
    ) => arg1 => func(arg1, arg2, arg3);

    /// <summary>
    /// Partially applies the second, third, and fourth arguments of a function with four parameters,
    /// returning a new function that takes the first argument and returns the result.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="T3">The type of the third argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth argument.</typeparam>
    /// <typeparam name="TResult">The return type of the function.</typeparam>
    /// <param name="func">The original function with four parameters.</param>
    /// <param name="arg2">The value to partially apply as the second argument.</param>
    /// <param name="arg3">The value to partially apply as the third argument.</param>
    /// <param name="arg4">The value to partially apply as the fourth argument.</param>
    /// <returns>A function that takes the first argument and returns the result.</returns>
    public static Func<T1, TResult> Partial<T1, T2, T3, T4, TResult>(
        Func<T1, T2, T3, T4, TResult> func,
        T2 arg2,
        T3 arg3,
        T4 arg4
    ) => arg1 => func(arg1, arg2, arg3, arg4);

    /// <summary>
    /// Partially applies the second through fifth arguments of a function with five parameters,
    /// returning a new function that takes the first argument and returns the result.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="T3">The type of the third argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth argument.</typeparam>
    /// <typeparam name="T5">The type of the fifth argument.</typeparam>
    /// <typeparam name="TResult">The return type of the function.</typeparam>
    /// <param name="func">The original function with five parameters.</param>
    /// <param name="arg2">The value to partially apply as the second argument.</param>
    /// <param name="arg3">The value to partially apply as the third argument.</param>
    /// <param name="arg4">The value to partially apply as the fourth argument.</param>
    /// <param name="arg5">The value to partially apply as the fifth argument.</param>
    /// <returns>A function that takes the first argument and returns the result.</returns>
    public static Func<T1, TResult> Partial<T1, T2, T3, T4, T5, TResult>(
        Func<T1, T2, T3, T4, T5, TResult> func,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5
    ) => arg1 => func(arg1, arg2, arg3, arg4, arg5);

    /// <summary>
    /// Partially applies the second through sixth arguments of a function with six parameters,
    /// returning a new function that takes the first argument and returns the result.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="T3">The type of the third argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth argument.</typeparam>
    /// <typeparam name="T5">The type of the fifth argument.</typeparam>
    /// <typeparam name="T6">The type of the sixth argument.</typeparam>
    /// <typeparam name="TResult">The return type of the function.</typeparam>
    /// <param name="func">The original function with six parameters.</param>
    /// <param name="arg2">The value to partially apply as the second argument.</param>
    /// <param name="arg3">The value to partially apply as the third argument.</param>
    /// <param name="arg4">The value to partially apply as the fourth argument.</param>
    /// <param name="arg5">The value to partially apply as the fifth argument.</param>
    /// <param name="arg6">The value to partially apply as the sixth argument.</param>
    /// <returns>A function that takes the first argument and returns the result.</returns>
    public static Func<T1, TResult> Partial<T1, T2, T3, T4, T5, T6, TResult>(
        Func<T1, T2, T3, T4, T5, T6, TResult> func,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        T6 arg6
    ) => arg1 => func(arg1, arg2, arg3, arg4, arg5, arg6);

    /// <summary>
    /// Partially applies the second through seventh arguments of a function with seven parameters,
    /// returning a new function that takes the first argument and returns the result.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="T3">The type of the third argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth argument.</typeparam>
    /// <typeparam name="T5">The type of the fifth argument.</typeparam>
    /// <typeparam name="T6">The type of the sixth argument.</typeparam>
    /// <typeparam name="T7">The type of the seventh argument.</typeparam>
    /// <typeparam name="TResult">The return type of the function.</typeparam>
    /// <param name="func">The original function with seven parameters.</param>
    /// <param name="arg2">The value to partially apply as the second argument.</param>
    /// <param name="arg3">The value to partially apply as the third argument.</param>
    /// <param name="arg4">The value to partially apply as the fourth argument.</param>
    /// <param name="arg5">The value to partially apply as the fifth argument.</param>
    /// <param name="arg6">The value to partially apply as the sixth argument.</param>
    /// <param name="arg7">The value to partially apply as the seventh argument.</param>
    /// <returns>A function that takes the first argument and returns the result.</returns>
    public static Func<T1, TResult> Partial<T1, T2, T3, T4, T5, T6, T7, TResult>(
        Func<T1, T2, T3, T4, T5, T6, T7, TResult> func,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        T6 arg6,
        T7 arg7
    ) => arg1 => func(arg1, arg2, arg3, arg4, arg5, arg6, arg7);

    /// <summary>
    /// Partially applies the second through eighth arguments of a function with eight parameters,
    /// returning a new function that takes the first argument and returns the result.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="T3">The type of the third argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth argument.</typeparam>
    /// <typeparam name="T5">The type of the fifth argument.</typeparam>
    /// <typeparam name="T6">The type of the sixth argument.</typeparam>
    /// <typeparam name="T7">The type of the seventh argument.</typeparam>
    /// <typeparam name="T8">The type of the eighth argument.</typeparam>
    /// <typeparam name="TResult">The return type of the function.</typeparam>
    /// <param name="func">The original function with eight parameters.</param>
    /// <param name="arg2">The value to partially apply as the second argument.</param>
    /// <param name="arg3">The value to partially apply as the third argument.</param>
    /// <param name="arg4">The value to partially apply as the fourth argument.</param>
    /// <param name="arg5">The value to partially apply as the fifth argument.</param>
    /// <param name="arg6">The value to partially apply as the sixth argument.</param>
    /// <param name="arg7">The value to partially apply as the seventh argument.</param>
    /// <param name="arg8">The value to partially apply as the eighth argument.</param>
    /// <returns>A function that takes the first argument and returns the result.</returns>
    public static Func<T1, TResult> Partial<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
        Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> func,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        T6 arg6,
        T7 arg7,
        T8 arg8
    ) => arg1 => func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);

    /// <summary>
    /// Partially applies the second through ninth arguments of a function with nine parameters,
    /// returning a new function that takes the first argument and returns the result.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="T3">The type of the third argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth argument.</typeparam>
    /// <typeparam name="T5">The type of the fifth argument.</typeparam>
    /// <typeparam name="T6">The type of the sixth argument.</typeparam>
    /// <typeparam name="T7">The type of the seventh argument.</typeparam>
    /// <typeparam name="T8">The type of the eighth argument.</typeparam>
    /// <typeparam name="T9">The type of the ninth argument.</typeparam>
    /// <typeparam name="TResult">The return type of the function.</typeparam>
    /// <param name="func">The original function with nine parameters.</param>
    /// <param name="arg2">The value to partially apply as the second argument.</param>
    /// <param name="arg3">The value to partially apply as the third argument.</param>
    /// <param name="arg4">The value to partially apply as the fourth argument.</param>
    /// <param name="arg5">The value to partially apply as the fifth argument.</param>
    /// <param name="arg6">The value to partially apply as the sixth argument.</param>
    /// <param name="arg7">The value to partially apply as the seventh argument.</param>
    /// <param name="arg8">The value to partially apply as the eighth argument.</param>
    /// <param name="arg9">The value to partially apply as the ninth argument.</param>
    /// <returns>A function that takes the first argument and returns the result.</returns>
    public static Func<T1, TResult> Partial<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> func,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        T6 arg6,
        T7 arg7,
        T8 arg8,
        T9 arg9
    ) => arg1 => func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);

    /// <summary>
    /// Partially applies the second through tenth arguments of a function with ten parameters,
    /// returning a new function that takes the first argument and returns the result.
    /// </summary>
    /// <typeparam name="T1">The type of the first argument.</typeparam>
    /// <typeparam name="T2">The type of the second argument.</typeparam>
    /// <typeparam name="T3">The type of the third argument.</typeparam>
    /// <typeparam name="T4">The type of the fourth argument.</typeparam>
    /// <typeparam name="T5">The type of the fifth argument.</typeparam>
    /// <typeparam name="T6">The type of the sixth argument.</typeparam>
    /// <typeparam name="T7">The type of the seventh argument.</typeparam>
    /// <typeparam name="T8">The type of the eighth argument.</typeparam>
    /// <typeparam name="T9">The type of the ninth argument.</typeparam>
    /// <typeparam name="T10">The type of the tenth argument.</typeparam>
    /// <typeparam name="TResult">The return type of the function.</typeparam>
    /// <param name="func">The original function with ten parameters.</param>
    /// <param name="arg2">The value to partially apply as the second argument.</param>
    /// <param name="arg3">The value to partially apply as the third argument.</param>
    /// <param name="arg4">The value to partially apply as the fourth argument.</param>
    /// <param name="arg5">The value to partially apply as the fifth argument.</param>
    /// <param name="arg6">The value to partially apply as the sixth argument.</param>
    /// <param name="arg7">The value to partially apply as the seventh argument.</param>
    /// <param name="arg8">The value to partially apply as the eighth argument.</param>
    /// <param name="arg9">The value to partially apply as the ninth argument.</param>
    /// <param name="arg10">The value to partially apply as the tenth argument.</param>
    /// <returns>A function that takes the first argument and returns the result.</returns>
    public static Func<T1, TResult> Partial<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(
        Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> func,
        T2 arg2,
        T3 arg3,
        T4 arg4,
        T5 arg5,
        T6 arg6,
        T7 arg7,
        T8 arg8,
        T9 arg9,
        T10 arg10
    ) => arg1 => func(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
}
