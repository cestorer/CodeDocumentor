﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeDocumentor.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeDocumentor
{
    /// <summary>
    ///   The interface code fix provider.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnumCodeFixProvider)), Shared]
    public class EnumCodeFixProvider : CodeFixProvider
    {
        private const string title = "Code Documentor this enum";
        private const string titleRebuild = "Code Documentor update this enum";

        /// <summary>
        ///   Gets the fixable diagnostic ids.
        /// </summary>
        public override sealed ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(EnumAnalyzerSettings.DiagnosticId);

        /// <summary>
        ///   Gets fix all provider.
        /// </summary>
        /// <returns> A FixAllProvider. </returns>
        public override sealed FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        /// <summary>
        ///   Registers code fixes async.
        /// </summary>
        /// <param name="context"> The context. </param>
        /// <returns> A Task. </returns>
        public override sealed async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            Diagnostic diagnostic = context.Diagnostics.First();
            Microsoft.CodeAnalysis.Text.TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

            EnumDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<EnumDeclarationSyntax>().First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: declaration.HasSummary() ? titleRebuild : title,
                    createChangedDocument: c => AddDocumentationHeaderAsync(context.Document, root, declaration, c),
                    equivalenceKey: declaration.HasSummary() ? titleRebuild : title),
                diagnostic);
        }

        /// <summary>
        ///   Adds documentation header async.
        /// </summary>
        /// <param name="document"> The document. </param>
        /// <param name="root"> The root. </param>
        /// <param name="declarationSyntax"> The declaration syntax. </param>
        /// <param name="cancellationToken"> The cancellation token. </param>
        /// <returns> A Document. </returns>
        private async Task<Document> AddDocumentationHeaderAsync(Document document, SyntaxNode root, EnumDeclarationSyntax declarationSyntax, CancellationToken cancellationToken)
        {
            var newDeclaration = BuildNewDeclaration(declarationSyntax);
            SyntaxNode newRoot = root.ReplaceNode(declarationSyntax, newDeclaration);
            return document.WithSyntaxRoot(newRoot);
        }

        private static EnumDeclarationSyntax BuildNewDeclaration(EnumDeclarationSyntax declarationSyntax)
        {
            SyntaxTriviaList leadingTrivia = declarationSyntax.GetLeadingTrivia();

            string comment = CommentHelper.CreateEnumComment(declarationSyntax.Identifier.ValueText);
            DocumentationCommentTriviaSyntax commentTrivia = DocumentationHeaderHelper.CreateOnlySummaryDocumentationCommentTrivia(comment);
            EnumDeclarationSyntax newDeclaration = declarationSyntax.WithLeadingTrivia(leadingTrivia.UpsertLeadingTrivia(commentTrivia));
            return newDeclaration;
        }

        /// <summary>
        /// Builds the headers.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="nodesToReplace">The nodes to replace.</param>
        internal static int BuildComments(SyntaxNode root, Dictionary<CSharpSyntaxNode, CSharpSyntaxNode> nodesToReplace)
        {
            var declarations = root.DescendantNodes().Where(w => w.IsKind(SyntaxKind.EnumDeclaration)).OfType<EnumDeclarationSyntax>().ToArray();
            var neededCommentCount = 0;
            foreach (var declarationSyntax in declarations)
            {
                if (declarationSyntax.HasSummary())
                {
                    continue;
                }
                var newDeclaration = BuildNewDeclaration(declarationSyntax);
                nodesToReplace.TryAdd(declarationSyntax, newDeclaration);
                neededCommentCount++;
            }
            return neededCommentCount;
        }
    }
}
