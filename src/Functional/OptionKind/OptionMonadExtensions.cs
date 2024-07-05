using Monads.Traits;

namespace Monads.OptionKind;

public static class OptionMonadExtensions
{
    internal static IMonad<OptionType<T>> Create<T>(T? value) where T : notnull =>
        value is not null ? CreateSome(value) : CreateNone<T>();

    internal static IMonad<OptionType<T>> CreateSome<T>(T value) where T : notnull => new Monad<OptionType<T>>(value);

    internal static IMonad<OptionType<T>> CreateNone<T>() where T : notnull =>
        new Monad<OptionType<T>>(new OptionType<T>.None());

    public static IMonad<OptionType<TNew>> Bind<T, TNew>(this IMonad<OptionType<T>> monad,
        Func<T, IMonad<OptionType<TNew>>> func) where T : notnull where TNew : notnull => monad.Type switch
    {
        OptionType<T>.Some some => func(some),
        _ => new Monad<OptionType<TNew>>(new OptionType<TNew>.None())
    };

    public static async Task<IMonad<OptionType<TNew>>> BindAsync<T, TNew>(this Task<IMonad<OptionType<T>>> monadTask,
        Func<T, Task<IMonad<OptionType<TNew>>>> func) where T : notnull where TNew : notnull =>
        (await monadTask).Type switch
        {
            OptionType<T>.Some some => await func(some),
            _ => new Monad<OptionType<TNew>>(new OptionType<TNew>.None())
        };
}