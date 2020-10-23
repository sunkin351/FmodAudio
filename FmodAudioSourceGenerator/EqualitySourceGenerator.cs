using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FmodAudioSourceGenerator
{
    internal class EqualitySyntaxReciever : ISyntaxReceiver
    {
        public List<TypeDeclarationSyntax> TypeDeclarations { get; } = new List<TypeDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is TypeDeclarationSyntax typeDecl && Helpers.HasAttributeOfName(typeDecl, "EqualityBoilerplate"))
            {
                TypeDeclarations.Add(typeDecl);
            }
        }
    }


    [Generator]
    public class EqualitySourceGenerator : ISourceGenerator
    {
        internal static readonly DiagnosticDescriptor FASG09 = new DiagnosticDescriptor(nameof(FASG10), "Class or Struct", "Type Declaration must be a class or a struct", "", DiagnosticSeverity.Warning, true);
        internal static readonly DiagnosticDescriptor FASG10 = new DiagnosticDescriptor(nameof(FASG10), "Type Location", "Type Declaration must not be located within another Type.", "", DiagnosticSeverity.Warning, true);
        internal static readonly DiagnosticDescriptor FASG11 = new DiagnosticDescriptor(nameof(FASG11), "Type Partiality", "Type Declaration must be partial.", "", DiagnosticSeverity.Warning, true);
        internal static readonly DiagnosticDescriptor FASG12 = new DiagnosticDescriptor(nameof(FASG12), "Must Implement Interface", "Type must implement IEquatable<{0}>.", "", DiagnosticSeverity.Warning, true);

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new EqualitySyntaxReciever());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxReceiver is EqualitySyntaxReciever syntaxReciever))
                return;

            var state = new EqualityGeneratorState(in context, syntaxReciever);

            if (context.CancellationToken.IsCancellationRequested)
                return;

            state.GenerateSources();
        }
    }
}
