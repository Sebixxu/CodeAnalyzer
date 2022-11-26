using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeAnalyzer.Common
{
    /// <summary>
    /// A set of extension methods for syntax (types derived from <see cref="CSharpSyntaxNode" />).
    /// </summary>
    static class SyntaxExtensions
    {
        /// <summary>
        /// Returns true if the list of either empty or contains only whitespace (<see cref="SyntaxKind.WhitespaceTrivia" /> or
        /// <see cref="SyntaxKind.EndOfLineTrivia" />).
        /// </summary>
        internal static bool IsEmptyOrWhitespace(this SyntaxTriviaList triviaList)
        {
            foreach (SyntaxTrivia trivia in triviaList)
                if (!trivia.IsKind(SyntaxKind.WhitespaceTrivia, SyntaxKind.EndOfLineTrivia))
                    return false;

            return true;
        }

        /// <summary>
        /// Returns true if a trivia's kind is one of the specified kinds.
        /// </summary>
        internal static bool IsKind(this SyntaxTrivia trivia, SyntaxKind kind1, SyntaxKind kind2)
        {
            SyntaxKind kind = trivia.Kind();
            return kind == kind1 || kind == kind2;
        }
    }
}