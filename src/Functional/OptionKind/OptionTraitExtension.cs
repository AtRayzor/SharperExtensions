using System.Diagnostics.CodeAnalysis;
using Monads.Traits;

namespace Monads.OptionKind;

public static class OptionTraitExtension
{
    public static bool IsSome<T>(this ITrait<OptionType<T>> optionTrait) where T : notnull
        => optionTrait.Type is OptionType<T>.Some;

    public static bool IsNone<T>(this ITrait<OptionType<T>> optionTrait) where T : notnull
        => optionTrait.Type is OptionType<T>.None;

    public static bool TryGetValue<T>(this ITrait<OptionType<T>> optionTrait,
        [NotNullWhen(returnValue: true)] out T? value)
        where T : notnull
    {
        value = optionTrait.Type is OptionType<T>.Some some ? some.Value : default;

        return value is not null;
    }

    private static TConstructable AsConstableTrait<T, TConstructable, TStruct>(this ITrait<OptionType<T>> optionTrait)
        where T : notnull where TConstructable : ITrait<OptionType<T>> where TStruct : struct, TConstructable
    {
        var optionTraitType = optionTrait.GetType();

        if (optionTraitType.IsAssignableTo(typeof(TConstructable)))
        {
            return (TConstructable)optionTrait;
        }

        return TraitFactory<OptionType<T>, TConstructable>.Construct<TStruct>(optionTrait.Type);
    }

    public static IOption<T> AsOption<T>(this ITrait<OptionType<T>> optionTrait) where T : notnull
        => optionTrait.AsConstableTrait<T, IOption<T>, Option<T>>();

    public static IOptionWithSideEffects<T> AsOptionWithSideEffects<T>(this ITrait<OptionType<T>> optionTrait)
        where T : notnull
        => optionTrait.AsConstableTrait<T, IOptionWithSideEffects<T>, OptionWithSideEffects<T>>();

    public static IFunctor<OptionType<T>> AsFunctor<T>(this ITrait<OptionType<T>> optionTrait) where T : notnull
        => optionTrait.AsConstableTrait<T, IFunctor<OptionType<T>>, Functor<OptionType<T>>>();

    public static IMonad<OptionType<T>> AsMonad<T>(this ITrait<OptionType<T>> optionTrait) where T : notnull =>
        optionTrait.AsConstableTrait<T, IMonad<OptionType<T>>, Monad<OptionType<T>>>();
}