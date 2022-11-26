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
    public class WrongDefinitionOfConfigVariablesTests
    {
        private const string DiagnosticId = "RAD008";

        [SetUp]
        public void Setup()
        {
        }

        //TODO: Rename tests

        [Test]
        public async Task CreatingStaticReadonlyVariableWithValidBaseTypeGeneratesNoDiagnostics()
        {
            string testCode = @"
class ScriptableObject
{

}

class WeaponConfig : ScriptableObject
{

}

class TestClass
{
    static readonly WeaponConfig _weaponConfig;
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongDefinitionOfConfigVariablesAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(0, diagnostics.Length);
        }

        [Test]
        public async Task CreatingReadonlyVariableWithValidBaseTypeGeneratesOneDiagnostics()
        {
            string testCode = @"
class ScriptableObject
{

}

class WeaponConfig : ScriptableObject
{

}

class TestClass
{
    readonly WeaponConfig w1Config;
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongDefinitionOfConfigVariablesAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingStaticVariableWithValidBaseTypeGeneratesOneDiagnostics()
        {
            string testCode = @"
class ScriptableObject
{

}

class WeaponConfig : ScriptableObject
{

}

class TestClass
{
    static WeaponConfig w2Config;
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongDefinitionOfConfigVariablesAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingPublicVariableWithValidBaseTypeGeneratesOneDiagnostics()
        {
            string testCode = @"
class ScriptableObject
{

}

class WeaponConfig : ScriptableObject
{

}

class TestClass
{
    public WeaponConfig w3Config;
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongDefinitionOfConfigVariablesAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingInternalVariableWithValidBaseTypeGeneratesOneDiagnostics()
        {
            string testCode = @"
class ScriptableObject
{

}

class WeaponConfig : ScriptableObject
{

}

class TestClass
{
    internal WeaponConfig w4Config;
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongDefinitionOfConfigVariablesAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingPrivateVariableWithValidBaseTypeGeneratesOneDiagnostics()
        {
            string testCode = @"
class ScriptableObject
{

}

class WeaponConfig : ScriptableObject
{

}

class TestClass
{
    private WeaponConfig w5Config;
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongDefinitionOfConfigVariablesAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingProtectedVariableWithValidBaseTypeGeneratesOneDiagnostics()
        {
            string testCode = @"
class ScriptableObject
{

}

class WeaponConfig : ScriptableObject
{

}

class TestClass
{
    protected WeaponConfig w6Config;
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongDefinitionOfConfigVariablesAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingVariableWithValidBaseTypeGeneratesOneDiagnostics()
        {
            string testCode = @"
class ScriptableObject
{

}

class WeaponConfig : ScriptableObject
{

}

class TestClass
{
    WeaponConfig w7Config;
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongDefinitionOfConfigVariablesAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingPublicStaticVariableWithValidBaseTypeGeneratesOneDiagnostics()
        {
            string testCode = @"
class ScriptableObject
{

}

class WeaponConfig : ScriptableObject
{

}

class TestClass
{
    public static WeaponConfig _w3Config;
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongDefinitionOfConfigVariablesAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingInternalStaticVariableWithValidBaseTypeGeneratesOneDiagnostics()
        {
            string testCode = @"
class ScriptableObject
{

}

class WeaponConfig : ScriptableObject
{

}

class TestClass
{
    internal static WeaponConfig _w4Config;
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongDefinitionOfConfigVariablesAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingPrivateStaticVariableWithValidBaseTypeGeneratesOneDiagnostics()
        {
            string testCode = @"
class ScriptableObject
{

}

class WeaponConfig : ScriptableObject
{

}

class TestClass
{
    private static WeaponConfig _w5Config;
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongDefinitionOfConfigVariablesAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingProtectedStaticVariableWithValidBaseTypeGeneratesOneDiagnostics()
        {
            string testCode = @"
class ScriptableObject
{

}

class WeaponConfig : ScriptableObject
{

}

class TestClass
{
    protected static WeaponConfig _w6Config;
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongDefinitionOfConfigVariablesAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingStaticReadonlyVariableWithNotValidBaseTypeGeneratesOneDiagnostics()
        {
            string testCode = @"
class ScriptableObject
{

}

class WeaponConfigNotScriptable
{

}

class TestClass
{
    static readonly WeaponConfigNotScriptable _weaponConfig;
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongDefinitionOfConfigVariablesAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(1, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingMultipleVariableWithValidBaseTypeGeneratesThreeDiagnostics()
        {
            string testCode = @"
class ScriptableObject
{

}

class WeaponConfig : ScriptableObject
{

}

class TestClass
{
    protected static WeaponConfig _w6Config;
    static readonly WeaponConfig _weaponConfig;

    internal WeaponConfig w4Config;

    static readonly WeaponConfig _weapon2Config;
    readonly WeaponConfig w1Config;
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongDefinitionOfConfigVariablesAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(3, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }

        [Test]
        public async Task CreatingMultipleVariableWithValidAndNotValidBaseTypeGeneratesFourDiagnostics()
        {
            string testCode = @"
class ScriptableObject
{

}

class WeaponConfigNotScriptable
{

}

class WeaponConfig : ScriptableObject
{

}

class TestClass
{
    protected static WeaponConfig _w6Config;

    static readonly WeaponConfig _weaponConfig;
    static readonly WeaponConfigNotScriptable _notWeaponConfig;

    internal WeaponConfigNotScriptable w4Config;

    static readonly WeaponConfig _weapon2Config;
    readonly WeaponConfigNotScriptable w1Config;
}

";
            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongDefinitionOfConfigVariablesAnalyzer());
            ImmutableArray<Diagnostic> diagnostics = await Utilities.GetDiagnostics(analyzers, testCode);

            Assert.AreEqual(4, diagnostics.Length);
            Assert.IsTrue(diagnostics.All(x => x.Id == DiagnosticId));
        }
    }
}