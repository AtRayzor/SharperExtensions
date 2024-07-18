using NetFunctional.Types.Traits;

namespace NetFunctional.Types.OptionKind;

public static class OptionApplicativeExtensions
{
    public static IApplicative<OptionType<TNew>> DoMap<T, TNew>(
        IApplicative<OptionType<T>> applicative,
        Func<T, TNew> mapping
    )
        where T : notnull
        where TNew : notnull
    {
        var functor = applicative.AsFunctor();
        return functor.Map(mapping).AsApplicative();
    }

    public static IApplicative<OptionType<TNew>> Apply<T, TNew>(
        this IApplicative<OptionType<T>> applicative,
        IApplicative<OptionType<Func<T, TNew>>> application
    )
        where T : notnull
        where TNew : notnull
    {
        return application.Type switch
        {
            OptionType<Func<T, TNew>>.Some someMapping => DoMap(applicative, someMapping.Value),
            _ => new Applicative<OptionType<TNew>>(new OptionType<TNew>.None())
        };
    }

    public static async Task<IApplicative<OptionType<TNew>>> DoMapAsync<T, TNew>(
        Task<IApplicative<OptionType<T>>> applicativeTask,
        Func<T, TNew> mapping
    )
        where T : notnull
        where TNew : notnull
    {
        var functor = (await applicativeTask).AsFunctor();
        return functor.Map(mapping).AsApplicative();
    }

    public static async Task<IApplicative<OptionType<TNew>>> ApplyAsync<T, TNew>(
        this Task<IApplicative<OptionType<T>>> applicativeTask,
        Task<IApplicative<OptionType<Func<T, TNew>>>> application
    )
        where T : notnull
        where TNew : notnull
    {
        return (await application).Type switch
        {
            OptionType<Func<T, TNew>>.Some someMapping
                => await DoMapAsync(applicativeTask, someMapping.Value),
            _ => new Applicative<OptionType<TNew>>(new OptionType<TNew>.None())
        };
    }
}
