using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FmodAudioSourceGenerator;

[Generator(LanguageNames.CSharp)]
public class FmodAudioNativeMarshallingGenerator : IIncrementalGenerator
{
    internal static readonly SymbolDisplayFormat TypeFormat = new(
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
        
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes
    );

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource("Attributes.cs", """
                using System;

                namespace SGHelpers;

                [AttributeUsage(AttributeTargets.Class)]
                internal class NativeVTableAttribute : Attribute
                {
                    public string? MethodNamePrefix { get; set; }

                }

                [AttributeUsage(AttributeTargets.Method)]
                internal class InteropMethodAttribute : Attribute
                {
                    /// <summary>
                    /// Name of the native method to load
                    /// </summary>
                    public string? InteropName { get; }

                    /// <summary>
                    /// If True, source generator will elegantly handle this method not being loaded from the Native Library.
                    /// </summary>
                    public bool Guard { get; set; } = false;

                    public InteropMethodAttribute()
                    {
                    }

                    public InteropMethodAttribute(string? interopName)
                    {
                        InteropName = interopName;
                    }
                }
                """
            );
        });

        var vtables = context.SyntaxProvider.ForAttributeWithMetadataName(
            "SGHelpers.NativeVTableAttribute",
            (node, cancel) =>
            {
                return node is ClassDeclarationSyntax @class && @class.Modifiers.Any(SyntaxKind.PartialKeyword);
            },
            CollectInfo
        ).WithTrackingName("VTable Information");

        context.RegisterSourceOutput(vtables, GenerateSources);
    }

    private static bool ShouldAddUsing(INamespaceSymbol @using, INamespaceSymbol parent)
    {
        if (SymbolEqualityComparer.Default.Equals(@using, parent))
            return false;

        while ((parent = parent.ContainingNamespace) is INamespaceSymbol { IsGlobalNamespace: false })
        {
            if (SymbolEqualityComparer.Default.Equals(@using, parent))
                return false;
        }

        return true;
    }

    private static ValueWithDiagnostics<VTableInfo> CollectInfo(GeneratorAttributeSyntaxContext ctx, CancellationToken cancel)
    {
        string? prefix = null;
        try
        {
            prefix = ctx.Attributes[0].NamedArguments
                .Where(x => x.Key == "MethodNamePrefix")
                .Select(x => x.Value)
                .SingleOrDefault().Value as string;
        }
        catch
        {
        }

        cancel.ThrowIfCancellationRequested();

        var vtableSymbol = (INamedTypeSymbol)ctx.TargetSymbol;

        if (vtableSymbol.ContainingType != null)
        {
            return new ValueWithDiagnostics<VTableInfo>();
        }

        var compile = ctx.SemanticModel.Compilation;

        var interopMethodAttr = compile.GetTypeByMetadataName("SGHelpers.InteropMethodAttribute");
        var fmodBool = compile.GetTypeByMetadataName("FmodAudio.Base.FmodBool");
        var handleInterface = compile.GetTypeByMetadataName("FmodAudio.Base.IHandleType`1");

        var parentNs = vtableSymbol.ContainingNamespace;

        var diagnostics = new List<Diagnostic>();
        var parameterList = new List<ParameterInfo>();
        var methodList = new List<MethodInfo>();

        var nativeNames = new HashSet<string>();
        var usings = new HashSet<INamespaceSymbol>(SymbolEqualityComparer.Default);

        foreach (var method in vtableSymbol.GetMembers().OfType<IMethodSymbol>())
        {
            cancel.ThrowIfCancellationRequested();

            var methodAttributes = method.GetAttributes();

            var attr = methodAttributes.FirstOrDefault(x =>
            {
                return SymbolEqualityComparer.Default.Equals(x.AttributeClass, interopMethodAttr);
            });

            if (attr is null)
            {
                continue;
            }

            if (method.MethodKind != MethodKind.Ordinary
                || method.IsImplicitlyDeclared
                || method.IsStatic)
            {
                continue;
            }

            {
                var refs = method.DeclaringSyntaxReferences;

                if (refs.Length != 1)
                    continue;

                var syntax = (MethodDeclarationSyntax)refs[0].GetSyntax(cancel);

                if (!syntax.Modifiers.Any(SyntaxKind.PartialKeyword)
                    || syntax.Body is not null
                    || syntax.ExpressionBody is not null)
                {
                    continue;
                }
            }

            parameterList.Clear();

            var problemFound = !TryCollectParameters(method, parameterList, usings, diagnostics);

            if (method.ReturnsByRef || method.ReturnsByRefReadonly)
            {
                diagnostics.Add(Diagnostic.Create(ErrorDescriptions.FASG02, method.Locations[0]));

                continue;
            }
            else if (problemFound)
            {
                continue;
            }

            var args = attr.ConstructorArguments;

            var name = method.Name;

            string nativeMethodName = args.Length >= 1 && args[0].Value is string val ? val : string.Concat(prefix, name);

            if (nativeNames.Add(nativeMethodName))
            {
                methodList.Add(
                    new MethodInfo(
                        name,
                        nativeMethodName,
                        parameterList.ToArray(),
                        ParameterInfo.Collect(method.ReturnType, "", handleInterface, fmodBool),
                        (bool?)attr.NamedArguments.FirstOrDefault(x => x.Key == "Guard").Value.Value ?? false,
                        method.DeclaredAccessibility switch
                        {
                            Accessibility.Public => SyntaxKind.PublicKeyword,
                            Accessibility.Internal => SyntaxKind.InternalKeyword,
                            Accessibility.Protected => SyntaxKind.ProtectedKeyword,
                            _ => SyntaxKind.PrivateKeyword
                        }
                    )
                );
            }
            else
            {
                diagnostics.Add(Diagnostic.Create(ErrorDescriptions.FASG04, method.Locations[0], nativeMethodName));
            }
        }

        return new ValueWithDiagnostics<VTableInfo>(
            new VTableInfo(vtableSymbol.Name, parentNs.ToDisplayString(), usings.Select(x => x.ToDisplayString()).ToArray(), methodList.ToArray()),
            diagnostics.ToArray()
        );

        bool ParamTypeCheck(ITypeSymbol type, Location loc, List<Diagnostic> diagnostics)
        {
            if (!type.IsUnmanagedType)
            {
                diagnostics.Add(Diagnostic.Create(ErrorDescriptions.FASG03, loc));

                return false;
            }

            return true;
        }

        bool TryCollectParameters(IMethodSymbol method, List<ParameterInfo> parameters, HashSet<INamespaceSymbol> usings, List<Diagnostic> diagnostics)
        {
            bool problemFound = false;

            foreach (var param in method.Parameters)
            {
                cancel.ThrowIfCancellationRequested();

                if (param.RefKind != RefKind.None)
                {
                    diagnostics.Add(Diagnostic.Create(ErrorDescriptions.FASG02, param.Locations[0]));
                    problemFound = true;
                    continue;
                }

                var paramType = param.Type;

                if (!ParamTypeCheck(paramType, param.Locations[0], diagnostics))
                {
                    problemFound = true;
                    continue;
                }

                parameters.Add(
                    ParameterInfo.Collect(paramType, param.Name, handleInterface, fmodBool)
                );

                var paramTypeNs = paramType is IPointerTypeSymbol ptype
                    ? ptype.PointedAtType.ContainingNamespace
                    : paramType.ContainingNamespace;

                if (paramTypeNs is not null && ShouldAddUsing(paramTypeNs, parentNs))
                {
                    usings.Add(paramTypeNs);
                }
            }

            if (problemFound)
            {
                parameters.Clear();
            }

            return !problemFound;
        }
    }

    private static void GenerateSources(SourceProductionContext ctx, ValueWithDiagnostics<VTableInfo> vtableValue)
    {
        if (vtableValue.Diagnostics is not null)
        {
            foreach (var diag in vtableValue.Diagnostics)
            {
                ctx.ReportDiagnostic(diag);
            }
        }

        var vtable = vtableValue.Value;

        if (vtable.VTableMethods.Length == 0)
        {
            return;
        }

        var cancel = ctx.CancellationToken;

        var memberList = new List<MemberDeclarationSyntax>(vtable.VTableMethods.Length * 2 + 2);

        var constructorStatements = new List<StatementSyntax>()
            {
                SyntaxFactory.ParseStatement("LibraryHandle = libHandle;"),
                SyntaxFactory.ParseStatement("nint export;")
            };

        var fieldModifiers = SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
        var exportVariable = SyntaxFactory.IdentifierName("export");
        var exportArgument = SyntaxFactory.Argument(
            null,
            SyntaxFactory.Token(SyntaxKind.OutKeyword),
            exportVariable
        );
        var libHandleArgument = SyntaxFactory.Argument(
            SyntaxFactory.IdentifierName("libHandle")
        );

        foreach (var methodInfo in vtable.VTableMethods)
        {
            cancel.ThrowIfCancellationRequested();

            var fpType = methodInfo.GetNativeFunctionPointer();

            var fieldIdentifier = SyntaxFactory.Identifier(methodInfo.NativeMethodName + "_Func");
            var fieldName = SyntaxFactory.IdentifierName(fieldIdentifier);

            var field = SyntaxFactory.FieldDeclaration(
                default,
                fieldModifiers,
                SyntaxFactory.VariableDeclaration(
                    fpType,
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.VariableDeclarator(fieldIdentifier)
                    )
                )
            );

            memberList.Add(field);

            ExpressionSyntax invokeExpr = SyntaxFactory.InvocationExpression(
                fieldName,
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SeparatedList(
                        methodInfo.Parameters.Select(x =>
                        {
                            ExpressionSyntax expr = SyntaxFactory.IdentifierName(x.ParamName);

                            if (x.IsHandle)
                            {
                                expr = SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    expr,
                                    SyntaxFactory.IdentifierName("Handle")
                                );
                            }
                            else if (x.IsFmodBool)
                            {
                                expr = SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    expr,
                                    SyntaxFactory.IdentifierName("value")
                                );
                            }

                            return SyntaxFactory.Argument(expr);
                        })
                    )
                )
            );

            var returnType = SyntaxFactory.ParseTypeName(methodInfo.ReturnParam.ActualType);

            StatementSyntax statement;

            if (returnType is PredefinedTypeSyntax predefined && predefined.Keyword.IsKind(SyntaxKind.VoidKeyword))
            {
                statement = SyntaxFactory.ExpressionStatement(invokeExpr);
            }
            else
            {
                if (methodInfo.ReturnParam.IsHandle || methodInfo.ReturnParam.IsFmodBool)
                {
                    invokeExpr = SyntaxFactory.ObjectCreationExpression(
                        returnType,
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SingletonSeparatedList(
                                SyntaxFactory.Argument(invokeExpr)
                            )
                        ),
                        null
                    );
                }

                statement = SyntaxFactory.ReturnStatement(invokeExpr);
            }

            var nativeMethodNameLiteralArgument = SyntaxFactory.Argument(
                SyntaxFactory.LiteralExpression(
                    SyntaxKind.StringLiteralExpression,
                    SyntaxFactory.Literal(methodInfo.NativeMethodName)
                )
            );

            if (methodInfo.Guard)
            {
                statement = SyntaxFactory.IfStatement(
                    SyntaxFactory.BinaryExpression(
                        SyntaxKind.NotEqualsExpression,
                        fieldName,
                        SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)
                    ),
                    statement,
                    SyntaxFactory.ElseClause(
                        SyntaxFactory.ParseStatement($"throw new MissingMethodException(\"{methodInfo.NativeMethodName} was not found while loading Fmod. Consider upgrading to the version of Fmod containing this function.\");")
                    )
                );

                constructorStatements.Add(
                    SyntaxFactory.IfStatement(
                        SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName("NativeLibrary"),
                                SyntaxFactory.IdentifierName("TryGetExport")
                            ),
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SeparatedList(
                                    new ArgumentSyntax[]
                                    {
                                            libHandleArgument,
                                            nativeMethodNameLiteralArgument,
                                            exportArgument
                                    }
                                )
                            )
                        ),
                        SyntaxFactory.ExpressionStatement(
                            SyntaxFactory.AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                SyntaxFactory.IdentifierName(fieldIdentifier),
                                SyntaxFactory.CastExpression(fpType, exportVariable)
                            )
                        )
                    )
                );
            }
            else
            {
                constructorStatements.Add(
                    SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            fieldName,
                            SyntaxFactory.CastExpression(
                                fpType,
                                SyntaxFactory.InvocationExpression(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.IdentifierName("NativeLibrary"),
                                        SyntaxFactory.IdentifierName("GetExport")
                                    ),
                                    SyntaxFactory.ArgumentList(
                                        SyntaxFactory.SeparatedList(
                                            new ArgumentSyntax[]
                                            {
                                                    libHandleArgument,
                                                    nativeMethodNameLiteralArgument
                                            }
                                        )
                                    )
                                )
                            )
                        )
                    )
                );
            }

            var methodImpl = SyntaxFactory.MethodDeclaration(
                default,
                SyntaxFactory.TokenList(
                    SyntaxFactory.Token(methodInfo.AccessibilityKeyword),
                    SyntaxFactory.Token(SyntaxKind.PartialKeyword)
                ),
                returnType,
                null,
                SyntaxFactory.Identifier(methodInfo.MethodName),
                null,
                SyntaxFactory.ParameterList(
                    SyntaxFactory.SeparatedList(
                        methodInfo.Parameters.Select(
                            x => SyntaxFactory.Parameter(
                                default,
                                default,
                                SyntaxFactory.ParseTypeName(x.ActualType),
                                SyntaxFactory.Identifier(x.ParamName),
                                null
                            )
                        )
                    )
                ),
                default,
                SyntaxFactory.Block(
                    SyntaxFactory.SingletonList(statement)
                ),
                null,
                default
            );

            memberList.Add(methodImpl);
        }

        cancel.ThrowIfCancellationRequested();

        memberList.Add(SyntaxFactory.ParseMemberDeclaration("internal nint LibraryHandle { get; }")!);

        memberList.Add(
            SyntaxFactory.ConstructorDeclaration(
                default,
                SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
                SyntaxFactory.Identifier(vtable.TypeName),
                SyntaxFactory.ParameterList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Parameter(
                            default,
                            default,
                            SyntaxFactory.ParseTypeName("nint"),
                            SyntaxFactory.Identifier("libHandle"),
                            null
                        )
                    )
                ),
                null,
                SyntaxFactory.Block(constructorStatements)
            )
        );

        var classDecl = SyntaxFactory.ClassDeclaration(
            default,
            SyntaxFactory.TokenList(
                SyntaxFactory.Token(SyntaxKind.UnsafeKeyword),
                SyntaxFactory.Token(SyntaxKind.PartialKeyword)
            ),
            SyntaxFactory.Identifier(vtable.TypeName),
            null,
            null,
            default,
            SyntaxFactory.List(memberList)
        );

        var nsDecl = SyntaxFactory.FileScopedNamespaceDeclaration(
            default,
            default,
            SyntaxFactory.ParseName(vtable.Namespace),
            default,
            default,
            SyntaxFactory.SingletonList<MemberDeclarationSyntax>(classDecl)
        );

        var cu = SyntaxFactory.CompilationUnit(
            default,
            SyntaxFactory.List(
                vtable.Usings.Concat(new[] {"System.Runtime.InteropServices"}).Select(
                    x => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(x))
                )
            ),
            default,
            SyntaxFactory.SingletonList<MemberDeclarationSyntax>(nsDecl)
        ).NormalizeWhitespace();

        ctx.AddSource(vtable.TypeName + "_Generated.cs", cu.ToFullString());
    }
}