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
    public class WrongOrderVariablesRelativeToMethodsAnalyzerTests
    {
        private const string DiagnosticId = "RAD004";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task CreatingOnePropertyInValidOrderGeneratesNoDiagnostics()
        {
            string testCode = @"
public static class Program
{
    public int Aaa { get; set; }

    public static void Main()
    {
    }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongOrderVariablesRelativeToMethodsAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(0, diagnostics.Length);
        }

        [Test]
        public async Task CreatingOnePropertyInNotValidOrderGeneratesOneDiagnostics()
        {
            string testCode = @"
public static class Program
{
    public int Aaa { get; set; }

    public static void Main()
    {
    }

    public string Bbb { get; set; }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongOrderVariablesRelativeToMethodsAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingOnePropertyInNotValidOrderWithOtherPropertiesGeneratesOneDiagnostics()
        {
            string testCode = @"
public static class Program
{
    public int Aaa { get; set; }
    public string Bbb { get; set; }
    public int MyProperty { get; set; }

    public static void Main()
    {
    }

    public string Bbb2 { get; set; }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongOrderVariablesRelativeToMethodsAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingTwoPropertyInNotValidOrderWithOtherPropertiesGeneratesTwoDiagnostics()
        {
            string testCode = @"
public static class Program
{
    public int Aaa { get; set; }

    public int MyProperty { get; set; }

    public static void Main()
    {
    }

    public string Bbb { get; set; }
    public List<string> Bbb2 { get; set; }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongOrderVariablesRelativeToMethodsAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(2, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingThreePropertyInNotValidOrderWithOtherPropertiesGeneratesThreeDiagnostics()
        {
            string testCode = @"
public static class Program
{
    public int Aaa { get; set; }

    public int MyProperty { get; set; }

    public static void Main()
    {
    }

    public string Bbb { get; set; }
    public List<string> Bbb2 { get; set; }

    public static void Test()
    {
    }

    public List<string> Bbb33 { get; set; }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongOrderVariablesRelativeToMethodsAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(3, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingOneFieldInNotValidOrderWithOtherPropertiesGeneratesOneDiagnostics()
        {
            string testCode = @"
public static class Program
{
    public int Aaa { get; set; }
    public int MyProperty { get; set; }

    public static void Main()
    {
    }

    private int _test;
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongOrderVariablesRelativeToMethodsAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingOneFieldAndOnePropertyInNotValidOrderWithOtherPropertiesGeneratesTwoDiagnostics()
        {
            string testCode = @"
public static class Program
{
    public int Aaa { get; set; }

    public static void Main()
    {
    }

    private int _test;
    public int MyProperty { get; set; }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongOrderVariablesRelativeToMethodsAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(2, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingOnePropertyInNotValidOrderWithOtherPropertySeparatedByMethodGeneratesTwoDiagnostics()
        {
            string testCode = @"
class Test
{
    public int Aaa { get; set; }

    public Test()
    {
    }

    public string Bbb { get; set; }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongOrderVariablesRelativeToMethodsAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }
    }
}