﻿using System.Diagnostics.CodeAnalysis;
using CodeDocumentor.Helper;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace CodeDocumentor.Test.Helper
{
    [SuppressMessage("XMLDocumentation", "")]
    public class ReturnCommentConstructionTests : IClassFixture<TestFixture>
    {
        private ReturnCommentConstruction _returnCommentBuilder;

        public ReturnCommentConstructionTests(TestFixture testFixure)
        {
            _returnCommentBuilder = new ReturnCommentConstruction();
        }

        #region ReadOnlyCollection

        [Fact]
        public void GenerateGenericTypeComment_CreatesValidStringFromIReadOnlyCollection()
        {
            var roc = TestFixture.BuildGenericNameSyntax("IReadOnlyCollection", SyntaxKind.StringKeyword);

            var comment = _returnCommentBuilder.BuildComment(roc, false);
            comment.Should().Be("A read only collection of strings.");
        }

        [Fact]
        public void GenerateGenericTypeComment_CreatesValidStringFromIReadOnlyCollectionOfList()
        {
            var list = TestFixture.BuildGenericNameSyntax("List", SyntaxKind.StringKeyword);

            var roc = TestFixture.BuildGenericNameSyntax("IReadOnlyCollection", list);

            var comment = _returnCommentBuilder.BuildComment(roc, false);
            comment.Should().Be("A read only collection of list of strings.");
        }

        [Fact]
        public void GenerateGenericTypeComment_CreatesValidStringFromIReadOnlyCollectionOfReadOnlyCollection()
        {
            var list = TestFixture.BuildGenericNameSyntax("IReadOnlyCollection", SyntaxKind.StringKeyword);

            var roc = TestFixture.BuildGenericNameSyntax("IReadOnlyCollection", list);

            var comment = _returnCommentBuilder.BuildComment(roc, false);
            comment.Should().Be("A read only collection of read only collections of strings.");
        }

        #endregion

        #region List

        [Fact]
        public void GenerateGenericTypeComment_CreatesValidStringFromList()
        {
            var roc = TestFixture.BuildGenericNameSyntax("List", SyntaxKind.StringKeyword);

            var comment = _returnCommentBuilder.BuildComment(roc, false);
            comment.Should().Be("A list of strings.");
        }

        [Fact]
        public void GenerateGenericTypeComment_CreatesValidStringFromListOfList()
        {
            var list = TestFixture.BuildGenericNameSyntax("List", SyntaxKind.StringKeyword);

            var roc = TestFixture.BuildGenericNameSyntax("List", list);

            var comment = _returnCommentBuilder.BuildComment(roc, false);
            comment.Should().Be("A list of list of strings.");
        }

        [Fact]
        public void GenerateGenericTypeComment_CreatesValidStringFromListOfListOfList()
        {
            var list = TestFixture.BuildGenericNameSyntax("List", SyntaxKind.StringKeyword);

            var list2 = TestFixture.BuildGenericNameSyntax("List", list);

            var roc = TestFixture.BuildGenericNameSyntax("List", list2);

            var comment = _returnCommentBuilder.BuildComment(roc, false);
            comment.Should().Be("A list of list of list of strings.");
        }

        [Fact]
        public void GenerateGenericTypeComment_CreatesValidStringFromIList()
        {
            var roc = TestFixture.BuildGenericNameSyntax("IList", SyntaxKind.StringKeyword);

            var comment = _returnCommentBuilder.BuildComment(roc, false);
            comment.Should().Be("A list of strings.");
        }

        [Fact]
        public void GenerateGenericTypeComment_CreatesValidStringFromIListOfIList()
        {
            var list = TestFixture.BuildGenericNameSyntax("IList", SyntaxKind.StringKeyword);
            var roc = TestFixture.BuildGenericNameSyntax("IList", list);

            var comment = _returnCommentBuilder.BuildComment(roc, false);
            comment.Should().Be("A list of list of strings.");
        }

        [Fact]
        public void GenerateGenericTypeComment_CreatesValidStringFromListOfInt()
        {
            var roc = TestFixture.BuildGenericNameSyntax("List", SyntaxKind.IntKeyword);

            var comment = _returnCommentBuilder.BuildComment(roc, false);
            comment.Should().Be("A list of integers.");
        }

        [Fact]
        public void GenerateGenericTypeComment_CreatesValidStringFromListOfListOfInt()
        {
            var list = TestFixture.BuildGenericNameSyntax("List", SyntaxKind.IntKeyword);
            var roc = TestFixture.BuildGenericNameSyntax("IList", list);
            var comment = _returnCommentBuilder.BuildComment(roc, false);
            comment.Should().Be("A list of list of integers.");
        }

        #endregion

        #region IEnumerable

        [Fact]
        public void GenerateGenericTypeComment_CreatesValidStringFromIEnumerable()
        {
            var roc = TestFixture.BuildGenericNameSyntax("IEnumerable", SyntaxKind.StringKeyword);

            var comment = _returnCommentBuilder.BuildComment(roc, false);
            comment.Should().Be("A list of strings.");
        }

        [Fact]
        public void GenerateGenericTypeComment_CreatesValidStringFromIEnumerableOfIEnumerable()
        {
            var list = TestFixture.BuildGenericNameSyntax("IEnumerable", SyntaxKind.StringKeyword);
            var roc = TestFixture.BuildGenericNameSyntax("IEnumerable", list);

            var comment = _returnCommentBuilder.BuildComment(roc, false);
            comment.Should().Be("A list of list of strings.");
        }

        #endregion

        #region ICollection

        [Fact]
        public void GenerateGenericTypeComment_CreatesValidStringFromICollection()
        {
            var roc = TestFixture.BuildGenericNameSyntax("ICollection", SyntaxKind.StringKeyword);

            var comment = _returnCommentBuilder.BuildComment(roc, false);
            comment.Should().Be("A list of strings.");
        }

        [Fact]
        public void GenerateGenericTypeComment_CreatesValidStringFromCollection()
        {
            var roc = TestFixture.BuildGenericNameSyntax("Collection", SyntaxKind.StringKeyword);

            var comment = _returnCommentBuilder.BuildComment(roc, false);
            comment.Should().Be("A list of strings.");
        }

        #endregion

        #region Dictionary

        [Fact]
        public void GenerateGenericTypeComment_CreatesValidStringFromIDictionary()
        {
            var roc = TestFixture.BuildGenericNameSyntax("IDictionary", SyntaxKind.StringKeyword, SyntaxKind.StringKeyword);

            var comment = _returnCommentBuilder.BuildComment(roc, false);
            comment.Should().Be("A dictionary with a key of type string and a value of type string.");
        }

        [Fact]
        public void GenerateGenericTypeComment_CreatesValidStringFromIDictionaryOfInt()
        {
            var roc = TestFixture.BuildGenericNameSyntax("IDictionary", SyntaxKind.IntKeyword, SyntaxKind.IntKeyword);

            var comment = _returnCommentBuilder.BuildComment(roc, false);
            comment.Should().Be("A dictionary with a key of type integer and a value of type integer.");
        }

        [Fact]
        public void GenerateGenericTypeComment_CreatesValidStringFromDictionary()
        {
            var roc = TestFixture.BuildGenericNameSyntax("Dictionary", SyntaxKind.StringKeyword, SyntaxKind.StringKeyword);

            var comment = _returnCommentBuilder.BuildComment(roc, false);
            comment.Should().Be("A dictionary with a key of type string and a value of type string.");
        }

        [Fact]
        public void GenerateGenericTypeComment_CreatesValidStringFromDictionaryWithListValue()
        {
            var list = TestFixture.BuildGenericNameSyntax("IEnumerable", SyntaxKind.StringKeyword);
            var roc = TestFixture.BuildGenericNameSyntax("Dictionary", SyntaxKind.StringKeyword, list);

            var comment = _returnCommentBuilder.BuildComment(roc, false);
            comment.Should().Be("A dictionary with a key of type string and a value of type list of strings.");
        }

        [Fact]
        public void GenerateGenericTypeComment_CreatesValidStringFromDictionaryWithListOfListValue()
        {
            var list = TestFixture.BuildGenericNameSyntax("IList", SyntaxKind.StringKeyword);
            var list2 = TestFixture.BuildGenericNameSyntax("List", list);
            var roc = TestFixture.BuildGenericNameSyntax("Dictionary", SyntaxKind.StringKeyword, list2);

            var comment = _returnCommentBuilder.BuildComment(roc, false);
            comment.Should().Be("A dictionary with a key of type string and a value of type list of list of strings.");
        }

        #endregion

        #region Task

        [Fact]
        public void GenerateGenericTypeComment_CreatesValidStringFromTaskOfString()
        {
            var roc = TestFixture.BuildGenericNameSyntax("Task", SyntaxKind.StringKeyword);

            var comment = _returnCommentBuilder.BuildComment(roc, false);
            comment.Should().Be("A string.");
        }

        [Fact]
        public void GenerateGenericTypeComment_CreatesValidStringFromTaskOfList()
        {
            var list = TestFixture.BuildGenericNameSyntax("IList", SyntaxKind.StringKeyword);
            var roc = TestFixture.BuildGenericNameSyntax("Task", list);

            var comment = _returnCommentBuilder.BuildComment(roc, false);
            comment.Should().Be("A list of strings.");
        }

        [Fact]
        public void GenerateGenericTypeComment_CreatesValidStringFromTaskOfDictionary()
        {
            var list = TestFixture.BuildGenericNameSyntax("IEnumerable", SyntaxKind.StringKeyword);
            var dict = TestFixture.BuildGenericNameSyntax("Dictionary", SyntaxKind.StringKeyword, list);
            var roc = TestFixture.BuildGenericNameSyntax("Task", dict);

            var comment = _returnCommentBuilder.BuildComment(roc, false);
            comment.Should().Be("A dictionary with a key of type string and a value of type list of strings.");
        }

        [Fact]
        public void GenerateGenericTypeComment_CreatesValidStringFromTaskOfCustom()
        {
            var custom = TestFixture.BuildGenericNameSyntax("CustomClass", SyntaxKind.StringKeyword, SyntaxKind.StringKeyword);
            var roc = TestFixture.BuildGenericNameSyntax("Task", custom);

            var comment = _returnCommentBuilder.BuildComment(roc, false);
            comment.Should().Be("A CustomClass.");
        }

        #endregion

        #region Unknown

        public class CustomClass<TIn, TOut>
        {
            public TIn InProp { get; set; }

            public TOut MyProperty { get; set; }
        }

        [Fact]
        public void GenerateGenericTypeComment_CreatesValidStringFromUnknown()
        {
            var roc = TestFixture.BuildGenericNameSyntax("Span", SyntaxKind.StringKeyword);

            var comment = _returnCommentBuilder.BuildComment(roc, false);
            comment.Should().Be("A Span.");
        }

        [Fact]
        public void GenerateGenericTypeComment_CreatesValidStringFromUnknownGeneric()
        {
            var roc = TestFixture.BuildGenericNameSyntax("CustomClass", SyntaxKind.StringKeyword, SyntaxKind.StringKeyword);

            var comment = _returnCommentBuilder.BuildComment(roc, false);
            comment.Should().Be("A CustomClass.");
        }

        #endregion

        #region IdentifierNameSyntax

        [Fact]
        public void IdentifierNameSyntaxComment_CreatesValidTypeParamRef()
        {
            var roc = TestFixture.BuildMethodDeclarationSyntax("CustomClass", "TestMethod");
            var returnType = TestFixture.GetReturnType(roc);

            returnType.Should().NotBeNull();
            var comment = _returnCommentBuilder.BuildComment(returnType, false);
            comment.Should().Be("<typeparamref name=\"CustomClass\"></typeparamref>");
        }

        #endregion
    }
}
