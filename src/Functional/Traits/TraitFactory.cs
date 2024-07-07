namespace Monads.Traits;

public abstract class TraitFactory<T, TTrait> where TTrait : ITrait<T> where T : IKind
{
    public static TTrait Construct<TStruct>(T type)  where TStruct : struct, TTrait =>
        (TTrait)Activator.CreateInstance(typeof(TStruct), [type])!;
}