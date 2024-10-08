using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace DotNetCoreFunctional.UnionTypes.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UnionTypeSwitchAnalyzer : DiagnosticAnalyzer
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

    internal static readonly DiagnosticDescriptor Rule =
        new(
            DiagnosticId,
            Title,
            MessageFormat,
            "UnionTypes",
            DiagnosticSeverity.Error,
            true,
            Description
        );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        if (!Debugger.IsAttached)
            Debugger.Launch();

        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterOperationAction(
            AnalyzeSwitchExpression,
            OperationKind.SwitchExpression,
            OperationKind.Switch
        );
    }

    private void AnalyzeSwitchExpression(OperationAnalysisContext context)
    {
        if (
            !TryToParseSwitch(
                context.Operation,
                out var switchCaseOperations,
                out var referenceOperationType
            )
        )
        {
            return;
        }

        var childTypes = GetUnionCaseTypes(referenceOperationType!);

        var switchCaseTypes = switchCaseOperations
            .Select(
                op =>
                    op switch
                    {
                        IRecursivePatternOperation {MatchedType: INamedTypeSymbol matchedType} 
                            => matchedType,
                        ITypePatternOperation { MatchedType: INamedTypeSymbol matchedType }
                            => matchedType,
                        IDeclarationPatternOperation { MatchedType: INamedTypeSymbol matchedType }
                            => matchedType,
                        _ => null
                    }
            )
            .OfType<INamedTypeSymbol>()
            .Select(GetComparableNamedTypeSymbol)
            .ToArray();

        if (
            switchCaseOperations.Length > 0
            && (
                switchCaseOperations.Last()
                    is IDiscardPatternOperation
                        or IDefaultCaseClauseOperation
                || (
                    childTypes.Length == switchCaseTypes.Length
                    && childTypes.Intersect(switchCaseTypes, SymbolEqualityComparer.Default).Count()
                        == childTypes.Length
                )
            )
        )
            return;

        var diagnostic = Diagnostic.Create(
            Rule,
            context.Operation.Syntax.GetLocation(),
            referenceOperationType
        );

        context.ReportDiagnostic(diagnostic);
    }

    private bool TryToParseSwitch(
        IOperation operation,
        out ImmutableArray<IOperation> caseOperations,
        out INamedTypeSymbol? switchValueType
    )
    {
        switch (operation)
        {
            case ISwitchExpressionOperation switchExpressionOperation:
                caseOperations = GetSwitchArmOperations(switchExpressionOperation);
                return TryGetNamedSwitchValueType(
                    switchExpressionOperation.Value,
                    out switchValueType
                );
            case ISwitchOperation switchOperation:
                caseOperations = GetSwitchStatementCaseOperations(switchOperation);
                return TryGetNamedSwitchValueType(switchOperation.Value, out switchValueType);

            default:
                caseOperations = ImmutableArray<IOperation>.Empty;
                switchValueType = default;
                return false;
        }
    }

    private bool TryGetNamedSwitchValueType(
        IOperation valueParameter,
        out INamedTypeSymbol? switchValueType
    )
    {
        switchValueType = default;

        if (
            valueParameter
            is not { Type: INamedTypeSymbol { TypeKind: TypeKind.Class } referenceOperationType }
        )
        {
            return false;
        }

        if (
            referenceOperationType
                .GetAttributes()
                .All(a => a is not { AttributeClass.Name: "ClosedAttribute" })
        )
        {
            return false;
        }

        switchValueType = referenceOperationType;
        return true;
    }

    private ImmutableArray<IOperation> GetSwitchArmOperations(
        ISwitchExpressionOperation switchExpressionOperation
    )
    {
        return [
            ..switchExpressionOperation
                .Arms
                .Select(arm => arm.ChildOperations)
                .SelectMany(
                    ops =>
                        ops.Select(
                                o =>
                                    o switch
                                    { 
                                        IRecursivePatternOperation  rpo => rpo ,
                                        ITypePatternOperation tpo => (IOperation)tpo,
                                        IDeclarationPatternOperation dpo => dpo,
                                        IDiscardPatternOperation def => def,
                                        _ => null
                                    }
                            )
                            .Where(o => o is not null)
                )
                .Except([null])
                .Cast<IOperation>()
        ];
    }

    private ImmutableArray<IOperation> GetSwitchStatementCaseOperations(
        ISwitchOperation switchOperation
    )
    {
        return switchOperation
            .Cases
            .Select(sc => sc.Clauses)
            .SelectMany(
                clauses =>
                    clauses.Select(
                        cco =>
                            cco switch
                            {
                                IPatternCaseClauseOperation { Pattern: IRecursivePatternOperation rpo }
                                    => (IOperation)rpo,
                                IPatternCaseClauseOperation { Pattern: ITypePatternOperation tpo }
                                    => tpo,
                                IPatternCaseClauseOperation
                                {
                                    Pattern: IDeclarationPatternOperation dpo
                                }
                                    => dpo,
                                IDefaultCaseClauseOperation dpo => dpo,
                                _ => null
                            }
                    )
            )
            .Except([null])
            .Cast<IOperation>()
            .ToImmutableArray();
    }

    private INamedTypeSymbol[] GetUnionCaseTypes(INamedTypeSymbol unionTypeSymbol)
    {
        var namedTypeMembers = unionTypeSymbol.GetTypeMembers().ToImmutableArray();

        var typesToCheck =
            namedTypeMembers.Length > 0
                ? namedTypeMembers
                : unionTypeSymbol.ContainingNamespace.GetTypeMembers();

        var unionTypeChecker = ResolveUnionTypeChecker(unionTypeSymbol);

        return typesToCheck.Where(unionTypeChecker).Select(GetComparableNamedTypeSymbol).ToArray();
    }

    private static bool ValueTypeChecker(
        INamedTypeSymbol typeSymbol,
        INamedTypeSymbol caseTypeSymbol
    )
    {
        return false;
    }

    private static bool ReferenceTypeChecker(
        INamedTypeSymbol typeSymbol,
        INamedTypeSymbol caseTypeSymbol
    )
    {
        var comparableTypeSymbol = GetComparableNamedTypeSymbol(typeSymbol);

        return caseTypeSymbol.BaseType is { } baseType
            && comparableTypeSymbol.Equals(
                GetComparableNamedTypeSymbol(baseType),
                SymbolEqualityComparer.Default
            );
    }

    private static Func<INamedTypeSymbol, bool> ResolveUnionTypeChecker(
        INamedTypeSymbol referenceTypeSymbol
    )
    {
        return referenceTypeSymbol switch
        {
            { TypeKind: TypeKind.Class }
                => implementationTypeSymbol =>
                    ReferenceTypeChecker(referenceTypeSymbol, implementationTypeSymbol),
            { TypeKind: TypeKind.Struct }
                => castTypeSymbol => ValueTypeChecker(referenceTypeSymbol, castTypeSymbol)
        };
    }

    private static INamedTypeSymbol GetComparableNamedTypeSymbol(INamedTypeSymbol typeSymbol)
    {
        return !typeSymbol.IsGenericType ? typeSymbol : typeSymbol.ConstructUnboundGenericType();
    }
}
