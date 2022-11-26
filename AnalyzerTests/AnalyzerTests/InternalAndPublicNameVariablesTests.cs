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
    public class InternalAndPublicNameVariablesTests
    {
        private const string DiagnosticId = "RAD006";

        [SetUp]
        public void Setup()
        {
        }

        //TODO: Rename tests

        [Test]
        public async Task CreatingInternalIntFieldWithFirstUppercaseGeneratesNoDiagnostics()
        {
            string testCode = @"
class Test6
{
    internal int Variable;
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongInternalAndPublicVariablesNameAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(0, diagnostics.Length);
        }

        [Test]
        public async Task CreatingPublicIntFieldWithFirstUppercaseGeneratesNoDiagnostics()
        {
            string testCode = @"
class Test6
{
    public int Variable2;
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongInternalAndPublicVariablesNameAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(0, diagnostics.Length);
        }

        [Test]
        public async Task CreatingInternalIntPropertyWithFirstUppercaseGeneratesNoDiagnostics()
        {
            string testCode = @"
class Test6
{
    internal int Prop { get; set; }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongInternalAndPublicVariablesNameAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(0, diagnostics.Length);
        }

        [Test]
        public async Task CreatingPublicIntPropertyWithFirstUppercaseGeneratesNoDiagnostics()
        {
            string testCode = @"
class Test6
{
    public int Prop2 { get; set; }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongInternalAndPublicVariablesNameAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(0, diagnostics.Length);
        }

        [Test]
        public async Task CreatingInternalIntFieldWithFirstLowercaseGeneratesOneDiagnostics()
        {
            string testCode = @"
class Test6
{
    internal int badVariable;
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongInternalAndPublicVariablesNameAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingPublicIntFieldWithFirstLowercaseGeneratesOneDiagnostics()
        {
            string testCode = @"
class Test6
{
    public int badVariable2;
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongInternalAndPublicVariablesNameAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingInternalIntPropertyWithFirstLowercaseGeneratesOneDiagnostics()
        {
            string testCode = @"
class Test6
{
    internal int badProp { get; set; }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongInternalAndPublicVariablesNameAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingPublicIntPropertyWithFirstLowercaseGeneratesOneDiagnostics()
        {
            string testCode = @"
class Test6
{
    public int badProp2 { get; set; }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongInternalAndPublicVariablesNameAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingMultipleFieldsAndPropertiesWithFirstLowercaseAndUppercaseGeneratesFourDiagnostics()
        {
            string testCode = @"
class Test6
{
    internal int Variable;
    internal int badVariable;

    public int Variable2;
    public int badVariable2;

    internal int Prop{ get; set; }
    internal int badProp { get; set; }

    public int Prop2 { get; set; }
    public int badProp2 { get; set; }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongInternalAndPublicVariablesNameAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(4, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingProtectedIntPropertyWithFirstLowercaseGeneratesNoDiagnostics()
        {
            string testCode = @"
class Test6
{
    protected int badProp2 { get; set; }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongInternalAndPublicVariablesNameAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(0, diagnostics.Length);
        }

        [Test]
        public async Task CreatingPrivateIntPropertyWithFirstLowercaseGeneratesNoDiagnostics()
        {
            string testCode = @"
class Test6
{
    private int badProp2 { get; set; }
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongInternalAndPublicVariablesNameAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(0, diagnostics.Length);
        }

        [Test]
        public async Task CreatingProtectedIntFieldWithFirstLowercaseGeneratesNoDiagnostics()
        {
            string testCode = @"
class Test6
{
    protected int badVariable;
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongInternalAndPublicVariablesNameAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(0, diagnostics.Length);
        }

        [Test]
        public async Task CreatingPrivateIntFieldWithFirstLowercaseGeneratesNoDiagnostics()
        {
            string testCode = @"
class Test6
{
    private int badVariable;
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongInternalAndPublicVariablesNameAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(0, diagnostics.Length);
        }

        [Test]
        public async Task CreatingMultipleFieldsAndPropertiesWithFirstLowercaseAndUppercaseWithInitializationGeneratesFourDiagnostics()
        {
            string testCode = @"
class Test6
{
    private string text = ""Abc"";

    internal int Variable;
    internal int badVariable;

    public int Variable2;
    public int badVariable2;

    internal int Prop{ get; set; }
    internal int badProp { get; set; }

    public int Prop2 { get; set; }
    public int badProp2 { get; set; }

    protected DateTime dateTime { get; set; } = DateTime.Now;
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongInternalAndPublicVariablesNameAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(4, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingInternalIntFieldsInLineWithLowercaseGeneratesTwoDiagnostic()
        {
            string testCode = @"
class Test6
{
    private string text = ""Abc"";

    internal int Variable;
    internal int badVariable, badVariable0 = 1;
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongInternalAndPublicVariablesNameAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(2, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingInternalIntFieldsInLineWithUppercaseGeneratesNoDiagnostic()
        {
            string testCode = @"
class Test6
{
    private string text = ""Abc"";

    internal int Variable, Variable1 = 2;
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongInternalAndPublicVariablesNameAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(0, diagnostics.Length);
        }
    }
}