using System.Diagnostics.CodeAnalysis;
using NetFunctional.Types.Traits;

namespace NetFunctional.Types.OptionKind;

public static class OptionTraitExtension
{
    public static bool IsSome<T>(this ITrait<OptionType<T>> optionTrait)
        where T : notnull
    {
        return optionTrait.Type is OptionType<T>.Some;
    }

    public static bool IsNone<T>(this ITrait<OptionType<T>> optionTrait)
        where T : notnull
    {
        return optionTrait.Type is OptionType<T>.None;
    }

    public static bool TryGetValue<T>(
        this ITrait<OptionType<T>> optionTrait,
        [NotNullWhen(true)] out T? value
    )
        where T : notnull
    {
        value = optionTrait.Type is OptionType<T>.Some some ? some.Value : default;

        return value is not null;
    }

    public static IOption<T> AsOption<T>(this IConstructableTrait<OptionType<T>> optionTrait)
        where T : notnull
    {
        return optionTrait.AsConstructableTrait<OptionType<T>, IOption<T>, Option<T>>();
    }

    public static IOptionWithSideEffects<T> AsOptionWithSideEffects<T>(
        this IConstructableTrait<OptionType<T>> optionTrait
    )
        where T : notnull
    {
        return optionTrait.AsConstructableTrait<
            OptionType<T>,
            IOptionWithSideEffects<T>,
            OptionWithSideEffects<T>
        >();
    }

    public static IFunctor<OptionType<T>> AsFunctor<T>(
        this IConstructableTrait<OptionType<T>> optionTrait
    )
        where T : notnull
    {
        return optionTrait.AsConstructableTrait<
            OptionType<T>,
            IFunctor<OptionType<T>>,
            Functor<OptionType<T>>
        >();
    }

    public static IMonad<OptionType<T>> AsMonad<T>(
        this IConstructableTrait<OptionType<T>> optionTrait
    )
        where T : notnull
    {
        return optionTrait.AsConstructableTrait<
            OptionType<T>,
            IMonad<OptionType<T>>,
            Monad<OptionType<T>>
        >();
    }

    public static IApplicative<OptionType<T>> AsApplicative<T>(
        this IConstructableTrait<OptionType<T>> optionTrait
    )
        where T : notnull
    {
        return optionTrait.AsConstructableTrait<
            OptionType<T>,
            IApplicative<OptionType<T>>,
            Applicative<OptionType<T>>
        >();
    }
}
