using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DiscriminatedUnionSwitchExpressionAnalyzer : DiagnosticAnalyzer
{
    private const string DiagnosticId = "NF0001";

    private static readonly LocalizableString Title = new LocalizableResourceString(
        nameof(Resources.NF0001Title),
        Resources.ResourceManager,
        typeof(Resources)
    );

    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
        nameof(Resources.NF0001MessageFormat),
        Resources.ResourceManager,
        typeof(Resources)
    );

    private static readonly LocalizableString Description = new LocalizableResourceString(
        nameof(Resources.NF0001Description),
        Resources.ResourceManager,
        typeof(Resources)
    );

    private static readonly DiagnosticDescriptor Rule =
        new(
            DiagnosticId,
            Title,
            MessageFormat,
            "Usage",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description
        );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        if (!Debugger.IsAttached)
        {
            Debugger.Launch();
        }

        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterOperationAction(AnalyzeSwitchExpression, OperationKind.SwitchExpression);
    }

    private void AnalyzeSwitchExpression(OperationAnalysisContext context)
    {
        if (
            context.Operation is not ISwitchExpressionOperation switchExpressionOperation
            || context.Operation.Syntax is not SwitchExpressionSyntax switchExpressionSyntax
            || switchExpressionOperation.Value
                is not IParameterReferenceOperation
                {
                    Type: INamedTypeSymbol { TypeKind: TypeKind.Interface } referenceOperationType
                }
            || referenceOperationType
                .GetAttributes()
                .All(ad => !(bool)ad.AttributeClass?.Name.Equals("DiscriminatedUnionAttribute"))
        )
        {
            return;
        }

        var referenceNamespace = referenceOperationType.ContainingNamespace;
        var interfaceChecker = CreateInterfaceChecker(referenceOperationType);
        var childTypes = referenceNamespace.GetTypeMembers().Where(interfaceChecker).ToArray();

        var switchArmOperations = switchExpressionOperation
            .Arms
            .Select(arm => arm.ChildOperations)
            .SelectMany(
                ops =>
                    ops.Select(
                            o =>
                                o switch
                                {
                                    ITypePatternOperation tpo => (IOperation)tpo,
                                    IDeclarationPatternOperation dpo => dpo,
                                    IDiscardPatternOperation def => def,
                                    _ => null
                                }
                        )
                        .Where(o => o is not null)
            )
            .ToArray();

        var switchArmTypes = switchArmOperations
            .Select(
                op =>
                    op switch
                    {
                        ITypePatternOperation tpo => tpo.MatchedType,
                        IDeclarationPatternOperation dpo => dpo.MatchedType,
                        _ => null
                    }
            )
            .Where(symbol => symbol is not null)
            .Cast<INamedTypeSymbol>()
            .ToArray();

        if (
            switchArmOperations.Length > 0
            && (
                switchArmOperations.Last() is IDiscardPatternOperation
                || (
                    childTypes.Length == switchArmTypes.Length
                    && childTypes.IntersectNamedTypeSymbols(switchArmTypes).Count()
                        == childTypes.Length
                )
            )
        )
        {
            return;
        }

        var diagnostic = Diagnostic.Create(
            Rule,
            switchExpressionSyntax.GetLocation(),
            referenceOperationType
        );

        context.ReportDiagnostic(diagnostic);
    }

    private static Func<INamedTypeSymbol, bool> CreateInterfaceChecker(
        INamedTypeSymbol referenceTypeSymbol
    )
    {
        var unboundReferenceTypeSymbol = referenceTypeSymbol.IsGenericType
            ? referenceTypeSymbol.ConstructUnboundGenericType()
            : null;

        return implementationTypeSymbol =>
        {
            var interfaces = implementationTypeSymbol.Interfaces;
            var contains = interfaces.Any(
                i =>
                    i.Equals(referenceTypeSymbol, SymbolEqualityComparer.Default)
                    || (
                        i.IsGenericType
                        && i.ConstructUnboundGenericType()
                            .Equals(unboundReferenceTypeSymbol, SymbolEqualityComparer.Default)
                    )
            );
            return contains;
        };
    }
}
