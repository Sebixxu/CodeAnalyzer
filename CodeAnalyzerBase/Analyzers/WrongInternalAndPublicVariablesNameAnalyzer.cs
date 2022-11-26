using CodeAnalyzer.Common;
using CodeAnalyzerBase.Common.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalyzerBase.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class WrongInternalAndPublicVariablesNameAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                    ImmutableInterlocked.InterlockedInitialize(ref _supportedDiagnostics, ImmutableArray.Create(DiagnosticDescriptors.WrongInternalAndPublicVariablesName));

                return _supportedDiagnostics;
            }
        }
        static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(f => AnalyzeNotUsedVariable(f), SyntaxKind.ClassDeclaration);
        }

        private void AnalyzeNotUsedVariable(SyntaxNodeAnalysisContext context)
        {
            var methodDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

            var variables = methodDeclarationSyntax.Members.Where(x => x.IsKind(SyntaxKind.PropertyDeclaration)
                                                                                        || x.IsKind(SyntaxKind.FieldDeclaration));

            var internalAndPublicVariables = variables.Where(x => x.Modifiers.Any(y => IsInternalOrPublic(y.Text)));

            foreach (var variable in internalAndPublicVariables)
            {
                string currentVariableName;

                VariableDeclarationSyntax variableDeclarationSyntax = null;
                PropertyDeclarationSyntax propertyDeclarationSyntax = null;

                if (variable.IsKind(SyntaxKind.FieldDeclaration))
                {
                    variableDeclarationSyntax = variable.DescendantNodes().OfType<VariableDeclarationSyntax>().FirstOrDefault();
                }

                if (variable.IsKind(SyntaxKind.PropertyDeclaration))
                {
                    propertyDeclarationSyntax = variable as PropertyDeclarationSyntax;
                }

                if (variableDeclarationSyntax != null)
                {
                    foreach (var variableInLine in variableDeclarationSyntax.Variables)
                    {
                        currentVariableName = variableInLine.Identifier.Value as string;

                        IfFirstCharIsNotUpperRaiseDiagnostic(context, variableInLine.Identifier, currentVariableName);
                    }
                }

                if (propertyDeclarationSyntax != null)
                {
                    currentVariableName = propertyDeclarationSyntax.Identifier.Value as string;
                    IfFirstCharIsNotUpperRaiseDiagnostic(context, propertyDeclarationSyntax.Identifier, currentVariableName);
                }
            }
        }

        private static void IfFirstCharIsNotUpperRaiseDiagnostic(SyntaxNodeAnalysisContext context, SyntaxToken syntaxToken, string currentVariableName)
        {
            if (!char.IsUpper(currentVariableName[0]))
            {
                var diagnostic = Diagnostic.Create(DiagnosticDescriptors.WrongInternalAndPublicVariablesName, syntaxToken.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }

        private bool IsInternalOrPublic(string text)
        {
            if (text == Modifiers.Internal || text == Modifiers.Public)
            {
                return true;
            }

            return false;
        }
    }
}
