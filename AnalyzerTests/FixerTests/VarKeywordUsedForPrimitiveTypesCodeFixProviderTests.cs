using AnalyzerTests.AnalyzerTests;
using CodeAnalyzerBase.Analyzers;
using CodeAnalyzerBase.Fixers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AnalyzerTests.FixerTests
{
    [TestFixture]
    public class VarKeywordUsedForPrimitiveTypesCodeFixProviderTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task CreatingOneVarVariableWithIntegerType_CodeChangeVarToInt()
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

            string expectedCode = @"
public static class Program
{
    public static void Main()
    {
        int x2 = 1;
        int a1 = 2;
    }
}
";

            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new VarKeywordUsedForPrimitiveTypesAnalyzer());
            ImmutableArray<CodeFixProvider> codeFixProviders = ImmutableArray.Create<CodeFixProvider>(new VarKeywordUsedForPrimitiveTypesCodeFixProvider());
            string newCode = await Utilities.ManageRunCodeFixTest(codeFixProviders, analyzers, testCode, expectedCode);
            Assert.AreEqual(expectedCode, newCode.ToString());
        }

        [Test]
        public async Task CreatingOneVarVariableWithDoubleType_CodeChangeVarToDouble()
        {
            string testCode = @"
public static class Program
{
    public static void Main()
    {
        int a1 = 2;
        var x2 = .1;
    }
}
";

            string expectedCode = @"
public static class Program
{
    public static void Main()
    {
        int a1 = 2;
        double x2 = .1;
    }
}
";

            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new VarKeywordUsedForPrimitiveTypesAnalyzer());
            ImmutableArray<CodeFixProvider> codeFixProviders = ImmutableArray.Create<CodeFixProvider>(new VarKeywordUsedForPrimitiveTypesCodeFixProvider());
            string newCode = await Utilities.ManageRunCodeFixTest(codeFixProviders, analyzers, testCode, expectedCode);
            Assert.AreEqual(expectedCode, newCode.ToString());
        }

        [Test]
        public async Task CreatingOneVarVariableWithBooleanType_CodeChangeVarToBool()
        {
            string testCode = @"
public static class Program
{
    public static void Main()
    {
        var x1 = true;
    }
}
";

            string expectedCode = @"
public static class Program
{
    public static void Main()
    {
        bool x1 = true;
    }
}
";

            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new VarKeywordUsedForPrimitiveTypesAnalyzer());
            ImmutableArray<CodeFixProvider> codeFixProviders = ImmutableArray.Create<CodeFixProvider>(new VarKeywordUsedForPrimitiveTypesCodeFixProvider());
            string newCode = await Utilities.ManageRunCodeFixTest(codeFixProviders, analyzers, testCode, expectedCode);
            Assert.AreEqual(expectedCode, newCode.ToString());
        }

        [Test]
        public async Task CreatingOneVarVariableWithCharType_CodeChangeVarToChar()
        {
            string testCode = @"
public static class Program
{
    public static void Main()
    {
        var x1 = 'x1';
    }
}
";

            string expectedCode = @"
public static class Program
{
    public static void Main()
    {
        char x1 = 'x1';
    }
}
";

            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new VarKeywordUsedForPrimitiveTypesAnalyzer());
            ImmutableArray<CodeFixProvider> codeFixProviders = ImmutableArray.Create<CodeFixProvider>(new VarKeywordUsedForPrimitiveTypesCodeFixProvider());
            string newCode = await Utilities.ManageRunCodeFixTest(codeFixProviders, analyzers, testCode, expectedCode);
            Assert.AreEqual(expectedCode, newCode.ToString());
        }

        [Test]
        public async Task CreatingMultipleVarVariableWithCharType_CodeChangeFirstLineVarToChar()
        {
            string testCode = @"
public static class Program
{
    public static void Main()
    {
        var x1 = 'x1', x2 = 'x2';
        var a1 = 1, a2 = 2;
    }
}
";

            string expectedCode = @"
public static class Program
{
    public static void Main()
    {
        char x1 = 'x1', x2 = 'x2';
        var a1 = 1, a2 = 2;
    }
}
";

            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new VarKeywordUsedForPrimitiveTypesAnalyzer());
            ImmutableArray<CodeFixProvider> codeFixProviders = ImmutableArray.Create<CodeFixProvider>(new VarKeywordUsedForPrimitiveTypesCodeFixProvider());
            string newCode = await Utilities.ManageRunCodeFixTest(codeFixProviders, analyzers, testCode, expectedCode);
            Assert.AreEqual(expectedCode, newCode.ToString());
        }

        [Test]
        public async Task CreatingMultipleVariableWithoutVarKeywordCharType_CodeChangeDoNothing()
        {
            string testCode = @"
public static class Program
{
    public static void Main()
    {
        int a = 1, b = 2;
        bool b1 = true;
    }
}

";

            string expectedCode = @"
public static class Program
{
    public static void Main()
    {
        int a = 1, b = 2;
        bool b1 = true;
    }
}

";

            string newCode = string.Empty;
            Exception ex = Assert.ThrowsAsync<Exception>(async () =>
            {
                ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new VarKeywordUsedForPrimitiveTypesAnalyzer());
                ImmutableArray<CodeFixProvider> codeFixProviders = ImmutableArray.Create<CodeFixProvider>(new VarKeywordUsedForPrimitiveTypesCodeFixProvider());
                newCode = await Utilities.ManageRunCodeFixTest(codeFixProviders, analyzers, testCode, expectedCode);
            });

            Assert.That(ex.Message, Is.EqualTo("No diagnostic found"));
        }
    }
}
