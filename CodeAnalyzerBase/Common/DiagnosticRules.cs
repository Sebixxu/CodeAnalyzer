using Microsoft.CodeAnalysis;

namespace CodeAnalyzer.Common
{
    static class DiagnosticDescriptors
    {
        const string Syntax = "Syntax";

        internal static readonly DiagnosticDescriptor RemoveUnnecessaryBlankLine = new DiagnosticDescriptor(
            "RAD000", "Remove unnecessary blank line", "Remove unnecessary blank line", Syntax, DiagnosticSeverity.Error, true);

        internal static readonly DiagnosticDescriptor RemoveTrailingWhitespace = new DiagnosticDescriptor(
            "RAD001", "Remove trailing white-space", "Remove trailing white-space", Syntax, DiagnosticSeverity.Error, true);

        internal static readonly DiagnosticDescriptor RemoveNewLineBetweenIfKeywordAndElseKeyword = new DiagnosticDescriptor(
            "RAD002", "Remove new line between 'if' keyword and 'else' keyword", "Remove new line between 'if' keyword and 'else' keyword", Syntax,
            DiagnosticSeverity.Error, true);

        internal static readonly DiagnosticDescriptor RemoveNotUsedVariable = new DiagnosticDescriptor(
            "RAD003", "Remove not used variable", "Remove not used variable", Syntax,
            DiagnosticSeverity.Error, true);

        internal static readonly DiagnosticDescriptor WrongOrderVariablesRelativeToMethods = new DiagnosticDescriptor(
            "RAD004", "Wrong order of variables relative to methods", "Wrong order of variables relative to methods", Syntax,
            DiagnosticSeverity.Error, true);

        internal static readonly DiagnosticDescriptor VarKeywordUsedForPrimitiveTypes = new DiagnosticDescriptor(
            "RAD005", "Keyword 'var' should not be used for primitive types", "Keyword 'var' should not be used for primitive types", Syntax,
            DiagnosticSeverity.Error, true);

        internal static readonly DiagnosticDescriptor WrongInternalAndPublicVariablesName = new DiagnosticDescriptor(
            "RAD006", "Public and internal variables should start with capital letter", "Public and internal variables should start with capital letter", Syntax,
            DiagnosticSeverity.Error, true);

        internal static readonly DiagnosticDescriptor WrongDefinitionOfSignalClass = new DiagnosticDescriptor(
           "RAD007", "Wrong definition of signal class", "Wrong definition of signal class", Syntax,
           DiagnosticSeverity.Error, true);

        internal static readonly DiagnosticDescriptor WrongDefinitionOfConfigVariables = new DiagnosticDescriptor(
           "RAD008", "Wrong definition of 'Config' variables", "Wrong definition of 'Config' variables", Syntax,
           DiagnosticSeverity.Error, true);
    }
}