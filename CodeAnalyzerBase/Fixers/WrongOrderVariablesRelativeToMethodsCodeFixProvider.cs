using CodeAnalyzer.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalyzerBase.Fixers
{
    [Shared]
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(WrongOrderVariablesRelativeToMethodsCodeFixProvider))]
    public class WrongOrderVariablesRelativeToMethodsCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DiagnosticId);
        private const string DiagnosticId = "RAD004";
        private const string CodeFixTitle = "Move variable above first method";

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();
            var document = context.Document;

            var root = await document.GetSyntaxRootAsync();
            var node = root.FindNode(diagnostic.Location.SourceSpan);

            bool isProperty = false;
            if (node is PropertyDeclarationSyntax)
            {
                isProperty = true;
            }

            bool isField = false;
            if (node is FieldDeclarationSyntax)
            {
                isField = true;
            }

            if (!isProperty && !isField)
            {
                throw new Exception("Node is not PropertyDeclarationSyntax and is not FieldDeclarationSyntax");
            }

            var parent = node.Parent;
            if (!(parent is ClassDeclarationSyntax classDeclarationSyntax))
            {
                throw new Exception("Parent is not ClassDeclarationSyntax");
            }

            SyntaxList<MemberDeclarationSyntax> members = classDeclarationSyntax.Members;
            var firstOrDefaultMethodMemberDeclarationSyntax = members.FirstOrDefault(x =>
                                                                x.IsKind(SyntaxKind.MethodDeclaration) || x.IsKind(SyntaxKind.ConstructorDeclaration));

            if (firstOrDefaultMethodMemberDeclarationSyntax == null)
            {
                return;
            }

            context.RegisterCodeFix(CodeAction.Create(CodeFixTitle, async c =>
            {
                SyntaxNode newRoot = null;

                //Remove variable in wrong spot
                newRoot = root.RemoveNode(node, SyntaxRemoveOptions.KeepNoTrivia);

                if (newRoot == null)
                {
                    throw new Exception("New root after RemoveNode was still null");
                }

                //Refresh variables
                document = document.WithSyntaxRoot(newRoot);
                root = await document.GetSyntaxRootAsync();

                //Move variable above first method
                var firstMethodSyntaxNode = root.FindNode(firstOrDefaultMethodMemberDeclarationSyntax.GetLocation().SourceSpan);

                List<SyntaxNode> newNodes = new List<SyntaxNode> { node };
                newRoot = root.InsertNodesBefore(firstMethodSyntaxNode, newNodes);

                return document.WithSyntaxRoot(newRoot);
            }), diagnostic);
        }
    }
}
