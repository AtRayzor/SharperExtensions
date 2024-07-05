using System.Diagnostics.CodeAnalysis;
using Monads.Traits;

namespace Monads.OptionKind;

public static class OptionTraitExtension
{
    public static TOptionTrait Create<T, TOptionTrait>(T? value)
        where TOptionTrait : IConstructableTrait<OptionType<T>> where T : notnull
        => value is not null
            ? Some<T, TOptionTrait>(value)
            : None<T, TOptionTrait>();

    public static TOptionTrait Some<T, TOptionTrait>(T value)
        where TOptionTrait : IConstructableTrait<OptionType<T>> where T : notnull
        => (TOptionTrait)TOptionTrait.Construct(value);

    public static TOptionTrait None<T, TOptionTrait>()
        where TOptionTrait : IConstructableTrait<OptionType<T>> where T : notnull
        => (TOptionTrait)TOptionTrait.Construct(new OptionType<T>.None());

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

    private static TConstructable AsConstableTrait<T, TConstructable>(this ITrait<OptionType<T>> optionTrait)
        where T : notnull where TConstructable : IConstructableTrait<OptionType<T>>
    {
        var optionTraitType = optionTrait.GetType();

        if (optionTraitType.IsAssignableTo(typeof(TConstructable)))
        {
            return (TConstructable)optionTrait;
        }

        return (TConstructable)TConstructable.Construct(optionTrait.Type);
    }

    public static IOption<T> AsOption<T>(this ITrait<OptionType<T>> optionTrait) where T : notnull
        => optionTrait.AsConstableTrait<T, Option<T>>();

    public static IFunctor<OptionType<T>> AsFunctor<T>(this ITrait<OptionType<T>> optionTrait) where T : notnull
        => optionTrait.AsConstableTrait<T, IFunctor<OptionType<T>>>();

    public static IMonad<OptionType<T>> AsMonad<T>(this ITrait<OptionType<T>> optionTrait) where T : notnull =>
        optionTrait.AsConstableTrait<T, IMonad<OptionType<T>>>();
}