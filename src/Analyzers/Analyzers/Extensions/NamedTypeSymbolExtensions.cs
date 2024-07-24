using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Analyzers.Comparers;
using Microsoft.CodeAnalysis;

namespace Analyzers.Extensions;

public static class NamedTypeSymbolExtensions
{
    [Pure]
    internal static IEnumerable<INamedTypeSymbol> IntersectNamedTypeSymbols<TComparer>(
        this IEnumerable<INamedTypeSymbol> first,
        IEnumerable<INamedTypeSymbol> second
    )
        where TComparer : IEqualityComparer<INamedTypeSymbol>, new() =>
        first.Intersect(second, new TComparer());

    [Pure]
    internal static IEnumerable<INamedTypeSymbol> IntersectNamedTypeSymbols(
        this IEnumerable<INamedTypeSymbol> first,
        IEnumerable<INamedTypeSymbol> second
    ) => first.IntersectNamedTypeSymbols<NamedTypeSymbolComparer>(second);
}
