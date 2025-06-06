using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace SharperExtensions.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UnionTypeSwitchExpressionSuppressor : DiagnosticSuppressor
{
    private const string SuppressorId = "SE0001S";
    internal const string SuppressedDiagnosticId = "CS8509";

    private static readonly SuppressionDescriptor Rule = new(
        SuppressorId,
        SuppressedDiagnosticId,
        "Discriminated union type"
    );

    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions =>
        ImmutableArray.Create(Rule);

    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
        foreach (var diagnostic in context.ReportedDiagnostics)
        {
            var location = diagnostic.Location;

            if (
                diagnostic.Location.SourceTree is not { } sourceTree
                || !sourceTree.TryGetRoot(out var rootNode)
                || rootNode.FindNode(location.SourceSpan)
                    is not SwitchExpressionSyntax switchExpressionSyntax
            )
                continue;

            var semanticModel = context.GetSemanticModel(sourceTree);

            if (
                semanticModel.GetOperation(switchExpressionSyntax)
                    is not ISwitchExpressionOperation
                    {
                        Value.Type: { TypeKind: TypeKind.Class } referenceOperationType
                    }
                || referenceOperationType
                    .GetAttributes()
                    .All(ad => ad is not { AttributeClass.Name: "ClosedAttribute" })
            )
            {
                continue;
            }

            var suppression = Suppression.Create(Rule, diagnostic);
            context.ReportSuppression(suppression);
        }
    }
}