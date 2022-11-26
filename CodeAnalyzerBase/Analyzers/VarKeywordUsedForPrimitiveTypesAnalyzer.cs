using CodeAnalyzer.Common;
using CodeAnalyzerBase.Common;
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
    public sealed class VarKeywordUsedForPrimitiveTypesAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                    ImmutableInterlocked.InterlockedInitialize(ref _supportedDiagnostics, ImmutableArray.Create(DiagnosticDescriptors.VarKeywordUsedForPrimitiveTypes));

                return _supportedDiagnostics;
            }
        }

        private const int VarKeywordLength = 3;
        static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(f => AnalyzeVarKeywordUsedForPrimitiveTypes(f), SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeVarKeywordUsedForPrimitiveTypes(SyntaxNodeAnalysisContext context)
        {
            var methodDeclarationSyntax = (MethodDeclarationSyntax)context.Node;

            if (methodDeclarationSyntax.Body == null)
            {
                return;
            }

            var dataFlow = context.SemanticModel.AnalyzeDataFlow(methodDeclarationSyntax.Body);
            var variablesDeclaration = dataFlow.VariablesDeclared;

            var varVariables = methodDeclarationSyntax.DescendantNodes().OfType<VariableDeclarationSyntax>().Where(x => x.Type.IsVar);

            foreach (var variable in variablesDeclaration)
            {
                var localVariableSymbol = variable as ILocalSymbol;
                if (localVariableSymbol == null)
                {
                    continue;
                }

                var variableType = localVariableSymbol.Type as INamedTypeSymbol;
                if (variableType == null)
                {
                    continue;
                }

                string variableName = localVariableSymbol.Name;
                var currentVariable = varVariables.FirstOrDefault(x => x.Variables.First().Identifier.Text == variableName);
                if (currentVariable == null)
                {
                    continue;
                }

                if (variableType.Name.IsPrimitveType())
                {
                    var variableLocation = variable.Locations.FirstOrDefault();
                    var diagnostic = Diagnostic.Create(DiagnosticDescriptors.VarKeywordUsedForPrimitiveTypes, 
                        Location.Create(variableLocation.SourceTree, new Microsoft.CodeAnalysis.Text.TextSpan(currentVariable.SpanStart, VarKeywordLength)));
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
