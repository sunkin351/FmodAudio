﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FmodAudioSourceGenerator
{
    internal class MethodMarshallingContext
    {
        List<INamedTypeSymbol> WrapperSymbols = new List<INamedTypeSymbol>();
        INamedTypeSymbol FmodBoolType;

        public MethodMarshallingContext(VTableCreationState state, List<StructDeclarationSyntax> RegisteredWrappers)
        {
            WrapperSymbols.Capacity = RegisteredWrappers.Count;

            foreach (var decl in RegisteredWrappers)
            {
                var model = state.GetSemanticModel(decl.SyntaxTree);

                var symbol = model.GetDeclaredSymbol(decl) ?? throw new InvalidOperationException();

                //TODO: Verify Attribute

                var handleSymbol = symbol.GetMembers()
                    .FirstOrDefault(member => ((member is IFieldSymbol field && field.Type.IsUnmanagedType)
                                            || (member is IPropertySymbol prop && prop.Type.IsUnmanagedType))
                                            && member.Name == "Handle");

                if (handleSymbol is null)
                {
                    //TODO: Create Diagnostic for this situation
                    continue;
                }

                if (handleSymbol.DeclaredAccessibility != Accessibility.Public)
                {
                    //TODO: Create Diagnostic for this situation
                    continue;
                }

                WrapperSymbols.Add(symbol);
            }

            FmodBoolType = state.Compilation.GetTypeByMetadataName("FmodAudio.Base.FmodBool");
        }

        public FunctionPointerTypeSyntax GenerateFunctionPointerType(IMethodSymbol method)
        {
            var symbolParameters = method.Parameters;
            var methodSyntax = (MethodDeclarationSyntax)method.DeclaringSyntaxReferences[0].GetSyntax();
            var syntaxParameters = methodSyntax.ParameterList.Parameters;

            var list = new List<FunctionPointerParameterSyntax>(symbolParameters.Length + 1);

            for (int i = 0; i < syntaxParameters.Count; ++i)
            {
                var param = syntaxParameters[i];
                var paramSymbol = symbolParameters[i];

                TypeSyntax typeSyntax = param.Type;

                if (paramSymbol.Type is INamedTypeSymbol namedType)
                {
                    if (WrapperSymbols.Contains(namedType))
                    {
                        typeSyntax = LookupHandleType(namedType);
                    }
                    else if (FmodBoolType != null && namedType.Equals(FmodBoolType, SymbolEqualityComparer.Default))
                    {
                        typeSyntax = SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword));
                    }
                }

                var tmp = SyntaxFactory.FunctionPointerParameter(typeSyntax);

                list.Add(tmp);
            }

            //Marshaling of return type is not currently supported
            list.Add(SyntaxFactory.FunctionPointerParameter(methodSyntax.ReturnType));

            //All Fmod library functions are stdcall
            return SyntaxFactory.FunctionPointerType(
                SyntaxFactory.FunctionPointerCallingConvention(
                    SyntaxFactory.Token(SyntaxKind.UnmanagedKeyword)
                ),
                SyntaxFactory.FunctionPointerParameterList(
                    SyntaxFactory.SeparatedList(list)
                )
            );
        }

        public MethodDeclarationSyntax ImplementMethod(IMethodSymbol method, string fieldName, bool guard)
        {
            var args = new List<ArgumentSyntax>(method.Parameters.Length);
            var syntax = (MethodDeclarationSyntax)method.DeclaringSyntaxReferences[0].GetSyntax();

            var handleName = SyntaxFactory.IdentifierName("Handle");

            foreach (var param in method.Parameters.Zip(syntax.ParameterList.Parameters, (symbol, paramSyntax) => (symbol, paramSyntax)))
            {
                ExpressionSyntax expression;

                if (param.symbol.Type.TypeKind != TypeKind.Pointer && WrapperSymbols.Contains(param.symbol.Type))
                {
                    expression = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName(param.paramSyntax.Identifier), handleName);
                }
                else if (param.symbol.Type.Equals(FmodBoolType, SymbolEqualityComparer.Default))
                {
                    expression = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.IdentifierName(param.paramSyntax.Identifier), SyntaxFactory.IdentifierName("value"));
                }
                else
                {
                    expression = SyntaxFactory.IdentifierName(param.paramSyntax.Identifier);
                }

                args.Add(SyntaxFactory.Argument(expression));
            }

            var fieldIdentifier = SyntaxFactory.IdentifierName(fieldName);

            var invoke = SyntaxFactory.InvocationExpression(
                fieldIdentifier,
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SeparatedList(args)
                )
            );

            var revisedParameterList = SyntaxFactory.ParameterList(
                SyntaxFactory.SeparatedList(
                    syntax.ParameterList.Parameters.Select(param => param.WithDefault(null))
                )
            );

            StatementSyntax statement = method.ReturnType.SpecialType == SpecialType.System_Void
                    ? (StatementSyntax)SyntaxFactory.ExpressionStatement(invoke)
                    : SyntaxFactory.ReturnStatement(invoke);

            if (guard)
            {
                statement = SyntaxFactory.IfStatement(
                    SyntaxFactory.BinaryExpression(
                        SyntaxKind.NotEqualsExpression,
                        fieldIdentifier,
                        SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)
                    ),
                    statement,
                    SyntaxFactory.ElseClause(
                        SyntaxFactory.ParseStatement($"throw new MissingMethodException(\"FMOD_{method.Name} was not found while loading Fmod. Consider upgrading to the version of Fmod containing this function.\");")
                    )
                );
            }

            var body = SyntaxFactory.Block(SyntaxFactory.SingletonList(statement));

            return SyntaxFactory.MethodDeclaration(default, syntax.Modifiers, syntax.ReturnType, default,
                                                   syntax.Identifier, default, revisedParameterList, default, body, null,
                                                   default);
        }

        private TypeSyntax LookupHandleType(INamedTypeSymbol symbol)
        {
            foreach (var member in symbol.GetMembers())
            {
                if (member is IFieldSymbol field && field.Name == "Handle")
                {
                    return SyntaxFactory.IdentifierName(field.Type.Name);
                }
                else if (member is IPropertySymbol property && property.Name == "Handle")
                {
                    return SyntaxFactory.IdentifierName(property.Type.Name);
                }
            }

            throw new InvalidOperationException("Unable to find Handle field or property");
        }
    }
}
