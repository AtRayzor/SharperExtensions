using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DiscriminatedUnionCaseDeclarationAnalyzer : DiagnosticAnalyzer
{
    internal const string DiagnosticId = "NF0002";
    private const string Category = "D";

    private static readonly LocalizableString Title = new LocalizableResourceString(
        nameof(Resources.NF0002Title),
        Resources.ResourceManager,
        typeof(Resources)
    );

    private static readonly LocalizableString Description = new LocalizableResourceString(
        nameof(Resources.NF0002Description),
        Resources.ResourceManager,
        typeof(Resources)
    );

    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
        nameof(Resources.NF0002MessageFormat),
        Resources.ResourceManager,
        typeof(Resources)
    );

    private static readonly DiagnosticDescriptor Rule =
        new(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            Description
        );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(
            AnalyzeRecordDeclarationSyntax,
            SyntaxKind.RecordDeclaration,
            SyntaxKind.RecordStructDeclaration
        );

        context.RegisterSyntaxNodeAction(
            AnalyzeClassDeclarationSyntax,
            SyntaxKind.ClassDeclaration
        );

        context.RegisterSyntaxNodeAction(
            AnalyzeStructDeclarationSyntax,
            SyntaxKind.StructDeclaration
        );
    }

    private void AnalyzeRecordDeclarationSyntax(SyntaxNodeAnalysisContext context)
    {
        if (
            context.Node is not RecordDeclarationSyntax recordDeclarationSyntax
            || context.SemanticModel.GetDeclaredSymbol(recordDeclarationSyntax)
                is not { } declaredSymbol
            || declaredSymbol.Interfaces.All(CheckInterfaceSymbolConditions)
            || CheckRecordDeclarationConditions(recordDeclarationSyntax)
        )
        {
            return;
        }

        var diagnostic = Diagnostic.Create(Rule, recordDeclarationSyntax.GetLocation());
        context.ReportDiagnostic(diagnostic);
    }

    private void AnalyzeClassDeclarationSyntax(SyntaxNodeAnalysisContext context)
    {
        if (
            context.Node is not ClassDeclarationSyntax classDeclarationSyntax
            || context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax)
                is not { } interfaceDeclaredSymbol
            || interfaceDeclaredSymbol.Interfaces.All(CheckInterfaceSymbolConditions)
        )
        {
            return;
        }

        var diagnostic = Diagnostic.Create(Rule, classDeclarationSyntax.GetLocation());
        context.ReportDiagnostic(diagnostic);
    }

    private void AnalyzeStructDeclarationSyntax(SyntaxNodeAnalysisContext context)
    {
        if (
            context.Node is not StructDeclarationSyntax structDeclarationSyntax
            || context.SemanticModel.GetDeclaredSymbol(structDeclarationSyntax)
                is not { } declaredSymbol
            || declaredSymbol.Interfaces.All(CheckInterfaceSymbolConditions)
        )
        {
            return;
        }

        var diagnostic = Diagnostic.Create(Rule, structDeclarationSyntax.GetLocation());
        context.ReportDiagnostic(diagnostic);
    }

    private static bool CheckInterfaceSymbolConditions(INamedTypeSymbol typeSymbol)
    {
        var attributes = typeSymbol.GetAttributes();
        return attributes.All(
            a => !(a.AttributeClass?.Name.Equals("DiscriminatedUnionAttribute") ?? false)
        );
    }

    private static bool CheckRecordDeclarationConditions(
        RecordDeclarationSyntax recordDeclarationSyntax
    ) =>
        recordDeclarationSyntax.IsKind(SyntaxKind.RecordStructDeclaration)
        && recordDeclarationSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.ReadOnlyKeyword));
}
