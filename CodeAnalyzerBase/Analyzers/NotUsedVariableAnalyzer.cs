using CodeAnalyzer.Common;
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
    public sealed class NotUsedVariableAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                    ImmutableInterlocked.InterlockedInitialize(ref _supportedDiagnostics, ImmutableArray.Create(DiagnosticDescriptors.RemoveNotUsedVariable));

                return _supportedDiagnostics;
            }
        }
        static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(f => AnalyzeNotUsedVariable(f), SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeNotUsedVariable(SyntaxNodeAnalysisContext context)
        {
            var methodDeclarationSyntax = (MethodDeclarationSyntax)context.Node;

            if (methodDeclarationSyntax.Body == null)
            {
                return;
            }

            var dataFlow = context.SemanticModel.AnalyzeDataFlow(methodDeclarationSyntax.Body);

            var variablesUsage = dataFlow.ReadInside;
            var variablesDeclaration = dataFlow.VariablesDeclared;

            var unusedVariables = variablesDeclaration.Except(variablesUsage);

            foreach (var variable in unusedVariables)
            {
                var firstOrDefaultDeclareLocation = variable.Locations.FirstOrDefault();

                if (firstOrDefaultDeclareLocation == null)
                {
                    continue;
                }

                var diagnostic = Diagnostic.Create(DiagnosticDescriptors.RemoveNotUsedVariable, firstOrDefaultDeclareLocation);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
