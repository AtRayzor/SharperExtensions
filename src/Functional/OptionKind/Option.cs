using Monads.Traits;

namespace Monads.OptionKind;

public interface IOption<T> : IFunctor<OptionType<T>>, IMonad<OptionType<T>>, IApplicative<OptionType<T>>
    where T : notnull;

public interface IOptionWithSideEffects<T> : IOption<T>, ISideEffects<OptionType<T>>
    where T : notnull;

public abstract class OptionFactory<T> : TraitFactory<OptionType<T>, IOption<T>> where T : notnull;

public static class Option
{
    public static IOption<T> Create<T>(T? value) where T : notnull => value is not null ? Some(value) : None<T>();
    public static IOption<T> Some<T>(T value) where T : notnull => OptionFactory<T>.Construct<Option<T>>(value);

    public static IOption<T> None<T>() where T : notnull =>
        OptionFactory<T>.Construct<Option<T>>(new OptionType<T>.None());
}

public abstract class OptionsWithSideEffectsFactory<T> : TraitFactory<OptionType<T>, IOptionWithSideEffects<T>>
    where T : notnull;

public static class OptionWithSideEffects
{
    public static IOptionWithSideEffects<T> Create<T>(T? value) where T : notnull =>
        value is not null ? Some(value) : None<T>();

    public static IOptionWithSideEffects<T> Some<T>(T value) where T : notnull =>
        OptionsWithSideEffectsFactory<T>.Construct<OptionWithSideEffects<T>>(value);

    public static IOptionWithSideEffects<T> None<T>() where T : notnull =>
        OptionsWithSideEffectsFactory<T>.Construct<OptionWithSideEffects<T>>(new OptionType<T>.None());
}

internal readonly record struct Option<T>(OptionType<T> Type) : IOption<T> where T : notnull;

internal readonly record struct OptionWithSideEffects<T>(OptionType<T> Type)
    : IOptionWithSideEffects<T> where T : notnull;