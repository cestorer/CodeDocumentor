﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeDocumentor.Helper;
using CodeDocumentor.Services;
using CodeDocumentor.Vsix2022;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeDocumentor
{
    /// <summary>
    ///   The class code fix provider.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ClassCodeFixProvider)), Shared]
    public class ClassCodeFixProvider : CodeFixProvider
    {
        private const string title = "Code Documentor this class";

        private const string titleRebuild = "Code Documentor update this class";

        /// <summary>
        ///   Gets the fixable diagnostic ids.
        /// </summary>
        public override sealed ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ClassAnalyzerSettings.DiagnosticId);

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

            ClassDeclarationSyntax declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().First();
            var optionsService = CodeDocumentorPackage.DIContainer().GetInstance<IOptionsService>();

            if (optionsService.IsEnabledForPublicMembersOnly && PrivateMemberVerifier.IsPrivateMember(declaration))
            {
                return;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: declaration.HasSummary() ? titleRebuild : title,
                    createChangedDocument: c => AddDocumentationHeaderAsync(context.Document, root, declaration, c),
                    equivalenceKey: declaration.HasSummary() ? titleRebuild : title),
                diagnostic);
        }

        /// <summary>
        /// Builds the headers.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="nodesToReplace">The nodes to replace.</param>
        internal static int BuildComments(SyntaxNode root, Dictionary<CSharpSyntaxNode, CSharpSyntaxNode> nodesToReplace)
        {
            var optionsService = CodeDocumentorPackage.DIContainer().GetInstance<IOptionsService>();
            var declarations = root.DescendantNodes().Where(w => w.IsKind(SyntaxKind.ClassDeclaration)).OfType<ClassDeclarationSyntax>().ToArray();
            var neededCommentCount = 0;
            foreach (var declarationSyntax in declarations)
            {
                if (optionsService.IsEnabledForPublicMembersOnly
                    && PrivateMemberVerifier.IsPrivateMember(declarationSyntax))
                {
                    continue;
                }
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

        /// <summary>
        ///   Adds documentation header async.
        /// </summary>
        /// <param name="document"> The document. </param>
        /// <param name="root"> The root. </param>
        /// <param name="declarationSyntax"> The declaration syntax. </param>
        /// <param name="cancellationToken"> The cancellation token. </param>
        /// <returns> A Document. </returns>
        internal static async Task<Document> AddDocumentationHeaderAsync(Document document, SyntaxNode root, ClassDeclarationSyntax declarationSyntax, CancellationToken cancellationToken)
        {
            var newDeclaration = BuildNewDeclaration(declarationSyntax);
            SyntaxNode newRoot = root.ReplaceNode(declarationSyntax, newDeclaration);

            return document.WithSyntaxRoot(newRoot);
        }

        private static ClassDeclarationSyntax BuildNewDeclaration(ClassDeclarationSyntax declarationSyntax)
        {
            SyntaxList<SyntaxNode> list = SyntaxFactory.List<SyntaxNode>();

            string comment = CommentHelper.CreateClassComment(declarationSyntax.Identifier.ValueText);
            list = list.AddRange(DocumentationHeaderHelper.CreateSummaryPartNodes(comment));

            if (declarationSyntax?.TypeParameterList?.Parameters.Any() == true)
            {
                foreach (TypeParameterSyntax parameter in declarationSyntax.TypeParameterList.Parameters)
                {
                    list = list.AddRange(DocumentationHeaderHelper.CreateTypeParameterPartNodes(parameter.Identifier.ValueText));
                }
            }

            DocumentationCommentTriviaSyntax commentTrivia = SyntaxFactory.DocumentationCommentTrivia(SyntaxKind.SingleLineDocumentationCommentTrivia, list);

            //append to any existing leading trivia [attributes, decorators, etc)
            SyntaxTriviaList leadingTrivia = declarationSyntax.GetLeadingTrivia();

            ClassDeclarationSyntax newDeclaration = declarationSyntax.WithLeadingTrivia(leadingTrivia.UpsertLeadingTrivia(commentTrivia));
            return newDeclaration;
        }
    }
}
