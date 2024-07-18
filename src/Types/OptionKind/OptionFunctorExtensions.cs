using NetFunctional.Types.Traits;

namespace NetFunctional.Types.OptionKind;

public static class OptionFunctorExtensions
{
    public static IFunctor<OptionType<TNew>> Map<T, TNew>(
        this IFunctor<OptionType<T>> functor,
        Func<T, TNew> map
    )
        where T : notnull
        where TNew : notnull
    {
        return functor.Type switch
        {
            OptionType<T>.Some some => new Functor<OptionType<TNew>>(map(some)),
            _ => new Functor<OptionType<TNew>>(new OptionType<TNew>.None())
        };
    }

    public static async Task<IFunctor<OptionType<TNew>>> MapAsync<T, TNew>(
        this Task<IFunctor<OptionType<T>>> functorTask,
        Func<T, TNew> map
    )
        where T : notnull
        where TNew : notnull
    {
        return (await functorTask).Type switch
        {
            OptionType<T>.Some some => new Functor<OptionType<TNew>>(map(some)),
            _ => new Functor<OptionType<TNew>>(new OptionType<TNew>.None())
        };
    }

    public static async Task<IFunctor<OptionType<TNew>>> MapAsync<T, TNew>(
        this Task<IFunctor<OptionType<T>>> functorTask,
        Func<T, Task<TNew>> map
    )
        where T : notnull
        where TNew : notnull
    {
        return (await functorTask).Type switch
        {
            OptionType<T>.Some some => new Functor<OptionType<TNew>>(await map(some)),
            _ => new Functor<OptionType<TNew>>(new OptionType<TNew>.None())
        };
    }
}
