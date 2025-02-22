﻿using System.Diagnostics.CodeAnalysis;
using CodeDocumentor.Test.TestHelpers;
using CodeDocumentor.Vsix2022;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

using Xunit;

namespace CodeDocumentor.Test.Enums
{
    /// <summary>
    /// The enum unit test.
    /// </summary>

    public class EnumUnitTest : CodeFixVerifier, IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public EnumUnitTest(TestFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Nos diagnostics show.
        /// </summary>
        /// <param name="testCode">The test code.</param>
        [Theory]
        [InlineData("")]
        public void NoDiagnosticsShow(string testCode)
        {
            VerifyCSharpDiagnostic(testCode);
        }

        /// <summary>
        /// Shows diagnostic and fix.
        /// </summary>
        /// <param name="testCode">The test code.</param>
        /// <param name="fixCode">The fix code.</param>
        /// <param name="line">The line.</param>
        /// <param name="column">The column.</param>
        [Theory]
        [InlineData("TestCode.cs", "TestFixCode.cs", 7, 17)]
        public void ShowDiagnosticAndFix(string testCode, string fixCode, int line, int column)
        {
            var fix = _fixture.LoadTestFile($@"./Enums/TestFiles/{fixCode}");
            var test = _fixture.LoadTestFile($@"./Enums/TestFiles/{testCode}");
            var expected = new DiagnosticResult
            {
                Id = EnumAnalyzerSettings.DiagnosticId,
                Message = EnumAnalyzerSettings.MessageFormat,
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", line, column)
                        }
            };

            VerifyCSharpDiagnostic(test, TestFixture.DIAG_TYPE_PUBLIC_ONLY, expected);

            VerifyCSharpFix(test, fix, TestFixture.DIAG_TYPE_PUBLIC_ONLY);
        }

        /// <summary>
        /// Gets c sharp code fix provider.
        /// </summary>
        /// <returns>A CodeFixProvider.</returns>
        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new EnumCodeFixProvider();
        }

        /// <summary>
        /// Gets c sharp diagnostic analyzer.
        /// </summary>
        /// <returns>A DiagnosticAnalyzer.</returns>
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer(string diagType)
        {
            return new EnumAnalyzer();
        }
    }
}
