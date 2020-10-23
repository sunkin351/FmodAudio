using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FmodAudioSourceGenerator
{
    internal static class Helpers
    {
        public static bool HasAttributeOfName(MemberDeclarationSyntax decl, string stringName)
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
