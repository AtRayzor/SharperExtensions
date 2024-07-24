using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DiscriminatedUnionDeclarationFileAnalyzer : DiagnosticAnalyzer
{
    private const string DiagnosticId = "NF0003";
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

    private static readonly DiagnosticDescriptor Rule =
        new(
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
            AnalyzeCaseDefinitionFile,
            SyntaxKind.RecordStructDeclaration
        );
    }

    private void AnalyzeCaseDefinitionFile(SyntaxNodeAnalysisContext context)
    {
        if (
            context.Node is not RecordDeclarationSyntax recordDeclarationSyntax
            || recordDeclarationSyntax.Kind() is not SyntaxKind.RecordStructDeclaration
            || context.SemanticModel.GetDeclaredSymbol(recordDeclarationSyntax)
                is not { } caseDeclaredSymbol
            || GetInterfaceSymbolOrDefault(caseDeclaredSymbol) is not { } interfaceDeclaredSymbol
            || FilePathsMatch(caseDeclaredSymbol, interfaceDeclaredSymbol)
        )
        {
            return;
        }

        var diagnostic = Diagnostic.Create(
            Rule,
            recordDeclarationSyntax.GetLocation(),
            caseDeclaredSymbol.Name,
            interfaceDeclaredSymbol.Name
        );
        context.ReportDiagnostic(diagnostic);
    }

    private static INamedTypeSymbol? GetInterfaceSymbolOrDefault(INamedTypeSymbol declaredSymbol) =>
        declaredSymbol
            .Interfaces
            .FirstOrDefault(
                i =>
                    i.GetAttributes()
                        .Any(
                            a => (bool)a.AttributeClass?.Name.Equals("DiscriminatedUnionAttribute")
                        )
            );

    private static bool FilePathsMatch(
        INamedTypeSymbol caseDeclaredSymbol,
        INamedTypeSymbol interfaceDeclaredSymbol
    ) =>
        interfaceDeclaredSymbol.Locations.Length == 1
        && caseDeclaredSymbol.Locations.Length == 1
        && caseDeclaredSymbol.Locations[0].SourceTree?.FilePath is { } caseFilePath
        && interfaceDeclaredSymbol.Locations[0].SourceTree?.FilePath is { } interfaceFilePath
        && caseFilePath.Equals(interfaceFilePath);
}
