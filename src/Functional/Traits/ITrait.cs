namespace Monads.Traits;

public interface ITrait<out T> where T : IKind
{
    T Type { get;  }
}
