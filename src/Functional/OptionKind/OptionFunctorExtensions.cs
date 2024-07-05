using Monads.Traits;

namespace Monads.OptionKind;

public static class OptionFunctorExtensions
{
    internal static Functor<OptionType<T>> Create<T>(T? value) where T : notnull
        => value is not null ? new OptionType<T>.Some(value) : new OptionType<T>.None();
    
    internal static Functor<OptionType<T>> CreateSome<T>(T value) where T : notnull
        =>  new OptionType<T>.Some(value);
    
    internal static Functor<OptionType<T>> CreateNone<T>() where T : notnull
        =>  new OptionType<T>.None();
    

    public static IFunctor<OptionType<TNew>> Map<T, TNew>(this IFunctor<OptionType<T>> functor, Func<T, TNew> map)
        where T : notnull where TNew : notnull => functor.Type switch
    {
        OptionType<T>.Some some =>  new Functor<OptionType<TNew>>(map(some)),
        _ =>  new Functor<OptionType<TNew>>(new OptionType<TNew>.None())
    };

    public static async Task<IFunctor<OptionType<TNew>>> MapAsync<T, TNew>(
        this Task<IFunctor<OptionType<T>>> functorTask,
        Func<T, TNew> map)
        where T : notnull where TNew : notnull => (await functorTask).Type switch
    {
        OptionType<T>.Some some => new Functor<OptionType<TNew>>(map(some)),
        _ => new Functor<OptionType<TNew>>(new OptionType<TNew>.None())
    };

    public static async Task<IFunctor<OptionType<TNew>>> MapAsync<T, TNew>(
        this Task<IFunctor<OptionType<T>>> functorTask,
        Func<T, Task<TNew>> map)
        where T : notnull where TNew : notnull => (await functorTask).Type switch
    {
        OptionType<T>.Some some => new Functor<OptionType<TNew>>(await map(some)),
        _ => new Functor<OptionType<TNew>>(new OptionType<TNew>.None())
    };
}