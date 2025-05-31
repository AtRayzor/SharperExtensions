using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Microsoft.CodeAnalysis;
using SharperExtensions.Analyzers.Comparers;

namespace SharperExtensions.Analyzers.Extensions;

public static class NamedTypeSymbolExtensions
{
    [Pure]
    internal static IEnumerable<INamedTypeSymbol> IntersectNamedTypeSymbols<TComparer>(
        this IEnumerable<INamedTypeSymbol> first,
        IEnumerable<INamedTypeSymbol> second
    )
        where TComparer : IEqualityComparer<INamedTypeSymbol>, new()
    {
        return first.Intersect(second, new TComparer());
    }

    [Pure]
    internal static IEnumerable<INamedTypeSymbol> IntersectNamedTypeSymbols(
        this IEnumerable<INamedTypeSymbol> first,
        IEnumerable<INamedTypeSymbol> second
    )
    {
        return first.IntersectNamedTypeSymbols<NamedTypeSymbolComparer>(second);
    }
}
