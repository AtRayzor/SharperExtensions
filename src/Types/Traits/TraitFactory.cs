namespace NetFunctional.Types.Traits;

public abstract class TraitFactory<T, TTrait>
    where TTrait : ITrait<T>
    where T : IKind
{
    public static TTrait Construct<TStruct>(T type)
        where TStruct : struct, TTrait
    {
        return (TTrait)Activator.CreateInstance(typeof(TStruct), [type])!;
    }
}
