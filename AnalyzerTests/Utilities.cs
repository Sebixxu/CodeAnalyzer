using CodeAnalyzerBase.Analyzers;
using CodeAnalyzerBase.Fixers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnalyzerTests.AnalyzerTests
{
    public static class Utilities
    {
        public static async Task<ImmutableArray<Diagnostic>> GetDiagnostics(ImmutableArray<DiagnosticAnalyzer> supportedAnalyzers, string testCode)
        {
            var result = await GetDiagnosticsAdvanced(supportedAnalyzers, testCode);
            return result.diagnostics;
        }

        public static async Task<(ImmutableArray<Diagnostic> diagnostics, Document document, AdhocWorkspace workspace)>
            GetDiagnosticsAdvanced(ImmutableArray<DiagnosticAnalyzer> supportedAnalyzers, string testCode)
        {
            AdhocWorkspace workspace = new AdhocWorkspace();
            var solution = workspace.CurrentSolution;

            var projectId = ProjectId.CreateNewId();

            solution = solution.AddProject(projectId, "AnalyerTestsProject", "AnalyerTestsProject", LanguageNames.CSharp);

            DocumentId documentId = DocumentId.CreateNewId(projectId);
            solution = solution.AddDocument(documentId, "TestFile.cs", testCode);

            var project = solution.GetProject(projectId);
            project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

            if (!workspace.TryApplyChanges(project.Solution))
            {
                throw new Exception("Can't apply changes to workspace by specified solution.");
            }

            var compilation = await project.GetCompilationAsync();
            var compilationWithAnaliyzer = compilation.WithAnalyzers(supportedAnalyzers);
            var diagnostics = await compilationWithAnaliyzer.GetAnalyzerDiagnosticsAsync();

            return (diagnostics, workspace.CurrentSolution.GetDocument(documentId), workspace);
        }

        public static async Task<string> ManageRunCodeFixTest(ImmutableArray<CodeFixProvider> supportedCodeFixProviders, 
            ImmutableArray<DiagnosticAnalyzer> supportedAnalyzers, string testCode, string expectedCode)
        {
            var (diagnostics, document, workspace) = await GetDiagnosticsAdvanced(supportedAnalyzers, testCode);

            var codeFixProvider = supportedCodeFixProviders.First();

            CodeAction registerCodeAction = null;
            var diagnostic = diagnostics.FirstOrDefault();

            if (diagnostic == null)
            {
                throw new Exception("No diagnostic found");
            }

            var context = new CodeFixContext(document, diagnostic, (codeAction, _) =>
            {
                if (registerCodeAction != null)
                {
                    throw new Exception("Code action was already registered.");
                }

                registerCodeAction = codeAction;
            }, CancellationToken.None);

            await codeFixProvider.RegisterCodeFixesAsync(context);

            if (registerCodeAction == null)
            {
                throw new Exception("Code action was not registered.");
            }

            var operations = await registerCodeAction.GetOperationsAsync(CancellationToken.None);
            foreach (var operation in operations)
            {
                operation.Apply(workspace, CancellationToken.None);
            }

            var updatedDocument = workspace.CurrentSolution.GetDocument(document.Id);

            var newCode = await updatedDocument.GetTextAsync();

            return newCode.ToString();
        }
    }
}