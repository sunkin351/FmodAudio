using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FmodAudioSourceGenerator;

public static class ErrorDescriptions
{
    /// <summary>
    /// <see langword="ref"/>, <see langword="in"/>, and <see langword="out"/> parameters and returns are explicitly disallowed
    /// </summary>
    internal static readonly DiagnosticDescriptor FASG02 = new(nameof(FASG02), "Ref Unsupported", "ref, in, and out parameters and ref return types are unsupported", "Native Marshaling", DiagnosticSeverity.Error, true);
    
    /// <summary>
    /// Only unmanaged types are marshalled
    /// </summary>
    internal static readonly DiagnosticDescriptor FASG03 = new(nameof(FASG03), "Unmanaged Types Only", "Types that are managed references or contain managed references are unsupported", "Native Marshaling", DiagnosticSeverity.Error, true);
    
    /// <summary>
    /// Duplicate Native Method Name
    /// </summary>
    internal static readonly DiagnosticDescriptor FASG04 = new(nameof(FASG04), "Duplicate Native Method Name", "A declaration for native method \"{0}\" already exists", "Native Marshaling", DiagnosticSeverity.Error, true);

}

public readonly record struct ValueWithDiagnostics<TValue>
{
    public TValue Value { get; }

    public Diagnostic[]? Diagnostics { get; }

    public ValueWithDiagnostics(TValue value, params Diagnostic[] diagnostics)
    {
        Value = value;
        Diagnostics = diagnostics?.Length > 0 ? diagnostics : null;
    }

    public bool Equals(ValueWithDiagnostics<TValue> other)
    {
        if (this.Diagnostics != null || other.Diagnostics != null)
            return false;

        return EqualityComparer<TValue>.Default.Equals(Value, other.Value);
    }

    public override int GetHashCode()
    {
        if (Diagnostics != null)
            return Diagnostics.GetHashCode();

        return EqualityComparer<TValue>.Default.GetHashCode(Value);
    }
}

public record struct ParameterInfo(
    string ActualType,
    string MarshalledType,
    string ParamName,
    bool IsHandle,
    bool IsFmodBool)
{
    public static ParameterInfo Collect(ITypeSymbol type, string name, INamedTypeSymbol handleInterface, INamedTypeSymbol fmodBool)
    {
        string actualTypeString, marshalledTypeString;
        bool isHandle = false, isFmodBool = false;

        marshalledTypeString = actualTypeString = type.ToDisplayString(FmodAudioNativeMarshallingGenerator.TypeFormat);

        if (type.TypeKind is not TypeKind.Pointer and not TypeKind.FunctionPointer)
        {
            if (SymbolEqualityComparer.Default.Equals(type, fmodBool))
            {
                marshalledTypeString = "int";
                isFmodBool = true;
            }
            else
            {
                var iHandle = type.AllInterfaces.FirstOrDefault(x =>
                {
                    return SymbolEqualityComparer.Default.Equals(x.OriginalDefinition, handleInterface);
                });

                if (iHandle != null)
                {
                    marshalledTypeString = iHandle.TypeArguments[0].ToDisplayString(FmodAudioNativeMarshallingGenerator.TypeFormat);
                    isHandle = true;
                }
            }
        }

        return new ParameterInfo(actualTypeString, marshalledTypeString, name, isHandle, isFmodBool);
    }
}

public record struct MethodInfo(
    string MethodName,
    string NativeMethodName,
    ParameterInfo[] Parameters,
    ParameterInfo ReturnParam,
    bool Guard,
    SyntaxKind AccessibilityKeyword
    )
{
    public bool Equals(MethodInfo other)
    {
        return MethodName == other.MethodName
            && NativeMethodName == other.NativeMethodName
            && Parameters.SequenceEqual(other.Parameters)
            && ReturnParam == other.ReturnParam;
    }

    public override int GetHashCode()
    {
        int code = MethodName.GetHashCode();

        unchecked
        {
            code = code * -1521134295 + NativeMethodName.GetHashCode();
            code = code * -1521134295 + Parameters.SequenceHashCode();
            code = code * -1521134295 + ReturnParam.GetHashCode();
        }

        return code;
    }

    public FunctionPointerTypeSyntax GetNativeFunctionPointer()
    {
        return SyntaxFactory.FunctionPointerType(
            SyntaxFactory.FunctionPointerCallingConvention(SyntaxFactory.Token(SyntaxKind.UnmanagedKeyword)),
            SyntaxFactory.FunctionPointerParameterList(
                SyntaxFactory.SeparatedList(
                    Parameters.Select(x => SyntaxFactory.FunctionPointerParameter(SyntaxFactory.ParseTypeName(x.MarshalledType)))
                        .Concat(new[] { SyntaxFactory.FunctionPointerParameter(SyntaxFactory.ParseTypeName(ReturnParam.MarshalledType)) })
                )
            )
        );
    }
}

public record struct VTableInfo(
    string TypeName,
    string Namespace,
    string[] Usings,
    MethodInfo[] VTableMethods)
{
    public VTableInfo() : this("", "", Array.Empty<string>(), Array.Empty<MethodInfo>())
    {
    }

    public bool Equals(VTableInfo other)
    {
        return TypeName == other.TypeName
            && Namespace == other.Namespace
            && Usings.SequenceEqual(other.Usings)
            && VTableMethods.SequenceEqual(other.VTableMethods);
    }

    public override int GetHashCode()
    {
        var code = TypeName.GetHashCode();

        unchecked
        {
            code = code * -1521134295 + Namespace.GetHashCode();
            code = code * -1521134295 + Usings.SequenceHashCode();
            code = code * -1521134295 + VTableMethods.SequenceHashCode();
        }

        return code;
    }
}
