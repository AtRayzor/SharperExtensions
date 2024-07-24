using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Analyzers;

[ExportCodeFixProvider(
    LanguageNames.CSharp,
    Name = nameof(DiscriminatedUnionCaseDeclarationCodeFixProvider)
)]
public sealed class DiscriminatedUnionCaseDeclarationCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiscriminatedUnionCaseDeclarationAnalyzer.DiagnosticId);

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        if (
            context.Diagnostics.SingleOrDefault() is not { } diagnostic
            || (
                await context
                    .Document
                    .GetSyntaxRootAsync(context.CancellationToken)
                    .ConfigureAwait(false)
            )
                is not { } root
            || root.FindNode(diagnostic.Location.SourceSpan)
                is not TypeDeclarationSyntax typeDeclarationSyntax
        )
        {
            return;
        }

        var codeFix = CodeAction.Create(
            Resources.NF0002CodeFixTitle,
            c => FixCaseDeclaration(context.Document, typeDeclarationSyntax, c),
            nameof(Resources.NF0002CodeFixTitle)
        );
    }

    private async Task<Document> FixCaseDeclaration(
        Document document,
        TypeDeclarationSyntax typeDeclarationSyntax,
        CancellationToken cancellationToken
    )
    {
        var newCaseRecordStructSyntax = (TypeDeclarationSyntax)
            SyntaxFactory
                .RecordDeclaration(
                    SyntaxKind.RecordStructDeclaration,
                    SyntaxFactory.Token(SyntaxKind.StructKeyword),
                    typeDeclarationSyntax.Identifier
                )
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword))
                .AddBaseListTypes(items: typeDeclarationSyntax.BaseList?.Types.ToArray() ?? [])
                .AddAttributeLists(typeDeclarationSyntax.AttributeLists.ToArray());

        if (await document.GetSyntaxRootAsync(cancellationToken) is not { } currentSyntaxRoot)
        {
            return document;
        }

        var newRoot = currentSyntaxRoot.ReplaceNode(
            typeDeclarationSyntax,
            newCaseRecordStructSyntax
        );
        return document.WithSyntaxRoot(newRoot);
    }
}
