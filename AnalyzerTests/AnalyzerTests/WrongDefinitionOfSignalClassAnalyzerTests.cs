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
    public class WrongDefinitionOfSignalClassAnalyzerTests
    {
        private const string DiagnosticId = "RAD007";

        [SetUp]
        public void Setup()
        {
        }

        //TODO: Rename tests

        [Test]
        public async Task CreatingPublicSealedClassWithAbstracSignalBaseTypeGeneratesNoDiagnostics()
        {
            string testCode = @"
 public abstract class AbstractSignal
 {
     protected AbstractSignal()
     {
     }
 }

public sealed class AmmoChangedSignal : AbstractSignal { }

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongDefinitionOfSignalClassAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(0, diagnostics.Length);
        }


        [Test]
        public async Task CreatingPublicSealedClassWithoutAbstracSignalBaseTypeGeneratesOneDiagnostics()
        {
            string testCode = @"
 public abstract class AbstractSignal
 {
     protected AbstractSignal()
     {
     }
 }

public sealed class AmmoChangedSignal { }

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongDefinitionOfSignalClassAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingPublicClassWithAbstracSignalBaseTypeGeneratesOneDiagnostics()
        {
            string testCode = @"
 public abstract class AbstractSignal
 {
     protected AbstractSignal()
     {
     }
 }

public class AmmoChangedSignal : AbstractSignal { }

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongDefinitionOfSignalClassAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingSealedClassWithAbstracSignalBaseTypeGeneratesOneDiagnostics()
        {
            string testCode = @"
 public abstract class AbstractSignal
 {
     protected AbstractSignal()
     {
     }
 }

sealed class AmmoChangedSignal : AbstractSignal { }

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongDefinitionOfSignalClassAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingPublicClassWithoutAbstracSignalBaseTypeGeneratesOneDiagnostics()
        {
            string testCode = @"
 public abstract class AbstractSignal
 {
     protected AbstractSignal()
     {
     }
 }

public class AmmoChangedSignal { }

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongDefinitionOfSignalClassAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingSealedClassWithoutAbstracSignalBaseTypeGeneratesOneDiagnostics()
        {
            string testCode = @"
 public abstract class AbstractSignal
 {
     protected AbstractSignal()
     {
     }
 }

sealed class AmmoChangedSignal { }

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongDefinitionOfSignalClassAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingNotSignalClassCreatingNoDiagnostics()
        {
            string testCode = @"

public class MyNotSignalTestClass { }


";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongDefinitionOfSignalClassAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(0, diagnostics.Length);
        }

        [Test]
        public async Task CreatingMultipleSignalClassesWithAndWithoutAbstracSignalBaseTypeGeneratesThreeDiagnostics()
        {
            string testCode = @"

 public abstract class AbstractSignal
 {
     protected AbstractSignal()
     {
     }
 }

public sealed class AmmoChangedSignal : AbstractSignal { }

public class AmmoSignal : AbstractSignal { }

sealed class AmmoChangedSignal : AbstractSignal { }

public sealed class AmmoChangedTestSignal { }

public sealed class OtherGoodSignal : AbstractSignal { }

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongDefinitionOfSignalClassAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(3, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }
    }
}