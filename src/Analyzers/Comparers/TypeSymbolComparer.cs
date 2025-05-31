using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace SharperExtensions.Analyzers.Comparers;

internal class TypeSymbolComparer : IEqualityComparer<ITypeSymbol?>
{
    public bool Equals(ITypeSymbol? x, ITypeSymbol? y)
    {
        var symbolEqualityComparer = SymbolEqualityComparer.Default;

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

    public int GetHashCode(ITypeSymbol? obj)
    {
        return obj is null ? 0 : 1;
    }
}
