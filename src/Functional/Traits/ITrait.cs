namespace Monads.Traits;

public interface ITrait<out T> where T : IKind
{
    T Type { get;  }
}

public interface IConstructableTrait<T> : ITrait<T> where T : IKind
{
    static abstract IConstructableTrait<T> Construct(T type);
}