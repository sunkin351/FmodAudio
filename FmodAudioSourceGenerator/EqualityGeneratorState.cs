using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace FmodAudioSourceGenerator
{
    internal class EqualityGeneratorState : GeneratorState
    {
        private List<TypeDeclarationSyntax> TypeList;

        private List<MemberDeclarationSyntax> typeMembers = new List<MemberDeclarationSyntax>();

        private INamedTypeSymbol EqualityAttribute;

        public EqualityGeneratorState(in GeneratorExecutionContext context, EqualitySyntaxReciever receiver) : base(in context)
        {
            TypeList = receiver.TypeDeclarations;

            EqualityAttribute = Compilation.GetTypeByMetadataName("FmodAudio.EqualityBoilerplateAttribute");
        }

        protected override IEnumerable<(string FileNameHint, string SourceText)> AttributeSources
        {
            get
            {
                const string EqualityAttributeSource =
@"using System;

namespace FmodAudio
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    internal class EqualityBoilerplateAttribute : Attribute
    {
    }
}";

                return new[] { ("EqualityBoilerplateAttribute", EqualityAttributeSource) };
            }
        }

        public override void GenerateSources()
        {
            var boolSymbol = Compilation.GetTypeByMetadataName("System.Boolean");
            var boolType = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword));

            var equalsInvocationExpression_binaryOperator = SyntaxFactory.ParseExpression("left.Equals(right)");
            var equalsInvocationExpression_method = SyntaxFactory.ParseExpression("this.Equals(other)");

            foreach (var type in TypeList)
            {
                var model = GetSemanticModel(type.SyntaxTree);

                var symbol = model.GetDeclaredSymbol(type);

                if (!CheckTypeQualifications(type, symbol))
                    continue;

                var equalsMethod = symbol.GetMembers("Equals")
                    .FirstOrDefault(
                        s => s is IMethodSymbol method
                            && method.ReturnType.Equals(boolSymbol)
                            && method.Parameters.Length == 1
                            && method.Parameters[0].Type.Equals(symbol));

                if (equalsMethod is null)
                    continue;



                var typename = SyntaxFactory.IdentifierName(type.Identifier);

                var doesEqualOperator = SyntaxFactory.OperatorDeclaration(default,
                    SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
                    SyntaxFactory.Token(SyntaxKind.OperatorKeyword),
                    SyntaxFactory.Token(SyntaxKind.EqualsEqualsToken),
                    SyntaxFactory.ParameterList(
                        SyntaxFactory.SeparatedList(
                            new[] {
                                SyntaxFactory.Parameter(default, default, typename, SyntaxFactory.Identifier("left"), null),
                                SyntaxFactory.Parameter(default, default, typename, SyntaxFactory.Identifier("right"), null)
                            }
                        )
                    ),
                    null,
                    SyntaxFactory.ArrowExpressionClause(equalsInvocationExpression_binaryOperator),
                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                );

                var doesNotEqualOperator = doesEqualOperator.WithOperatorToken(SyntaxFactory.Token(SyntaxKind.ExclamationEqualsToken))
                    .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, equalsInvocationExpression_binaryOperator)));

                var equalsOverride = SyntaxFactory.MethodDeclaration(default,
                    SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.OverrideKeyword)),
                    boolType,
                    null, SyntaxFactory.Identifier("Equals"),
                    null, SyntaxFactory.ParameterList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Parameter(
                                default,
                                default,
                                SyntaxFactory.NullableType(
                                    SyntaxFactory.PredefinedType(
                                        SyntaxFactory.Token(SyntaxKind.ObjectKeyword)
                                    )
                                ),
                                SyntaxFactory.Identifier("obj"),
                                null
                            )
                        )
                    ),
                    default,
                    SyntaxFactory.Block(
                        SyntaxFactory.ReturnStatement(
                            SyntaxFactory.BinaryExpression(
                                SyntaxKind.LogicalAndExpression,
                                SyntaxFactory.IsPatternExpression(
                                    SyntaxFactory.IdentifierName("obj"),
                                    SyntaxFactory.DeclarationPattern(
                                        typename,
                                        SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("other"))
                                    )
                                ),
                                equalsInvocationExpression_method
                            )
                        )
                    ),
                    default(SyntaxToken)
                );

                typeMembers.Add(doesEqualOperator);
                typeMembers.Add(doesNotEqualOperator);
                typeMembers.Add(equalsOverride);

                TypeDeclarationSyntax newDecl;

                if (type is ClassDeclarationSyntax classDecl)
                {
                    newDecl = SyntaxFactory.ClassDeclaration(default, classDecl.Modifiers, classDecl.Identifier, classDecl.TypeParameterList, null, default, SyntaxFactory.List(typeMembers));
                }
                else if (type is StructDeclarationSyntax structDecl)
                {
                    newDecl = SyntaxFactory.StructDeclaration(default, structDecl.Modifiers, structDecl.Identifier, structDecl.TypeParameterList, null, default, SyntaxFactory.List(typeMembers));
                }
                else
                {
                    throw new InvalidOperationException("Unexpected declaration type");
                }

                var CUNode = SyntaxFactory.CompilationUnit(
                    default,
                    default,
                    default,
                    SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                        SyntaxFactory.NamespaceDeclaration(
                            SyntaxFactory.ParseName(symbol.ContainingNamespace.ToString()),
                            default, default,
                            SyntaxFactory.SingletonList<MemberDeclarationSyntax>(newDecl)
                        )
                    )
                ).NormalizeWhitespace();

                typeMembers.Clear();

                var text = SourceText.From(CUNode.ToFullString(), Encoding.UTF8);

                Context.AddSource(symbol.Name + "_Equality", text);
            }
        }

        private bool CheckTypeQualifications(TypeDeclarationSyntax typeDecl, INamedTypeSymbol symbol)
        {
            if (symbol.GetAttributes().FirstOrDefault(attrib => attrib.AttributeClass.Equals(EqualityAttribute, SymbolEqualityComparer.Default)) == null)
            {
                return false;
            }

            if (!(typeDecl is ClassDeclarationSyntax || typeDecl is StructDeclarationSyntax))
            {
                Context.ReportDiagnostic(Diagnostic.Create(EqualitySourceGenerator.FASG09, typeDecl.Identifier.GetLocation()));
                return false;
            }

            if (!(typeDecl.Parent is NamespaceDeclarationSyntax || typeDecl.Parent is CompilationUnitSyntax))
            {
                Context.ReportDiagnostic(Diagnostic.Create(EqualitySourceGenerator.FASG10, typeDecl.Identifier.GetLocation()));
                return false;
            }

            if (!typeDecl.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                Context.ReportDiagnostic(Diagnostic.Create(EqualitySourceGenerator.FASG11, typeDecl.Identifier.GetLocation()));
                return false;
            }

            if (!FindInterface(symbol))
            {
                Context.ReportDiagnostic(Diagnostic.Create(EqualitySourceGenerator.FASG12, typeDecl.Identifier.GetLocation(), typeDecl.Identifier.ValueText));
                return false;
            }

            return true;
        }

        private bool FindInterface(INamedTypeSymbol symbol)
        {
            var iequatable = Compilation.GetTypeByMetadataName("System.IEquatable`1");

            if (iequatable is null)
                return false;

            return symbol.Interfaces.Any(
                i =>
                {
                    return i.OriginalDefinition.Equals(iequatable, SymbolEqualityComparer.Default)
                        && i.TypeArguments[0].Equals(symbol, SymbolEqualityComparer.Default);
                });
        }
    }
}
