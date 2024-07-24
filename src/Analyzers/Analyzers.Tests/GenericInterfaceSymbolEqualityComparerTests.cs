using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace Analyzers.Tests;

public class GenericInterfaceSymbolEqualityComparerTests
{
    [Fact]
    public void TestEquality()
    {
        var publicKeywordToken = SyntaxFactory.Token(SyntaxKind.PublicKeyword);
        AccessorDeclarationSyntax[] accessorDeclarations =
        [
            SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration),
            SyntaxFactory.AccessorDeclaration(SyntaxKind.InitAccessorDeclaration)
        ];

        SyntaxToken[] modifiers = [publicKeywordToken];
        MemberDeclarationSyntax[] members =
        [
            SyntaxFactory
                .PropertyDeclaration(
                    SyntaxFactory.ParseTypeName("int"),
                    SyntaxFactory.Identifier("Count")
                )
                .AddModifiers([publicKeywordToken])
                .AddAccessorListAccessors(accessorDeclarations)
        ];
        var interfaceSyntax = SyntaxFactory
            .InterfaceDeclaration(SyntaxFactory.Identifier("DummyInterface1"))
            .AddModifiers(modifiers)
            .AddMembers(members);

        var implementationSyntax1 = SyntaxFactory
            .StructDeclaration(SyntaxFactory.Identifier("Implementation1"))
            .AddBaseListTypes(
                [SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("DummyInterface1"))]
            )
            .AddModifiers(modifiers)
            .AddMembers(members);

        var implementationSyntax2 = SyntaxFactory
            .StructDeclaration(SyntaxFactory.Identifier("Implementation2"))
            .AddBaseListTypes(
                [SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("DummyInterface1"))]
            )
            .AddModifiers(modifiers)
            .AddMembers(members);
    }
}
