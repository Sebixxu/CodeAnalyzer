using System.Collections.Immutable;
using CodeAnalyzer.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace CodeAnalyzer.Analyzers
{
    /// <summary>
    /// This analyzer throws an error for redundant tailing spaces and for redundant new lines.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class RedundantTailingSpaceAndNewlineAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                    ImmutableInterlocked.InterlockedInitialize(ref _supportedDiagnostics,
                        ImmutableArray.Create(DiagnosticDescriptors.RemoveTrailingWhitespace, DiagnosticDescriptors.RemoveUnnecessaryBlankLine));

                return _supportedDiagnostics;
            }
        }
        static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxTreeAction(f => AnalyzeTrailingTrivia(f));
        }

        static void AnalyzeTrailingTrivia(SyntaxTreeAnalysisContext context)
        {
            if (!context.Tree.TryGetText(out var sourceText))
                return;

            if (!context.Tree.TryGetRoot(out var root))
                return;

            var emptyLines = default(TextSpan);
            bool previousLineIsEmpty = false;
            int i = 0;

            foreach (TextLine textLine in sourceText.Lines)
            {
                bool lineIsEmpty = false;

                if (textLine.Span.Length == 0)
                {
                    SyntaxTrivia endOfLine = root.FindTrivia(textLine.End);

                    if (endOfLine.IsKind(SyntaxKind.EndOfLineTrivia))
                    {
                        lineIsEmpty = true;

                        if (previousLineIsEmpty)
                            emptyLines = emptyLines.IsEmpty ? endOfLine.Span : TextSpan.FromBounds(emptyLines.Start, endOfLine.Span.End);
                    }
                    else
                    {
                        emptyLines = default;
                    }
                }
                else
                {
                    if (!emptyLines.IsEmpty)
                    {
                        var diagnostic = Diagnostic.Create(DiagnosticDescriptors.RemoveUnnecessaryBlankLine, Location.Create(context.Tree, emptyLines));
                        context.ReportDiagnostic(diagnostic);
                    }

                    emptyLines = default;

                    int end = textLine.End - 1;

                    if (char.IsWhiteSpace(sourceText[end]))
                    {
                        int start = end;

                        while (start > textLine.Span.Start && char.IsWhiteSpace(sourceText[start - 1]))
                            start--;

                        TextSpan whitespace = TextSpan.FromBounds(start, end + 1);

                        if (root.FindTrivia(start).IsKind(SyntaxKind.WhitespaceTrivia) || root.FindToken(start, true).IsKind(SyntaxKind.XmlTextLiteralToken))
                        {
                            if (previousLineIsEmpty && start == textLine.Start)
                                whitespace = TextSpan.FromBounds(sourceText.Lines[i - 1].End, whitespace.End);

                            var diagnostic = Diagnostic.Create(DiagnosticDescriptors.RemoveTrailingWhitespace, Location.Create(context.Tree, whitespace));
                            context.ReportDiagnostic(diagnostic);
                        }
                    }
                }

                previousLineIsEmpty = lineIsEmpty;
                i++;
            }
        }
    }
}