using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FmodAudioSourceGenerator
{
    public class SyntaxReceiver : ISyntaxReceiver
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

            if (syntaxNode is ClassDeclarationSyntax classDecl)
            {
                if (classDecl.Modifiers.Any(SyntaxKind.PartialKeyword) && HasAttributeOfName(classDecl, "VTable"))
                {
                    VTableCandidates.Add(classDecl);
                }

                return;
            }

            if (syntaxNode is StructDeclarationSyntax structDecl)
            {
                if (HasAttributeOfName(structDecl, "WrapperType"))
                {
                    WrapperTypes.Add(structDecl);
                }

                return;
            }
        }

        private static bool HasAttributeOfName(MemberDeclarationSyntax decl, string stringName)
        {
            var lists = decl.AttributeLists;

            if (lists.Count > 0)
            {
                foreach (var list in lists)
                {
                    foreach (var attrib in list.Attributes)
                    {
                        var name = attrib.Name;

                        if (!(name is SimpleNameSyntax sname))
                        {
                            if (name is QualifiedNameSyntax qname)
                                sname = qname.Right;
                            else
                                continue;
                        }

                        if (sname.Identifier.Text == stringName)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
