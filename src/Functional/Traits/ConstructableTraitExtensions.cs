namespace Monads.Traits;

public static class ConstructableTraitExtensions
{
    internal static TConstructable AsConstructableTrait<T, TConstructable, TStruct>(
        this IConstructableTrait<T> constructableTrait) where T : IKind
        where TConstructable : ITrait<T>
        where TStruct : struct, TConstructable
    {
        var optionTraitType = constructableTrait.GetType();

        if (optionTraitType.IsAssignableTo(typeof(TConstructable)))
        {
            return (TConstructable)constructableTrait;
        }

        return TraitFactory<T, TConstructable>.Construct<TStruct>(constructableTrait.Type);
    }
}