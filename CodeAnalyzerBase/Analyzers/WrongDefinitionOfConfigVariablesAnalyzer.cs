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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeAnalyzerBase.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class WrongDefinitionOfConfigVariablesAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                    ImmutableInterlocked.InterlockedInitialize(ref _supportedDiagnostics, ImmutableArray.Create(DiagnosticDescriptors.WrongDefinitionOfConfigVariables));

                return _supportedDiagnostics;
            }
        }

        private const string BaseTypeScriptableObjectName = "ScriptableObject";
        private const string SupportedSufix = "Config";
        private const string SplitTextByUppercaseWordRegex = @"(?<!^)(?=[A-Z])";
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

            var variables = methodDeclarationSyntax.Members.Where(x => x.IsKind(SyntaxKind.FieldDeclaration));

            if (!variables.Any())
            {
                return;
            }

            foreach (var variable in variables)
            {
                VariableDeclarationSyntax variableDeclarationSyntax = null;

                if (variable.IsKind(SyntaxKind.FieldDeclaration))
                {
                    variableDeclarationSyntax = variable.DescendantNodes().OfType<VariableDeclarationSyntax>().FirstOrDefault();
                }

                if (variableDeclarationSyntax == null)
                {
                    continue;
                }

                foreach (var variableInLine in variableDeclarationSyntax.Variables)
                {
                    string currentVariableName = variableInLine.Identifier.Value as string;
                    if (currentVariableName != null && !IsConfigVariable(currentVariableName))
                    {
                        return;
                    }

                    var variableInLineSymbol = context.SemanticModel.GetDeclaredSymbol(variableInLine);
                    if (variableInLineSymbol == null)
                    {
                        return;
                    }

                    var fieldSymbol = variableInLineSymbol as IFieldSymbol;
                    if (fieldSymbol == null)
                    {
                        return;
                    }

                    var nameTypeSymbol = fieldSymbol.Type as INamedTypeSymbol;
                    if (nameTypeSymbol == null)
                    {
                        return;
                    }

                    string variableBaseTypeName = string.Empty;
                    if (nameTypeSymbol.BaseType != null)
                    {
                        variableBaseTypeName = nameTypeSymbol.BaseType.Name;
                    }

                    var modifires = variable.Modifiers.Select(x => x.Text);
                    if (!modifires.Contains(Modifiers.Static) || !modifires.Contains(Modifiers.Readonly) || variableBaseTypeName != BaseTypeScriptableObjectName)
                    {
                        var diagnostic = Diagnostic.Create(DiagnosticDescriptors.WrongDefinitionOfConfigVariables, variableInLine.Identifier.GetLocation());
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

        private bool IsConfigVariable(string text)
        {
            var lastWord = Regex.Split(text, SplitTextByUppercaseWordRegex).Last();
            if (lastWord == SupportedSufix)
            {
                return true;
            }

            return false;
        }
    }
}
