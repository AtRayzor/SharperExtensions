using System;
using System.Collections.Immutable;
using System.Linq;
using Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DiscriminatedUnionSwitchStatementAnalyzer : DiagnosticAnalyzer
{
    internal const string DiagnosticId = "NF0004";
    private const string Category = "DiscriminatedUnion";

    private static readonly LocalizableString Title =
        new LocalizableResourceString(
            nameof(Resources.NF0004Title),
            Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString MessageFormat =
        new LocalizableResourceString(
            nameof(Resources.NF0004MessageFormat),
            Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString Description =
        new LocalizableResourceString(
            nameof(Resources.NF0004Description),
            Resources.ResourceManager, typeof(Resources));

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Warning,
        true,
        Description
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterOperationAction(AnalyzeSwitchStatementOperation, OperationKind.Switch);
    }

    public void AnalyzeSwitchStatementOperation(OperationAnalysisContext context)
    {
        if (context.Operation is not ISwitchOperation switchOperation
            || context.Operation.Syntax is not SwitchStatementSyntax switchStatementSyntax
            || switchOperation.Value is not
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
        var childTypes = referenceNamespace
            .GetTypeMembers()
            .Where(interfaceChecker)
            .ToArray();

        var switchCaseOperations = switchOperation
            .Cases
            .Select(switchCase => switchCase.ChildOperations)
            .SelectMany(
                ops =>
                    ops.Select(
                            o =>
                                o switch
                                {
                                    IPatternCaseClauseOperation pco => (IOperation)pco,
                                    IDefaultCaseClauseOperation dco => dco,
                                    _ => null
                                }
                        )
                        .Where(o => o is not null)
            )
            .ToArray();

        var switchCaseTypes = switchCaseOperations
            .Select(
                op =>
                    op switch
                    {
                        IPatternCaseClauseOperation { Pattern: IDeclarationPatternOperation dpo } => dpo.MatchedType,
                        IPatternCaseClauseOperation { Pattern: ITypePatternOperation tpo } => tpo.MatchedType,
                        _ => null
                    }
            )
            .Where(symbol => symbol is not null)
            .Cast<INamedTypeSymbol>()
            .ToArray();

        if (
            switchCaseOperations.Length > 0
            && (
                switchCaseOperations.Last() is IDefaultCaseClauseOperation
                || (
                    childTypes.Length == switchCaseTypes.Length
                    && childTypes.IntersectNamedTypeSymbols(switchCaseTypes).Count()
                    == childTypes.Length
                )
            )
        )
        {
            return;
        }

        var diagnostic = Diagnostic.Create(
            Rule,
            switchStatementSyntax.GetLocation(),
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