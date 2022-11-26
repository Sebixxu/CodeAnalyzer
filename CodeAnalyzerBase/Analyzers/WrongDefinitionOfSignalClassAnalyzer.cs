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
    public sealed class WrongDefinitionOfSignalClassAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics 
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                    ImmutableInterlocked.InterlockedInitialize(ref _supportedDiagnostics, ImmutableArray.Create(DiagnosticDescriptors.WrongDefinitionOfSignalClass));

                return _supportedDiagnostics;
            }
        }

        private const string BaseTypeSignalName = "AbstractSignal";
        private const string SplitTextByUppercaseWordRegex = @"(?<!^)(?=[A-Z])";
        private const string SingalLastKeyword = "Signal";
        static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(f => AnalyzeWrongDefinitionOfSignalClass(f), SyntaxKind.ClassDeclaration);
        }

        private void AnalyzeWrongDefinitionOfSignalClass(SyntaxNodeAnalysisContext context)
        {
            var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

            if (classDeclarationSyntax.Identifier.Text == BaseTypeSignalName)
            {
                return;
            }

            var lastWordInClassName = Regex.Split(classDeclarationSyntax.Identifier.Text, SplitTextByUppercaseWordRegex).Last();
            if (lastWordInClassName == SingalLastKeyword)
            {
                var modifires = classDeclarationSyntax.Modifiers.Select(x => x.Text);
                var typeSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax) as ITypeSymbol;

                if (typeSymbol == null)
                {
                    return;
                }

                if (!modifires.Contains(Modifiers.Public) || !modifires.Contains(Modifiers.Sealed) || typeSymbol.BaseType.Name != BaseTypeSignalName)
                {
                    var diagnostic = Diagnostic.Create(DiagnosticDescriptors.WrongDefinitionOfSignalClass, classDeclarationSyntax.GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }

        }
    }
}
