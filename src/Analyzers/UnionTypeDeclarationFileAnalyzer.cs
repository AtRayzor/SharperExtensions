using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace SharperExtensions.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UnionTypeDeclarationFileAnalyzer : DiagnosticAnalyzer
{
    internal static string DiagnosticId => Configuration.DiagnosticId;
    internal static DiagnosticDescriptor Rule => Configuration.Rule;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Configuration.Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(
            AnalyzeCaseDefinitionFile,
            SyntaxKind.ClassDeclaration,
            SyntaxKind.RecordDeclaration
        );
    }

    private void AnalyzeCaseDefinitionFile(SyntaxNodeAnalysisContext context)
    {
        if (
            context.Node is not TypeDeclarationSyntax typeDeclarationSyntax
            || typeDeclarationSyntax.Kind()
                is not (SyntaxKind.ClassDeclaration or SyntaxKind.RecordDeclaration)
            || context.SemanticModel.GetDeclaredSymbol(typeDeclarationSyntax)
                is not { BaseType: { } caseDeclarationBaseSymbol } caseDeclarationSymbol
            || caseDeclarationBaseSymbol
                .GetAttributes()
                .All(att => att is not { AttributeClass.Name: "ClosedAttribute" })
            || FilePathsMatch(caseDeclarationSymbol, caseDeclarationBaseSymbol)
        )
            return;

        var diagnostic = Diagnostic.Create(
            Configuration.Rule,
            typeDeclarationSyntax.GetLocation(),
            caseDeclarationSymbol.Name,
            caseDeclarationBaseSymbol.Name
        );
        context.ReportDiagnostic(diagnostic);
    }

    private static bool FilePathsMatch(
        INamedTypeSymbol caseDeclaredSymbol,
        INamedTypeSymbol baseDeclaredSymbol
    )
    {
        return baseDeclaredSymbol is { Locations.Length: 1 }
            && caseDeclaredSymbol is { Locations.Length: 1 }
            && caseDeclaredSymbol.Locations[0] is { SourceTree.FilePath: { } caseFilePath }
            && baseDeclaredSymbol.Locations[0] is { SourceTree.FilePath: { } baseFilePath }
            && caseFilePath.Equals(baseFilePath);
    }
}

file static class Configuration
{
    public const string DiagnosticId = "NF0003";
    private const string Category = "Unkown";

    private static readonly LocalizableString Title = new LocalizableResourceString(
        nameof(Resources.NF0003Title),
        Resources.ResourceManager,
        typeof(Resources)
    );

    private static readonly LocalizableString Description = new LocalizableResourceString(
        nameof(Resources.NF0003Description),
        Resources.ResourceManager,
        typeof(Resources)
    );

    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
        nameof(Resources.NF0003MessageFormat),
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
