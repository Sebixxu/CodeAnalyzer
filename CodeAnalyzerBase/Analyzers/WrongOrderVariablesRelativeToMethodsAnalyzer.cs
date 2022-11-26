using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using CodeAnalyzer.Common;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Runtime.Remoting.Contexts;

namespace CodeAnalyzerBase.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class WrongOrderVariablesRelativeToMethodsAnalyzer : DiagnosticAnalyzer 
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get 
            {
                if (_supportedDiagnostics.IsDefault)
                    ImmutableInterlocked.InterlockedInitialize(ref _supportedDiagnostics, ImmutableArray.Create(DiagnosticDescriptors.WrongOrderVariablesRelativeToMethods));

                return _supportedDiagnostics;
            }
        }
        static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(f => AnalyzeWrongOrderVariablesRelativeToMethods(f), SyntaxKind.ClassDeclaration);
        }

        private void AnalyzeWrongOrderVariablesRelativeToMethods(SyntaxNodeAnalysisContext context)
        {
            var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

            SyntaxList<MemberDeclarationSyntax> members = classDeclarationSyntax.Members;

            int indexOfFirstMethod = members.IndexOf(x => x.IsKind(SyntaxKind.MethodDeclaration) || x.IsKind(SyntaxKind.ConstructorDeclaration));
            if (indexOfFirstMethod == -1)
            {
                return;
            }

            var membersBelowFirstMethod = classDeclarationSyntax.Members.Skip(indexOfFirstMethod + 1);
            var variableDeclaretdAfterFirstMethod = membersBelowFirstMethod.Where(x => x.IsKind(SyntaxKind.PropertyDeclaration) 
                                                                                        || x.IsKind(SyntaxKind.FieldDeclaration));

            foreach (var member in variableDeclaretdAfterFirstMethod)
            {
                var location = member.GetLocation();
                var diagnostic = Diagnostic.Create(DiagnosticDescriptors.WrongOrderVariablesRelativeToMethods, location);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
