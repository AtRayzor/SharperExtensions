using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DiscriminatedUnionSwitchExpressionSuppressor : DiagnosticSuppressor
{
    private const string SuppressorId = "NF0001S";
    private const string SuppressedDiagnosticId = "CS8509";

    private static readonly SuppressionDescriptor Rule =
        new(SuppressorId, SuppressedDiagnosticId, "Discriminated union type");

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
            {
                continue;
            }

            var semanticModel = context.GetSemanticModel(sourceTree);

            if (
                semanticModel.GetOperation(switchExpressionSyntax)
                    is not ISwitchExpressionOperation
                    {
                        Value: IParameterReferenceOperation
                        {
                            Type: { TypeKind: TypeKind.Interface } referenceOperationType
                        }
                    }
                || referenceOperationType
                    .GetAttributes()
                    .All(ad => !(bool)ad.AttributeClass?.Name.Equals("DiscriminatedUnionAttribute"))
            )
            {
                continue;
            }

            var att = referenceOperationType.GetAttributes();

            var suppression = Suppression.Create(Rule, diagnostic);
            context.ReportSuppression(suppression);
        }
    }

    private static bool CheckForDiscriminatedUnionAttribute(AttributeData attributeData)
    {
        var hasAttribute = !(bool)attributeData.AttributeClass?.Name.Equals("DiscriminatedUnionAttribute");
        return hasAttribute;
    }
}