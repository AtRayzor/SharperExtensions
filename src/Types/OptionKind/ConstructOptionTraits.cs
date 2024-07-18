using NetFunctional.Types.OptionKind;

// ReSharper disable once CheckNamespace
namespace NetFunctional.Types.Traits;

public static partial class Functor
{
    public static class Option
    {
        public static IFunctor<OptionType<T>> Create<T>(T? value)
            where T : notnull
        {
            return value is not null ? Some(value) : None<T>();
        }

        public static IFunctor<OptionType<T>> Some<T>(T value)
            where T : notnull
        {
            return FunctorFactory<OptionType<T>>.Construct<Option<T>>(value);
        }

        public static IFunctor<OptionType<T>> None<T>()
            where T : notnull
        {
            return FunctorFactory<OptionType<T>>.Construct<Option<T>>(new OptionType<T>.None());
        }
    }
}

public static partial class Monad
{
    public static class Option
    {
        public static IMonad<OptionType<T>> Create<T>(T? value)
            where T : notnull
        {
            return value is not null ? Some(value) : None<T>();
        }

        public static IMonad<OptionType<T>> Some<T>(T value)
            where T : notnull
        {
            return MonadFactory<OptionType<T>>.Construct<Option<T>>(value);
        }

        public static IMonad<OptionType<T>> None<T>()
            where T : notnull
        {
            return MonadFactory<OptionType<T>>.Construct<Option<T>>(new OptionType<T>.None());
        }
    }
}
