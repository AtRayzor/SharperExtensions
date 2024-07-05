using Monads.Traits;

namespace Monads.OptionKind;

public interface IOption<T> : IFunctor<OptionType<T>>, IMonad<OptionType<T>>
    where T : notnull
{
    static IConstructableTrait<OptionType<T>> IConstructableTrait<OptionType<T>>.Construct(OptionType<T> type) =>
        Create(type);

    new static IOption<T> Construct(OptionType<T> type) => new Option<T>(type);
}

public interface IOptionWithSideEffects<T> : IOption<T>, ISideEffects<OptionType<T>>
    where T : notnull
{
    static IConstructableTrait<OptionType<T>> IConstructableTrait<OptionType<T>>.Construct(OptionType<T> type) =>
        Construct(type);

    new static IOptionWithSideEffects<T> Construct(OptionType<T> type) => new OptionWithSideEffects<T>(type);
}

public static class Option
{
    public static IOption<T> Create<T>(T? value) where T : notnull => OptionTraitExtension.Create<T, IOption<T>>(value);
    public static IOption<T> Some<T>(T value) where T : notnull => OptionTraitExtension.Some<T, IOption<T>>(value);
    public static IOption<T> None<T>() where T : notnull => OptionTraitExtension.None<T, IOption<T>>();
}

public static class OptionWithSideEffects
{
    public static IOptionWithSideEffects<T> Create<T>(T? value) where T : notnull =>
        OptionTraitExtension.Create<T, IOptionWithSideEffects<T>>(value);

    public static IOptionWithSideEffects<T> Some<T>(T value) where T : notnull =>
        OptionTraitExtension.Some<T, IOptionWithSideEffects<T>>(value);

    public static IOptionWithSideEffects<T> None<T>() where T : notnull =>
        OptionTraitExtension.None<T, IOptionWithSideEffects<T>>();
}

internal readonly record struct Option<T>(OptionType<T> Type) : IOption<T> where T : notnull;

internal readonly record struct OptionWithSideEffects<T>(OptionType<T> Type)
    : IOptionWithSideEffects<T> where T : notnull;