namespace NetFunctional.Types.Traits;

public interface ITrait<out T>
    where T : IKind
{
    T Type { get; }
}

public interface IConstructableTrait<out T> : ITrait<T>
    where T : IKind;
