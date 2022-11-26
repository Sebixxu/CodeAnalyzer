using System.Collections.Immutable;
using CodeAnalyzer.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace CodeAnalyzer.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class RedundantNewLineBetweenIfAndElseKeywordAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                    ImmutableInterlocked.InterlockedInitialize(ref _supportedDiagnostics, ImmutableArray.Create(DiagnosticDescriptors.RemoveNewLineBetweenIfKeywordAndElseKeyword));

                return _supportedDiagnostics;
            }
        }
        static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(f => AnalyzeElseClause(f), SyntaxKind.ElseClause);
        }

        static void AnalyzeElseClause(SyntaxNodeAnalysisContext context)
        {
            var elseClause = (ElseClauseSyntax)context.Node;

            StatementSyntax statement = elseClause.Statement;

            if (!statement.IsKind(SyntaxKind.IfStatement))
                return;

            SyntaxTriviaList trailingTrivia = elseClause.ElseKeyword.TrailingTrivia;

            if (!SyntaxTriviaAnalysis.IsOptionalWhitespaceThenEndOfLineTrivia(trailingTrivia))
                return;

            if (!statement.GetLeadingTrivia().IsEmptyOrWhitespace())
                return;

            var diagnostic = Diagnostic.Create(DiagnosticDescriptors.RemoveNewLineBetweenIfKeywordAndElseKeyword,
                Location.Create(elseClause.SyntaxTree, new TextSpan(trailingTrivia.Last().SpanStart, 0)));
            context.ReportDiagnostic(diagnostic);
        }
    }
}