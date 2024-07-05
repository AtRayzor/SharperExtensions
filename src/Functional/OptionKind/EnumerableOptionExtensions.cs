using Monads.Traits;

namespace Monads.OptionKind;

public static class EnumerableOptionExtensions
{
    public static IEnumerable<T> SelectIfSome<T>(this IEnumerable<ITrait<OptionType<T>>> optionTraits) where T : notnull
        => optionTraits.Where(t => t.IsSome())
            .Select(ot => ((OptionType<T>.Some)ot.Type).Value);

    private static TReturn Extract<T, TConstructable, TReturn>(
        this IEnumerable<TConstructable> optionTraits)
        where T : notnull
        where TConstructable : IConstructableTrait<OptionType<T>>
        where TReturn : IConstructableTrait<OptionType<IEnumerable<T>>>
    {
        var enumerable = optionTraits as TConstructable[] ?? optionTraits.ToArray();

        return enumerable.All(ot => ot.IsSome())
            ? (TReturn)TReturn.Construct(new OptionType<IEnumerable<T>>.Some(
                enumerable.Select(ot => ((OptionType<T>.Some)ot.Type).Value)))
            : (TReturn)TReturn.Construct(new OptionType<IEnumerable<T>>.None());
    }

    public static IFunctor<OptionType<IEnumerable<T>>> Extract<T>(
        this IEnumerable<IFunctor<OptionType<T>>> optionFunctors)
        where T : notnull => optionFunctors.Extract<T, IFunctor<OptionType<T>>, IFunctor<OptionType<IEnumerable<T>>>>();

    public static IMonad<OptionType<IEnumerable<T>>> Extract<T>(
        this IEnumerable<IMonad<OptionType<T>>> optionMonads)
        where T : notnull => optionMonads.Extract<T, IMonad<OptionType<T>>, IMonad<OptionType<IEnumerable<T>>>>();

    public static IOption<IEnumerable<T>> Extract<T>(
        this IEnumerable<IOption<T>> optionFunctors)
        where T : notnull => optionFunctors.Extract<T, IOption<T>, IOption<IEnumerable<T>>>();
    
    public static IOptionWithSideEffects<IEnumerable<T>> Extract<T>(
        this IEnumerable<IOptionWithSideEffects<T>> optionFunctors)
        where T : notnull => optionFunctors.Extract<T, IOption<T>, IOptionWithSideEffects<IEnumerable<T>>>();
}