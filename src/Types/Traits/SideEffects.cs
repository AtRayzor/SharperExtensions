namespace NetFunctional.Types.Traits;

public interface IImplementsSideEffects : IKind;

public interface ISideEffects<out T> : ITrait<T>
    where T : IImplementsSideEffects { }

internal readonly record struct SideEffects<T>(T Type) : ISideEffects<T>
    where T : IImplementsSideEffects { }
