using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FmodAudioSourceGenerator
{
    internal class UsingDirectiveManager
    {
        private static readonly UsingDirectiveComparer comparer = new UsingDirectiveComparer();

        private readonly HashSet<UsingDirectiveSyntax> set = new HashSet<UsingDirectiveSyntax>(comparer);

        public void AddUsingsFromTree(MemberDeclarationSyntax node)
        {
            SyntaxNode parentNode = node.Parent;

            do
            {
                if (parentNode is NamespaceDeclarationSyntax namespaceDecl)
                {
                    AddUsings(namespaceDecl.Usings);
                }
                else if (parentNode is CompilationUnitSyntax compilationUnit)
                {
                    AddUsings(compilationUnit.Usings);
                }

                parentNode = parentNode.Parent;
            }
            while (parentNode != null);
        }

        public void AddUsing(UsingDirectiveSyntax directive)
        {
            set.Add(directive.NormalizeWhitespace());
        }

        public void AddUsings<T>(T directives) where T: IEnumerable<UsingDirectiveSyntax>
        {
            foreach (var directive in directives)
            {
                AddUsing(directive);
            }
        }

        public IList<UsingDirectiveSyntax> SortAndReturnUsingDirectives()
        {
            var list = new List<UsingDirectiveSyntax>(set);

            list.Sort(comparer);

            return list;
        }

        private sealed class UsingDirectiveComparer : IEqualityComparer<UsingDirectiveSyntax>, IComparer<UsingDirectiveSyntax>
        {
            public bool Equals(UsingDirectiveSyntax x, UsingDirectiveSyntax y)
            {
                return x.IsEquivalentTo(y);
            }

            public int GetHashCode(UsingDirectiveSyntax obj)
            {
                //Seed and Factor for hashing given by `Special Sauce` on StackOverflow
                //https://stackoverflow.com/a/34006336/4330090
                PoorMansHashCode hash = new PoorMansHashCode(1009, 9176);

                if (obj.Alias != null)
                {
                    hash.Add(obj.Alias.Name.Identifier.Text);
                }

                var name = obj.Name;

                while (true)
                {
                    if (name is QualifiedNameSyntax qname)
                    {
                        hash.Add(qname.Right.Identifier.Text);
                        name = qname.Left;
                    }
                    else if (name is SimpleNameSyntax sname)
                    {
                        hash.Add(sname.Identifier.Text);
                        break;
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }

                return hash.ToHashCode();
            }

            public int Compare(UsingDirectiveSyntax x, UsingDirectiveSyntax y)
            {
                if (ReferenceEquals(x, y))
                    return 0;

                bool t0, t1;
                t0 = x.Alias != null;
                t1 = y.Alias != null;

                if (t0 == t1)
                {
                    if (t0)
                    {
                        var comp = x.Alias.Name.Identifier.Text.CompareTo(y.Alias.Name.Identifier.Text);

                        if (comp != 0)
                            return comp;
                    }
                }
                else
                {
                    if (t0)
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }

                return CompareNameSyntax(x.Name, y.Name);
            }

            private static int CompareNameSyntax(NameSyntax x, NameSyntax y)
            {
                var xPartList = NamePartList(x);
                var yPartList = NamePartList(y);

                int min = Math.Min(xPartList.Length, yPartList.Length);

                for (int i = 0; i < min; ++i)
                {
                    int tmp = xPartList[i].Identifier.Text.CompareTo(yPartList[i].Identifier.Text);

                    if (tmp != 0)
                        return tmp;
                }

                return xPartList.Length.CompareTo(yPartList.Length);

                SimpleNameSyntax[] NamePartList(NameSyntax name)
                {
                    var sname = name as SimpleNameSyntax;

                    if (sname != null)
                    {
                        return new[] { sname };
                    }

                    var qname = (QualifiedNameSyntax)name;

                    List<SimpleNameSyntax> parts = new List<SimpleNameSyntax>();

                    while (true)
                    {
                        parts.Add(qname.Right);

                        sname = qname.Left as SimpleNameSyntax;

                        if (sname != null)
                        {
                            parts.Add(sname);
                            break;
                        }

                        qname = (QualifiedNameSyntax)qname.Left;
                    }

                    parts.Reverse();

                    return parts.ToArray();
                }
            }

            private struct PoorMansHashCode
            {
                private int HashCode;
                private readonly int Factor;
                
                public PoorMansHashCode(int seed, int factor)
                {
                    HashCode = seed;
                    Factor = factor;
                }

                public void Add<T>(T obj)
                {
                    HashCode = unchecked(HashCode * Factor + (obj == null ? 0 : obj.GetHashCode()));
                }

                public int ToHashCode()
                {
                    return HashCode;
                }
            }
        }
    }
}
