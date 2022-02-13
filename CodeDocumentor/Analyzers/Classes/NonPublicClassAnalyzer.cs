﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CodeDocumentor.Helper;
using CodeDocumentor.Vsix2022;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeDocumentor
{
    /// <summary>
    ///   The class analyzer.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NonPublicClassAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        ///   Gets the supported diagnostics.
        /// </summary>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (CodeDocumentorPackage.Options?.IsEnabledForPublishMembersOnly == true)
                {
                    return new List<DiagnosticDescriptor>().ToImmutableArray();
                }
                return ImmutableArray.Create(ClassAnalyzerSettings.GetRule());
            }
        }

        /// <summary>
        ///   Initializes action.
        /// </summary>
        /// <param name="context"> The context. </param>
        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ClassDeclaration);
        }

        /// <summary>
        ///   Analyzes node.
        /// </summary>
        /// <param name="context"> The context. </param>
        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            ClassDeclarationSyntax node = context.Node as ClassDeclarationSyntax;
            if (!PrivateMemberVerifier.IsPrivateMember(node))
            {
                return;
            }

            DocumentationCommentTriviaSyntax commentTriviaSyntax = node
                .GetLeadingTrivia()
                .Select(o => o.GetStructure())
                .OfType<DocumentationCommentTriviaSyntax>()
                .FirstOrDefault();

            if (CodeDocumentorPackage.Options?.IsEnabledForPublishMembersOnly == true)
            {
                return;
            }

            var excludeAnanlyzer = DocumentationHeaderHelper.HasAnalyzerExclusion(node);
            if (excludeAnanlyzer)
            {
                return;
            }
            if (commentTriviaSyntax != null && CommentHelper.HasComment(commentTriviaSyntax))
            {
                return;
            }
            context.ReportDiagnostic(Diagnostic.Create(ClassAnalyzerSettings.GetRule(), node.Identifier.GetLocation()));
        }
    }
}
