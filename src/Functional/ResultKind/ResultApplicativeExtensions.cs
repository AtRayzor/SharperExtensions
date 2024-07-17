using Monads.ResultMonad;
using Monads.Traits;

namespace Monads.ResultKind;

#pragma warning disable CS8509
// The switch expression does not handle all possible values of its input type (it is not exhaustive).

public static class ResultApplicativeExtensions
{
    public static IApplicative<ResultType<TNew, TE>> DoMap<T, TE, TNew>(
        IApplicative<ResultType<T, TE>> applicative,
        Func<T, TNew> mapping
    ) where TNew : notnull where TE : notnull where T : notnull
    {
        var functor = applicative.AsFunctor();

        return functor.MapOk(mapping).AsApplicative();
    }

    public static IApplicative<ResultType<TNew, TE>> Apply<T, TE, TNew>(
        this IApplicative<ResultType<T, TE>> applicative,
        IApplicative<ResultType<Func<T, TNew>, TE>> application
    )
        where T : notnull where TE : notnull where TNew : notnull
        => application.Type switch
        {
            ResultType<Func<T, TNew>, TE>.Ok mappingResult => DoMap(applicative, mappingResult.Value),
            ResultType<Func<T, TNew>, TE>.Error errorResult => new Applicative<ResultType<TNew, TE>>(
                new ResultType<TNew, TE>.Error(errorResult.Err))
        };

    public static async Task<IApplicative<ResultType<TNew, TE>>> DoMapAsync<T, TE, TNew>(
        Task<IApplicative<ResultType<T, TE>>> applicativeTask,
        Func<T, TNew> mapping
    ) where TNew : notnull where TE : notnull where T : notnull
    {
        var functor = (await applicativeTask).AsFunctor();
        return functor.MapOk(mapping).AsApplicative();
    }

    public static async Task<IApplicative<ResultType<TNew, TE>>> ApplyAsync<T, TE, TNew>(
        this Task<IApplicative<ResultType<T, TE>>> applicativeTask,
        Task<IApplicative<ResultType<Func<T, TNew>, TE>>> applicationTask
    )
        where T : notnull where TE : notnull where TNew : notnull
        => (await applicationTask).Type switch
        {
            ResultType<Func<T, TNew>, TE>.Ok mappingResult => await DoMapAsync(applicativeTask, mappingResult.Value),
            ResultType<Func<T, TNew>, TE>.Error errorResult => new Applicative<ResultType<TNew, TE>>(
                new ResultType<TNew, TE>.Error(errorResult.Err))
        };
}