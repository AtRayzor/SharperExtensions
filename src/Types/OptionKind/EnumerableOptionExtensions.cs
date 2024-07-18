using NetFunctional.Types.Traits;

namespace NetFunctional.Types.OptionKind;

public static class EnumerableOptionExtensions
{
    public static IEnumerable<T> SelectIfSome<T>(
        this IEnumerable<ITrait<OptionType<T>>> optionTraits
    )
        where T : notnull
    {
        return optionTraits
            .Where(t => t.IsSome())
            .Select(ot => ((OptionType<T>.Some)ot.Type).Value);
    }

    private static TReturn Extract<T, TTrait, TReturn, TStruct>(
        this IEnumerable<TTrait> optionTraits
    )
        where T : notnull
        where TTrait : ITrait<OptionType<T>>
        where TReturn : ITrait<OptionType<IEnumerable<T>>>
        where TStruct : struct, TReturn
    {
        var enumerable = optionTraits as TTrait[] ?? optionTraits.ToArray();

        return enumerable.All(ot => ot.IsSome())
            ? TraitFactory<OptionType<IEnumerable<T>>, TReturn>.Construct<TStruct>(
                new OptionType<IEnumerable<T>>.Some(
                    enumerable.Select(ot => ((OptionType<T>.Some)ot.Type).Value)
                )
            )
            : TraitFactory<OptionType<IEnumerable<T>>, TReturn>.Construct<TStruct>(
                new OptionType<IEnumerable<T>>.None()
            );
    }

    public static IFunctor<OptionType<IEnumerable<T>>> Extract<T>(
        this IEnumerable<IFunctor<OptionType<T>>> optionFunctors
    )
        where T : notnull
    {
        return optionFunctors.Extract<
            T,
            IFunctor<OptionType<T>>,
            IFunctor<OptionType<IEnumerable<T>>>,
            Functor<OptionType<IEnumerable<T>>>
        >();
    }

    public static IMonad<OptionType<IEnumerable<T>>> Extract<T>(
        this IEnumerable<IMonad<OptionType<T>>> optionMonads
    )
        where T : notnull
    {
        return optionMonads.Extract<
            T,
            IMonad<OptionType<T>>,
            IMonad<OptionType<IEnumerable<T>>>,
            Monad<OptionType<IEnumerable<T>>>
        >();
    }

    public static IOption<IEnumerable<T>> Extract<T>(this IEnumerable<IOption<T>> optionFunctors)
        where T : notnull
    {
        return optionFunctors.Extract<
            T,
            IOption<T>,
            IOption<IEnumerable<T>>,
            Option<IEnumerable<T>>
        >();
    }

    public static IOptionWithSideEffects<IEnumerable<T>> Extract<T>(
        this IEnumerable<IOptionWithSideEffects<T>> optionFunctors
    )
        where T : notnull
    {
        return optionFunctors.Extract<
            T,
            IOption<T>,
            IOptionWithSideEffects<IEnumerable<T>>,
            OptionWithSideEffects<IEnumerable<T>>
        >();
    }
}
