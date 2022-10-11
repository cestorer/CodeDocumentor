﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Threading.Tasks;
using CodeDocumentor.Helper;
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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FileCodeFixProvider)), Shared]
    public class FileCodeFixProvider : CodeFixProvider
    {


        /// <summary>
        ///   The title.
        /// </summary>
        private const string title = "Code Documentor this whole file";

        /// <summary>
        ///   Gets the fixable diagnostic ids.
        /// </summary>
        public override sealed ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.CreateRange(new List<string> {
            FileAnalyzerSettings.DiagnosticId,
            ClassAnalyzerSettings.DiagnosticId
        });

        /// <summary>
        ///   Gets fix all provider.
        /// </summary>
        /// <returns> A FixAllProvider. </returns>
        public override sealed FixAllProvider GetFixAllProvider()
        {
            return null;
            //return FixAllProvider.Create(async (context, doc, codes) =>
            //{
            //    if (context.Document != null)
            //    {
            //        Document d = context.Document;
            //        SyntaxNode root = await doc.GetSyntaxRootAsync(context.CancellationToken);
            //        ClassCodeFixProvider.BuildHeaders(root, _nodesToReplace);


            //        root = root.ReplaceNodes(_nodesToReplace.Keys, (n1, n2) =>
            //        {
            //            return _nodesToReplace[n1];
            //        });
            //        return d.WithSyntaxRoot(root);
            //    }

            //    //SyntaxNode root1 = await d.GetSyntaxRootAsync(context.CancellationToken);
            //    //return d.WithSyntaxRoot(root1);
            //});
        }

        /// <summary>
        ///   Registers code fixes async.
        /// </summary>
        /// <param name="context"> The context. </param>
        /// <returns> A Task. </returns>
        public override sealed async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            Diagnostic diagnostic = context.Diagnostics.First();

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: async (c) =>
                    {
                        var _nodesToReplace = new Dictionary<CSharpSyntaxNode, CSharpSyntaxNode>();
                        Document d = context.Document;
                        SyntaxNode root = await d.GetSyntaxRootAsync(context.CancellationToken);
                        ClassCodeFixProvider.BuildHeaders(root, _nodesToReplace);

                        root = root.ReplaceNodes(_nodesToReplace.Keys, (n1, n2) =>
                        {
                            return _nodesToReplace[n1];
                        });
                        return d.WithSyntaxRoot(root);
                    },
                    equivalenceKey: title),
                diagnostic);


        }

    }
}
