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
    public class WrongOrderVariablesRelativeToMethodsCodeFixProviderTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task CreatingOnePropertyAfterConstructor_CodeFixMovePropertyBeforeConstructor()
        {
            string testCode = @"
public static class Program
{
    public int Aaa { get; set; }

    public static void Main()
    {
    }

    public string Ccc { get; set; }
}
";

            string expectedCode = @"
public static class Program
{
    public int Aaa { get; set; }

    public string Ccc { get; set; }

    public static void Main()
    {
    }
}
";

            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongOrderVariablesRelativeToMethodsAnalyzer());
            ImmutableArray<CodeFixProvider> codeFixProviders = ImmutableArray.Create<CodeFixProvider>(new WrongOrderVariablesRelativeToMethodsCodeFixProvider());
            string newCode = await Utilities.ManageRunCodeFixTest(codeFixProviders, analyzers, testCode, expectedCode);
            Assert.AreEqual(expectedCode, newCode.ToString());
        }

        [Test]
        public async Task CreatingOneFieldAfterConstructor_CodeFixMoveFieldBeforeConstructor()
        {
            string testCode = @"
public static class Program
{
    public int Aaa { get; set; }

    public static void Main()
    {
    }

    private int _test;
}
";

            string expectedCode = @"
public static class Program
{
    public int Aaa { get; set; }

    private int _test;

    public static void Main()
    {
    }
}
";

            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongOrderVariablesRelativeToMethodsAnalyzer());
            ImmutableArray<CodeFixProvider> codeFixProviders = ImmutableArray.Create<CodeFixProvider>(new WrongOrderVariablesRelativeToMethodsCodeFixProvider());
            string newCode = await Utilities.ManageRunCodeFixTest(codeFixProviders, analyzers, testCode, expectedCode);
            Assert.AreEqual(expectedCode, newCode.ToString());
        }

        [Test]
        public async Task CreatingOnePropertyAfterMethod_CodeFixMovePropertyBeforeMethod()
        {
            string testCode = @"
public static class Program
{
    public int Aaa { get; set; }

    public Test()
    {
    }

    public string Bbb { get; set; }
}
";

            string expectedCode = @"
public static class Program
{
    public int Aaa { get; set; }

    public string Bbb { get; set; }

    public Test()
    {
    }
}
";

            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongOrderVariablesRelativeToMethodsAnalyzer());
            ImmutableArray<CodeFixProvider> codeFixProviders = ImmutableArray.Create<CodeFixProvider>(new WrongOrderVariablesRelativeToMethodsCodeFixProvider());
            string newCode = await Utilities.ManageRunCodeFixTest(codeFixProviders, analyzers, testCode, expectedCode);
            Assert.AreEqual(expectedCode, newCode.ToString());
        }

        [Test]
        public async Task CreatingOneFieldAfterMethod_CodeFixMoveFieldBeforeMethod()
        {
            string testCode = @"
public static class Program
{
    public int Aaa { get; set; }

    public Test()
    {
    }

    private int _test;
}
";

            string expectedCode = @"
public static class Program
{
    public int Aaa { get; set; }

    private int _test;

    public Test()
    {
    }
}
";

            ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongOrderVariablesRelativeToMethodsAnalyzer());
            ImmutableArray<CodeFixProvider> codeFixProviders = ImmutableArray.Create<CodeFixProvider>(new WrongOrderVariablesRelativeToMethodsCodeFixProvider());
            string newCode = await Utilities.ManageRunCodeFixTest(codeFixProviders, analyzers, testCode, expectedCode);
            Assert.AreEqual(expectedCode, newCode.ToString());
        }

        [Test]
        public async Task CreatingOnePropertyBeforeMethod_CodeFixDoNothing()
        {
            string testCode = @"
public static class Program
{
    public int Aaa { get; set; }

    public Test()
    {
    }
}
";

            string expectedCode = @"
public static class Program
{
    public int Aaa { get; set; }

    public Test()
    {
    }
}
";

            string newCode = string.Empty;
            Exception ex = Assert.ThrowsAsync<Exception>(async () =>
            {
                ImmutableArray<DiagnosticAnalyzer> analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new WrongOrderVariablesRelativeToMethodsAnalyzer());
                ImmutableArray<CodeFixProvider> codeFixProviders = ImmutableArray.Create<CodeFixProvider>(new WrongOrderVariablesRelativeToMethodsCodeFixProvider());
                newCode = await Utilities.ManageRunCodeFixTest(codeFixProviders, analyzers, testCode, expectedCode);
            });

            Assert.That(ex.Message, Is.EqualTo("No diagnostic found"));
        }
    }
}
