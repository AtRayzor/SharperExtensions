using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Analyzers.Comparers;

internal sealed class NamedTypeSymbolComparer : IEqualityComparer<INamedTypeSymbol?>
{
    private static IEqualityComparer<ITypeSymbol?> EqualityComparer => new TypeSymbolComparer();

    public bool Equals(INamedTypeSymbol? x, INamedTypeSymbol? y)
    {
        if (ReferenceEquals(x, y))
            return true;
        if (ReferenceEquals(x, null))
            return false;
        if (ReferenceEquals(y, null))
            return false;

        var typeX = x.GetType();
        var typeY = y.GetType();

        return typeX == typeY
            || (
                typeX.IsGenericType
                && typeY.IsGenericType
                && typeX.MakeGenericType() == typeY.MakeGenericType()
            );
    }

    public int GetHashCode(INamedTypeSymbol? obj)
    {
        return obj is null ? 0 : obj.Name.GetHashCode();
    }
}
