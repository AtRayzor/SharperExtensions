using Monads.ResultMonad;
using Monads.Traits;

namespace Monads.ResultKind;

public interface IResultFunctor<T, TE> : IFunctor<ResultType<T, TE>> where T : notnull where TE : notnull;
public interface IResultMonad<T, TE> : IMonad<ResultType<T, TE>> where T : notnull where TE : notnull;
public interface IResultApplicative<T, TE> : IApplicative<ResultType<T, TE>> where T : notnull where TE : notnull;