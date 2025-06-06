using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace SharperExtensions.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UnionParameterAnalyzer : DiagnosticAnalyzer
{
    internal static string InvalidParamTypeDiagnosticId =>
        InvalidParamTypeConfiguration.DiagnosticId;

    internal static DiagnosticDescriptor InvalidParamTypeRule =>
        InvalidParamTypeConfiguration.Rule;

    internal static string InvalidArgumentDiagnosticId =>
        InvalidArgumentConfiguration.DiagnosticId;

    internal static DiagnosticDescriptor InvalidArgumentRule =>
        InvalidArgumentConfiguration.Rule;

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeParameterSymbol, SymbolKind.Parameter);
        context.RegisterOperationAction(
            AnalyzeInvocationOperation,
            OperationKind.Invocation
        );
    }

    private static void AnalyzeParameterSymbol(SymbolAnalysisContext context)
    {
        if (
            context.Symbol is not IParameterSymbol { Type: { } type } parameterSymbol
            || parameterSymbol
                .GetAttributes()
                .All(a => a.AttributeClass is not { Name: "UnionAttribute" })
            || type
                is INamedTypeSymbol
                {
                    Name: "object" or "Object",
                    ContainingNamespace.Name: "System"
                }
            || parameterSymbol.Locations.SingleOrDefault() is not { } location
        )
        {
            return;
        }

        var diagnostic = Diagnostic.Create(
            InvalidParamTypeRule,
            location,
            type.MetadataName
        );
        context.ReportDiagnostic(diagnostic);
    }

    private static void AnalyzeInvocationOperation(OperationAnalysisContext context)
    {
        if (
            context.Operation
            is not IInvocationOperation
            {
                TargetMethod: var targetMethod, Arguments: var arguments
            }
        )
        {
            return;
        }

        if (
            targetMethod
                .Parameters
                .Select(p =>
                    (
                        Parameter: p,
                        Types: p
                            .GetAttributes()
                            .SingleOrDefault(a => a.AttributeClass is
                                { Name: "UnionAttribute" }
                            )
                            ?.ConstructorArguments
                            .Select(ExtractTypeFromConstant)
                            .OfType<ITypeSymbol>()
                            .ToImmutableArray()
                    )
                )
                .Where(ta => ta is not (_, null))
                .ToImmutableArray()
            is not { Length: > 0 } parameterSymbols
        )
        {
            return;
        }

        var unmatched = arguments
            .OfType<IArgumentOperation>()
            .Join(
                parameterSymbols,
                a => a.Parameter?.Ordinal,
                pp => pp.Parameter.Ordinal,
                (a, pp) => (a, pp)
            )
            .Where(j =>
                j
                        .a
                        .ChildOperations
                        .SingleOrDefault()
                        ?.ChildOperations
                        .SingleOrDefault()
                        ?.Type
                    is { } argType
                && j.pp.Types!.Value.All(t => !argType.Equals(
                        t,
                        SymbolEqualityComparer.Default
                    )
                )
            )
            .Select(j => (j.a, j.pp))
            .ToImmutableArray();

        foreach (var (arg, param) in unmatched)
        {
            var location = arg.Syntax.GetLocation();
            var value = arg.Value;
            var type = value.ChildOperations.SingleOrDefault()?.Type?.Name;
            var diagnostic = Diagnostic.Create(InvalidArgumentRule, location, type);

            context.ReportDiagnostic(diagnostic);
        }
    }

    private static ITypeSymbol? ExtractTypeFromConstant(TypedConstant typedConstant)
    {
        if (typedConstant.Type is not { } type)
        {
            return null;
        }

        switch (type)
        {
            case
            {
                OriginalDefinition: IArrayTypeSymbol
                {
                    ElementType: { ContainingNamespace.Name: "System", Name: "Type" }
                }
            }:
            {
                if (typedConstant.Values.IsEmpty)
                {
                    return type.IsType ? null : type;
                }

                foreach (var value in typedConstant.Values)
                {
                    if (ExtractTypeFromConstant(value) is { } extractedType)
                    {
                        return extractedType;
                    }
                }

                return null;
            }

            case { ContainingNamespace.Name: "System", Name: "Type" }:
            {
                if (typedConstant.Value is not ITypeSymbol extractedType)
                {
                    return type is { ContainingNamespace.Name: "System", Name: "Type" }
                        ? null
                        : type;
                }

                return extractedType;
            }
            default:
            {
                return type;
            }
        }
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [InvalidParamTypeRule, InvalidArgumentRule];
}

file static class InvalidParamTypeConfiguration
{
    public const string DiagnosticId = "NF0004";
    private const string Category = "Unions";

    private static readonly LocalizableString Title = new LocalizableResourceString(
        nameof(Resources.SE0004Title),
        Resources.ResourceManager,
        typeof(Resources)
    );

    private static readonly LocalizableString Description = new LocalizableResourceString(
        nameof(Resources.SE0004Description),
        Resources.ResourceManager,
        typeof(Resources)
    );

    private static readonly LocalizableString MessageFormat =
        new LocalizableResourceString(
            nameof(Resources.SE0004MessageFormat),
            Resources.ResourceManager,
            typeof(Resources)
        );

    public static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Error,
        true,
        Description
    );
}

file static class InvalidArgumentConfiguration
{
    public const string DiagnosticId = "NF0005";
    private const string Category = "Unions";

    private static readonly LocalizableString Title = new LocalizableResourceString(
        nameof(Resources.SE0005Title),
        Resources.ResourceManager,
        typeof(Resources)
    );

    private static readonly LocalizableString Description = new LocalizableResourceString(
        nameof(Resources.SE0005Description),
        Resources.ResourceManager,
        typeof(Resources)
    );

    private static readonly LocalizableString MessageFormat =
        new LocalizableResourceString(
            nameof(Resources.SE0005MessageFormat),
            Resources.ResourceManager,
            typeof(Resources)
        );

    public static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Error,
        true,
        Description
    );
}