using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FmodAudioSourceGenerator
{
    public class VTableSyntaxReceiver : ISyntaxReceiver
    {
        public readonly List<ClassDeclarationSyntax> VTableCandidates = new List<ClassDeclarationSyntax>();

        public readonly List<StructDeclarationSyntax> WrapperTypes = new List<StructDeclarationSyntax>(8);

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            var parent = syntaxNode.Parent;

            if (!(parent is NamespaceDeclarationSyntax || parent is CompilationUnitSyntax))
            {
                return;
            }

            if (syntaxNode is TypeDeclarationSyntax typeDecl)
            {
                bool isPartial = typeDecl.Modifiers.Any(SyntaxKind.PartialKeyword);

                if (typeDecl is ClassDeclarationSyntax classDecl)
                {
                    if (isPartial && Helpers.HasAttributeOfName(classDecl, "VTable"))
                    {
                        VTableCandidates.Add(classDecl);
                    }

                    return;
                }

                if (typeDecl is StructDeclarationSyntax structDecl)
                {
                    if (Helpers.HasAttributeOfName(structDecl, "WrapperType"))
                    {
                        WrapperTypes.Add(structDecl);
                    }

                    return;
                }
            }
        }


    }
}
