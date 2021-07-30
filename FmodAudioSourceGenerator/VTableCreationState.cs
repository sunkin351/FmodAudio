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
        private static readonly MemberAccessExpressionSyntax NativeLibrary_GetExport = (MemberAccessExpressionSyntax)SyntaxFactory.ParseExpression("NativeLibrary.GetExport");
        private static readonly MemberAccessExpressionSyntax NativeLibrary_TryGetExport = (MemberAccessExpressionSyntax)SyntaxFactory.ParseExpression("NativeLibrary.TryGetExport");
        
        private static readonly QualifiedNameSyntax NamespaceName = SyntaxFactory.QualifiedName(SyntaxFactory.IdentifierName("FmodAudio"), SyntaxFactory.IdentifierName("Base"));
        private static readonly SyntaxTokenList FieldDeclarationModifiers = SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
        private static readonly IdentifierNameSyntax LibParamName = SyntaxFactory.IdentifierName("libHandle");
        private static readonly UsingDirectiveSyntax[] RequiredUsings = new UsingDirectiveSyntax[]
        {
            SyntaxFactory.UsingDirective(
                SyntaxFactory.ParseName("System.Runtime.InteropServices")
            ),
        };

        private static readonly TypeSyntax HandleType = SyntaxFactory.IdentifierName("IntPtr");

        internal readonly VTableSyntaxReceiver Receiver;

        private readonly INamedTypeSymbol /*VTableAttributeClass, WrapperTypeAttributeClass,*/ InteropMethodAttributeClass;

        public VTableCreationState(in GeneratorExecutionContext context, VTableSyntaxReceiver receiver) : base(in context)
        {
            Receiver = receiver;

            //VTableAttributeClass = this.Compilation.GetTypeByMetadataName("FmodAudio.VTableAttribute");

            //WrapperTypeAttributeClass = this.Compilation.GetTypeByMetadataName("FmodAudio.WrapperTypeAttribute");

            InteropMethodAttributeClass = this.Compilation.GetTypeByMetadataName("FmodAudio.Base.SGAttributes.InteropMethodAttribute");
        }

        protected override IEnumerable<(string FileNameHint, string SourceText)> AttributeSources
        {
            get
            {
                return Enumerable.Empty<(string, string)>();
            }
        }

        public override void GenerateSources()
        {
            var marshalContext = new MethodMarshallingContext(this, Receiver.WrapperTypes);

            var symbol = Compilation.GetTypeByMetadataName("FmodAudio.Base.FmodLibrary");

            if (symbol is null)
                return;

            var source = ProcessVTableType(marshalContext, symbol);

            if (source != null)
            {
                Context.AddSource(symbol.Name + "_Generated", source);
            }
        }

        private SourceText ProcessVTableType(MethodMarshallingContext context, INamedTypeSymbol typeSymbol)
        {
            var classSyntax = (ClassDeclarationSyntax)typeSymbol.DeclaringSyntaxReferences[0].GetSyntax();

            Debug.Assert(classSyntax.Modifiers.Any(SyntaxKind.PartialKeyword));

            List<StatementSyntax> ConstructorStatements = new List<StatementSyntax>()
            {
                SyntaxFactory.ParseStatement("IntPtr Export;")
            };
            List<MemberDeclarationSyntax> ClassMembers = new List<MemberDeclarationSyntax>();
            var exportVariableExpression = SyntaxFactory.IdentifierName("Export");

            foreach (var member in typeSymbol.GetMembers())
            {
                Context.CancellationToken.ThrowIfCancellationRequested();

                if (!(member is IMethodSymbol method))
                    continue;

                if (!IsTargetMethod(method, out bool shouldGuard))
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

                var exportNameExpression = SyntaxFactory.LiteralExpression(
                    SyntaxKind.StringLiteralExpression,
                    SyntaxFactory.Literal("FMOD_" + method.Name)
                );

                StatementSyntax statement;

                if (shouldGuard)
                {
                    statement = SyntaxFactory.IfStatement(
                        SyntaxFactory.InvocationExpression(
                            NativeLibrary_TryGetExport,
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SeparatedList(
                                    new ArgumentSyntax[]
                                    {
                                        SyntaxFactory.Argument(LibParamName),
                                        SyntaxFactory.Argument(exportNameExpression),
                                        SyntaxFactory.Argument(null, SyntaxFactory.Token(SyntaxKind.OutKeyword), exportVariableExpression)
                                    }
                                )
                            )
                        ),
                        SyntaxFactory.ExpressionStatement(
                            SyntaxFactory.AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                SyntaxFactory.IdentifierName(fieldName),
                                SyntaxFactory.CastExpression(funcPtrType, exportVariableExpression)
                            )
                        )
                    );
                }
                else
                {
                    var invoke = SyntaxFactory.InvocationExpression(
                        NativeLibrary_GetExport,
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SeparatedList(
                                new[]
                                {
                                    SyntaxFactory.Argument(LibParamName),
                                    SyntaxFactory.Argument(exportNameExpression)
                                }
                            )
                        )
                    );

                    var assignment = SyntaxFactory.AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        SyntaxFactory.IdentifierName(fieldName),
                        SyntaxFactory.CastExpression(funcPtrType, invoke)
                    );

                    statement = SyntaxFactory.ExpressionStatement(assignment);
                }

                ConstructorStatements.Add(statement);

                ClassMembers.Add(
                    context.ImplementMethod(method, fieldName, shouldGuard)
                );
            }

            var libraryHandleProperty = SyntaxFactory.ParseMemberDeclaration("public IntPtr LibraryHandle { get; }");

            var libHandleSetStatement = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.IdentifierName("LibraryHandle"),
                    LibParamName
                )
            );

            ClassMembers.Add(libraryHandleProperty);
            ConstructorStatements.Add(libHandleSetStatement);

            Context.CancellationToken.ThrowIfCancellationRequested();

            var constructor = SyntaxFactory.ConstructorDeclaration(
                default,
                SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.InternalKeyword)),
                classSyntax.Identifier,
                SyntaxFactory.ParameterList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Parameter(default, default, HandleType, LibParamName.Identifier, null)
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

        private bool IsTargetMethod(IMethodSymbol method, out bool ShouldGuard)
        {
            ShouldGuard = false;

            if (method.MethodKind != MethodKind.Ordinary
                || method.IsImplicitlyDeclared)
                return false;

            AttributeData data = method.GetAttributes().FirstOrDefault(attribData => attribData.AttributeClass.Equals(InteropMethodAttributeClass, SymbolEqualityComparer.Default));

            if (data is null)
                return false;

            if (data.ConstructorArguments.Length > 0)
            {
                ShouldGuard = (bool)data.ConstructorArguments[0].Value;
            }

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
