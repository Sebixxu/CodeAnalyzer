using CodeAnalyzer.Common;
using CodeAnalyzerBase.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalyzerBase.Fixers
{
    [Shared]
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(VarKeywordUsedForPrimitiveTypesCodeFixProvider))]
    public class VarKeywordUsedForPrimitiveTypesCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(DiagnosticId);
        private const string DiagnosticId = "RAD005";
        private const string CodeFixTitle = "Replace 'var' keyword by primitive type";

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();
            var document = context.Document;

            var root = await document.GetSyntaxRootAsync();
            var node = root.FindNode(diagnostic.Location.SourceSpan);

            if (!(node is IdentifierNameSyntax oldIdentifierNameSyntax)) 
            {
                throw new Exception("Node is not IdentifierNameSyntax");
            }

            var parent = node.Parent;
            if (!(parent is VariableDeclarationSyntax variableDeclarationSyntax))
            {
                throw new Exception("Parent is not VariableDeclarationSyntax");
            }

            var sematnicModel = await context.Document.GetSemanticModelAsync();

            var symbol = sematnicModel.GetSymbolInfo(variableDeclarationSyntax.Type).Symbol;

            var variableType = symbol as INamedTypeSymbol;
            if (variableType == null)
            {
                return;
            }

            context.RegisterCodeFix(CodeAction.Create(CodeFixTitle, async c =>
            {
                var localDeclarationStatementSyntax = parent.Parent as LocalDeclarationStatementSyntax;

                var newIdentifierNameSyntax = SyntaxFactory.IdentifierName(variableType.Name.ParseVariableType()).WithAdditionalAnnotations(Formatter.Annotation);
                var newDeclaration = localDeclarationStatementSyntax.ReplaceNode(oldIdentifierNameSyntax, newIdentifierNameSyntax);

                var newRoot = root.ReplaceNode(localDeclarationStatementSyntax, newDeclaration);

                return document.WithSyntaxRoot(newRoot);
            }), diagnostic);
        }
    }
}
