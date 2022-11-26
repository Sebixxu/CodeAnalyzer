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
    public class NotUsedVariableTests
    {
        private const string DiagnosticId = "RAD003";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task CreatingOneUsedVariableGeneratesNoDiagnostics()
        {
            string testCode = @"
public static class Program
{
    public static void Main()
    {
        int a = 1;
        if(a == 1)
        {

        }

        a = a + 1;
    }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new NotUsedVariableAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(0, diagnostics.Length);
        }


        [Test]
        public async Task CreatingOneNotUsedVariableGeneratesOneDiagnostics()
        {
            string testCode = @"
public static class Program
{
    public static void Main()
    {
        int a = 1;
    }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new NotUsedVariableAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingOneNotUsedVariableInForeachGeneratesOneDiagnostics()
        {
            string testCode = @"
public static class Program
{
    public static void Main()
    {
        var list = new List<int> { 1, 2, 3 };

        foreach (var item in list)
        {

        }
    }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new NotUsedVariableAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingOneNotUsedVarVariableGeneratesOneDiagnostics()
        {
            string testCode = @"
public static class Program
{
    public static void Main()
    {
        var a = 1;
    }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new NotUsedVariableAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingOneNotUsedVariableFromOutVarGeneratesOneDiagnostics()
        {
            string testCode = @"
public static class Program
{
    public static void Main()
    {
        int.TryParse(""1"", out var test);
    }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new NotUsedVariableAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }
    }
}