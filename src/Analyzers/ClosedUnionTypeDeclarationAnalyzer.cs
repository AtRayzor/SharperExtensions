using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace SharperExtensions.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ClosedUnionTypeDeclarationAnalyzer : DiagnosticAnalyzer
{
    internal const string DiagnosticId = "NF0002";
    private const string Category = "Naming";

    private static readonly LocalizableString Title = new LocalizableResourceString(
        nameof(Resources.NF0002Title),
        Resources.ResourceManager,
        typeof(Resources)
    );

    private static readonly LocalizableString MessageFormat =
        new LocalizableResourceString(
            nameof(Resources.NF0002MessageFormat),
            Resources.ResourceManager,
            typeof(Resources)
        );

    private static readonly LocalizableString Description = new LocalizableResourceString(
        nameof(Resources.NF0002Description),
        Resources.ResourceManager,
        typeof(Resources)
    );

    internal static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        Title,
        MessageFormat,
        Category,
        DiagnosticSeverity.Error,
        true,
        Description
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(
            AnalyzeClosedUnionTypeDeclaration,
            SyntaxKind.ClassDeclaration,
            SyntaxKind.RecordDeclaration
        );
    }

    private void AnalyzeClosedUnionTypeDeclaration(SyntaxNodeAnalysisContext context)
    {
        if (
            context.Node is not TypeDeclarationSyntax typeDeclarationSyntax
            || context.SemanticModel.GetDeclaredSymbol(typeDeclarationSyntax) is not
                { } typeSymbol
            || typeSymbol
                .GetAttributes()
                .All(att => att is not { AttributeClass.Name: "ClosedAttribute" })
            || typeDeclarationSyntax.Modifiers.Any(mod =>
                mod.IsKind(SyntaxKind.AbstractKeyword)
            )
        )
            return;

        var typeModifier = typeDeclarationSyntax is RecordDeclarationSyntax
            ? "record"
            : "class";

        var diagnostic = Diagnostic.Create(
            Rule,
            typeDeclarationSyntax.GetLocation(),
            typeModifier,
            typeSymbol.ToDisplayString()
        );
        context.ReportDiagnostic(diagnostic);
    }
}