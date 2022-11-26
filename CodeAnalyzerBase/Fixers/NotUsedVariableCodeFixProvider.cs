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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NotUsedVariableCodeFixProvider))]
    public class NotUsedVariableCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DiagnosticId);
        private const string DiagnosticId = "RAD003";
        private const string CodeFixTitle = "Remove unused variable";

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();
            var document = context.Document;

            var root = await document.GetSyntaxRootAsync();
            var node = root.FindNode(diagnostic.Location.SourceSpan);

            if (!(node is VariableDeclaratorSyntax)) 
            {
                throw new Exception("Node is not VariableDeclarationSyntax");
            }

            var parent = node.Parent;
            if (!(parent is VariableDeclarationSyntax))
            {
                throw new Exception("Parent is not VariableDeclarationSyntax");
            }

            var variables = parent.DescendantNodes().OfType<VariableDeclaratorSyntax>();

            context.RegisterCodeFix(CodeAction.Create(CodeFixTitle, async c =>
            {
                SyntaxNode newRoot = null;

                if (variables.Count() == 1)
                {
                    var localDeclarationStatement = parent.Parent;

                    if (!(localDeclarationStatement is LocalDeclarationStatementSyntax))
                    {
                        throw new Exception("Parent of VariableDeclarationSyntax is not VariableDeclarationSyntax");
                    }

                    var localDeclarationStatementNode = root.FindNode(localDeclarationStatement.GetLocation().SourceSpan);
                    newRoot = root.RemoveNode(localDeclarationStatementNode, SyntaxRemoveOptions.KeepNoTrivia);
                }
                else
                {
                    newRoot = root.RemoveNode(node, SyntaxRemoveOptions.KeepNoTrivia);
                }

                return document.WithSyntaxRoot(newRoot);
            }), diagnostic);
        }
    }
}
