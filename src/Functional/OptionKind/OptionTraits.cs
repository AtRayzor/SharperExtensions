using Monads.Traits;

namespace Monads.OptionKind;

public interface IOptionFunctor<T> : IFunctor<OptionType<T>> where T : notnull;
public interface IOptionMonad<T>: IMonad<OptionType<T>> where T : notnull;
public interface IOptionApplicative<T> : IApplicative<OptionType<T>> where T : notnull;