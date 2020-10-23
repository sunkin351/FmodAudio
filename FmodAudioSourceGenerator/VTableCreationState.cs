using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace FmodAudioSourceGenerator
{
    internal class VTableCreationState : GeneratorState
    {
        private static readonly MemberAccessExpressionSyntax NativeLibrary_GetExport = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName("NativeLibrary"), SyntaxFactory.IdentifierName("GetExport"));
        private static readonly QualifiedNameSyntax NamespaceName = SyntaxFactory.QualifiedName(SyntaxFactory.IdentifierName("FmodAudio"), SyntaxFactory.IdentifierName("Base"));
        private static readonly SyntaxTokenList FieldDeclarationModifiers = SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
        private static readonly IdentifierNameSyntax LibParamName = SyntaxFactory.IdentifierName("libHandle");
        private static readonly UsingDirectiveSyntax[] RequiredUsings = new UsingDirectiveSyntax[]
        {
            SyntaxFactory.UsingDirective(
                SyntaxFactory.QualifiedName(
                    SyntaxFactory.QualifiedName(
                        SyntaxFactory.IdentifierName("System"),
                        SyntaxFactory.IdentifierName("Runtime")
                    ),
                    SyntaxFactory.IdentifierName("InteropServices")
                )
            )
        };

        internal readonly VTableSyntaxReceiver Receiver;

        private readonly INamedTypeSymbol VTableAttributeClass, WrapperTypeAttributeClass, InteropMethodAttributeClass;

        public VTableCreationState(in GeneratorExecutionContext context, VTableSyntaxReceiver receiver) : base(in context)
        {
            Receiver = receiver;

            VTableAttributeClass = this.Compilation.GetTypeByMetadataName("FmodAudio.VTableAttribute");

            WrapperTypeAttributeClass = this.Compilation.GetTypeByMetadataName("FmodAudio.WrapperTypeAttribute");

            InteropMethodAttributeClass = this.Compilation.GetTypeByMetadataName("FmodAudio.InteropMethodAttribute");
        }

        protected override IEnumerable<(string FileNameHint, string SourceText)> AttributeSources
        {
            get
            {
                const string VTableAttrib =
@"using System;

namespace FmodAudio
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class VTableAttribute : Attribute
    {
    }
}
";

                const string WrapperTypeAttribute =
@"using System;

namespace FmodAudio
{
    [AttributeUsage(AttributeTargets.Struct)]
    internal class WrapperTypeAttribute : Attribute
    {
    }
}
";

                const string InteropMethodAttribute =
@"using System;

namespace FmodAudio
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class InteropMethodAttribute : Attribute
    {
    }
}
";

                return new[]
                {
                    ("VTableAttribute", VTableAttrib),
                    ("WrapperTypeAttribute", WrapperTypeAttribute),
                    ("InteropMethodAttribute", InteropMethodAttribute)
                };
            }
        }

        public override void GenerateSources()
        {
            var marshalContext = new MethodMarshallingContext(this, Receiver.WrapperTypes);

            foreach (var vTable in Receiver.VTableCandidates)
            {
                var model = GetSemanticModel(vTable.SyntaxTree);

                var symbol = model.GetDeclaredSymbol(vTable);

                var source = ProcessVTableType(marshalContext, symbol);

                if (Context.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (source != null)
                {
                    Context.AddSource(symbol.Name + "_Generated", source);
                }
            }
        }

        private SourceText ProcessVTableType(MethodMarshallingContext context, INamedTypeSymbol typeSymbol)
        {
            if (!typeSymbol.GetAttributes().Any(attrib => attrib.AttributeClass.Equals(VTableAttributeClass, SymbolEqualityComparer.Default)))
                return null;

            var classSyntax = (ClassDeclarationSyntax)typeSymbol.DeclaringSyntaxReferences[0].GetSyntax();

            Debug.Assert(classSyntax.Modifiers.Any(SyntaxKind.PartialKeyword));

            List<StatementSyntax> ConstructorStatements = new List<StatementSyntax>();
            List<MemberDeclarationSyntax> ClassMembers = new List<MemberDeclarationSyntax>();

            foreach (var member in typeSymbol.GetMembers())
            {
                if (Context.CancellationToken.IsCancellationRequested)
                    return null;

                if (!(member is IMethodSymbol method))
                    continue;

                if (!IsTargetMethod(method))
                    continue;

                if (LookForParameterProblems(method))
                    continue;

                //Begin of syntax tree generation
                var funcPtrType = context.GenerateFunctionPointerType(method);
                var fieldName = method.Name + "_Func";

                var fieldDeclaration = SyntaxFactory.FieldDeclaration(
                    default,
                    FieldDeclarationModifiers,
                    SyntaxFactory.VariableDeclaration(funcPtrType,
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(fieldName)
                        )
                    )
                );

                ClassMembers.Add(fieldDeclaration);

                var invoke = SyntaxFactory.InvocationExpression(
                    NativeLibrary_GetExport,
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SeparatedList(
                            new[]
                            {
                                SyntaxFactory.Argument(LibParamName),
                                SyntaxFactory.Argument(
                                    SyntaxFactory.LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        SyntaxFactory.Literal("FMOD_" + method.Name)
                                    )
                                )
                            }
                        )
                    )
                );

                var assignment = SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.IdentifierName(fieldName),
                    SyntaxFactory.CastExpression(funcPtrType, invoke)
                );

                ConstructorStatements.Add(SyntaxFactory.ExpressionStatement(assignment));

                ClassMembers.Add(
                    context.ImplementMethod(method, fieldName)
                );
            }

            if (Context.CancellationToken.IsCancellationRequested)
                return null;

            var constructor = SyntaxFactory.ConstructorDeclaration(
                default,
                SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.InternalKeyword)),
                classSyntax.Identifier,
                SyntaxFactory.ParameterList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Parameter(default, default, SyntaxFactory.IdentifierName("IntPtr"), LibParamName.Identifier, null)
                    )
                ),
                null,
                SyntaxFactory.Block(ConstructorStatements)
            );

            ClassMembers.Add(constructor);

            var newDefinition = CreateNewDefinition(classSyntax, ClassMembers);

            ClassMembers.Clear();
            ConstructorStatements.Clear();

            var namespaceSyntax = SyntaxFactory.NamespaceDeclaration(
                NamespaceName,
                default, default,
                SyntaxFactory.SingletonList<MemberDeclarationSyntax>(newDefinition)
            );

            UsingDirectiveManager usingsManager = new UsingDirectiveManager();

            usingsManager.AddUsingsFromTree(classSyntax);
            usingsManager.AddUsings(RequiredUsings);

            var compileUnit = SyntaxFactory.CompilationUnit(
                default, SyntaxFactory.List(usingsManager.SortAndReturnUsingDirectives()),
                default, SyntaxFactory.SingletonList<MemberDeclarationSyntax>(namespaceSyntax)
            ).NormalizeWhitespace();

            return SourceText.From(compileUnit.ToFullString(), Encoding.UTF8);
        }

        private bool IsTargetMethod(IMethodSymbol method)
        {
            if (method.MethodKind != MethodKind.Ordinary
                || method.IsImplicitlyDeclared
                || !method.GetAttributes().Any(attribData => InteropMethodAttributeClass.Equals(attribData.AttributeClass, SymbolEqualityComparer.Default)))
                return false;

            var references = method.DeclaringSyntaxReferences;

            if (references.Length != 1) //Partial Method already implemented
            {
                //TODO: Create Diagnostic for this scenario
                return false;
            }

            var syntax = (MethodDeclarationSyntax)references[0].GetSyntax();

            if (!syntax.Modifiers.Any(SyntaxKind.PartialKeyword)
                || syntax.Body != null
                || syntax.ExpressionBody != null)
            {
                return false;
            }

            return true;
        }

        private bool LookForParameterProblems(IMethodSymbol method)
        {
            bool problemFound = false;

            foreach (var param in method.Parameters)
            {
                if (param.RefKind != RefKind.None)
                {
                    Context.ReportDiagnostic(Diagnostic.Create(PrimarySourceGenerator.FASG02, param.Locations[0]));

                    problemFound |= true;
                    continue;
                }

                var paramType = param.Type;

                if (paramType.IsReferenceType)
                {
                    Context.ReportDiagnostic(Diagnostic.Create(PrimarySourceGenerator.FASG03, param.Locations[0]));

                    problemFound |= true;
                    continue;
                }

                if (!paramType.IsUnmanagedType)
                {
                    Context.ReportDiagnostic(Diagnostic.Create(PrimarySourceGenerator.FASG04, param.Locations[0]));

                    problemFound |= true;
                    continue;
                }
            }

            if (!method.ReturnsVoid)
            {
                if (method.ReturnsByRef || method.ReturnsByRefReadonly)
                {
                    Context.ReportDiagnostic(Diagnostic.Create(PrimarySourceGenerator.FASG02, method.Locations[0]));
                }
            }

            return problemFound;
        }

        private static ClassDeclarationSyntax CreateNewDefinition(ClassDeclarationSyntax syntax, List<MemberDeclarationSyntax> members)
        {
            SyntaxTokenList modifiers = syntax.Modifiers;

            if (!modifiers.Any(SyntaxKind.UnsafeKeyword))
            {
                modifiers = modifiers.Insert(modifiers.Count - 1, SyntaxFactory.Token(SyntaxKind.UnsafeKeyword));
            }

            return SyntaxFactory.ClassDeclaration(default, modifiers, syntax.Identifier, null, null, default, SyntaxFactory.List(members));
        }
    }
}
