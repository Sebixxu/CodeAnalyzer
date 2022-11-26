using CodeAnalyzer.Analyzers;
using CodeAnalyzerBase.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace AnalyzerTests.AnalyzerTests
{
    [TestFixture]
    public class VarKeywordUsedForPrimitiveTypesTests
    {
        private const string DiagnosticId = "RAD005";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task CreatingOneIntVariableWithPrimitiveDeclarationGeneratesNoDiagnostics()
        {
            string testCode = @"
public static class Program
{
    public static void Main()
    {
        int a1 = 1;
    }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new VarKeywordUsedForPrimitiveTypesAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(0, diagnostics.Length);
        }

        [Test]
        public async Task CreatingOneListOfIntsVariableGeneratesNoDiagnostics()
        {
            string testCode = @"
public static class Program
{
    public static void Main()
    {
        List<int> a1 = new List<int>();
    }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new VarKeywordUsedForPrimitiveTypesAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(0, diagnostics.Length);
        }

        [Test]
        public async Task CreatingOneListOfIntsVariableWithVarKeywordGeneratesNoDiagnostics()
        {
            string testCode = @"
public static class Program
{
    public static void Main()
    {
        var a1 = new List<int>();
    }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new VarKeywordUsedForPrimitiveTypesAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(0, diagnostics.Length);
        }

        [Test]
        public async Task CreatingOneIntVariableWithVarKeywordGeneratesOneDiagnostics()
        {
            string testCode = @"
public static class Program
{
    public static void Main()
    {
        var x2 = 1;
        int a1 = 2;
    }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new VarKeywordUsedForPrimitiveTypesAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingOneCharVariableWithVarKeywordGeneratesOneDiagnostics()
        {
            string testCode = @"
public static class Program
{
    public static void Main()
    {
        var x2 = 'x';
        char a1 = 'D';
    }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new VarKeywordUsedForPrimitiveTypesAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingThreeIntBooleanDoubleVariableWithVarKeywordGeneratesThreeDiagnostics()
        {
            string testCode = @"
public static class Program
{
    public static void Main()
    {
        var x2 = 1;
        var x1 = true;
        var x3 = .68;
    }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new VarKeywordUsedForPrimitiveTypesAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(3, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingThreeIntBooleanDoubleVariableWithVarKeywordWithOtherVariablesGeneratesThreeDiagnostics()
        {
            string testCode = @"
public static class Program
{
    public static void Main()
    {
        var x2 = 1;
        var x1 = 'x1';
        var x3 = .68;
        string a = ""test"";
        var stringBuilder = new StringBuilder();
    }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new VarKeywordUsedForPrimitiveTypesAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(3, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }
    }
}