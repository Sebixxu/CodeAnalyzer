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
    public class NotUsedVariableCodeFixProviderTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task CreatingThreeNotUsedVariablesInOneLine_CodeFixRemovesFirstOne()
        {
            string testCode = @"
public static class Program
{
    public static void Main()
    {
        int a = 1, b, c = 2;
    }
}

";

            string expectedCode = @"
public static class Program
{
    public static void Main()
    {
        int b, c = 2;
    }
}

";

            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new NotUsedVariableAnalyzer());
            ImmutableArray<CodeFixProvider> codeFixProviders = ImmutableArray.Create<CodeFixProvider>(new NotUsedVariableCodeFixProvider());
            string newCode = await Utilities.ManageRunCodeFixTest(codeFixProviders, analyzers, testCode, expectedCode);
            Assert.AreEqual(expectedCode, newCode.ToString());
        }

        [Test]
        public async Task CreatingOneNotUsedVariableInOneLine_CodeFixRemovesLine()
        {
            string testCode = @"
public static class Program
{
    public static void Main()
    {
        int c = 2;
    }
}

";

            string expectedCode = @"
public static class Program
{
    public static void Main()
    {
    }
}

";

            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new NotUsedVariableAnalyzer());
            ImmutableArray<CodeFixProvider> codeFixProviders = ImmutableArray.Create<CodeFixProvider>(new NotUsedVariableCodeFixProvider());
            string newCode = await Utilities.ManageRunCodeFixTest(codeFixProviders, analyzers, testCode, expectedCode);
            Assert.AreEqual(expectedCode, newCode.ToString());
        }

        [Test]
        public async Task CreatingOneNotUsedVariableAndOneUsedInOneLine_CodeFixRemovesNotUsedOne()
        {
            string testCode = @"
public static class Program
{
    public static void Main()
    {
        int a = 1, b = 2;

        if(b == 2)
        {
        }
    }
}

";

            string expectedCode = @"
public static class Program
{
    public static void Main()
    {
        int b = 2;

        if(b == 2)
        {
        }
    }
}

";

            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new NotUsedVariableAnalyzer());
            ImmutableArray<CodeFixProvider> codeFixProviders = ImmutableArray.Create<CodeFixProvider>(new NotUsedVariableCodeFixProvider());
            string newCode = await Utilities.ManageRunCodeFixTest(codeFixProviders, analyzers, testCode, expectedCode);
            Assert.AreEqual(expectedCode, newCode.ToString());
        }

        [Test]
        public async Task CreatingTwoUsedVariablesInOneLine_CodeFixDoNothing()
        {
            string testCode = @"
public static class Program
{
    public static void Main()
    {
        int a = 1, b = 2;

        if(a == 1 && b == 2)
        {
        }
    }
}

";

            string expectedCode = @"
public static class Program
{
    public static void Main()
    {
        int a = 1, b = 2;

        if(a == 1 && b == 2)
        {
        }
    }
}

";

            string newCode = string.Empty;
            Exception ex = Assert.ThrowsAsync<Exception>(async () =>
            {
                ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new NotUsedVariableAnalyzer());
                ImmutableArray<CodeFixProvider> codeFixProviders = ImmutableArray.Create<CodeFixProvider>(new NotUsedVariableCodeFixProvider());
                newCode = await Utilities.ManageRunCodeFixTest(codeFixProviders, analyzers, testCode, expectedCode);
            });

            Assert.That(ex.Message, Is.EqualTo("No diagnostic found"));
        }
    }
}
