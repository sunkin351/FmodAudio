using Microsoft.CodeAnalysis;

namespace FmodAudioSourceGenerator
{
    [Generator]
    public class PrimarySourceGenerator : ISourceGenerator
    {
        internal static readonly DiagnosticDescriptor FASG02 = new DiagnosticDescriptor(nameof(FASG02), "Ref Unsupported", "ref, out, and in parameters and ref return types are unsupported", "Source Generator", DiagnosticSeverity.Error, true);
        internal static readonly DiagnosticDescriptor FASG03 = new DiagnosticDescriptor(nameof(FASG03), "Reference Type Parameter found", "Reference type marshalling is not supported", "Source Generator", DiagnosticSeverity.Error, true);
        internal static readonly DiagnosticDescriptor FASG04 = new DiagnosticDescriptor(nameof(FASG04), "Unmanaged Structures only", "Structs that contain references are not supported for marshalling", "", DiagnosticSeverity.Error, true);

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new VTableSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxReceiver is VTableSyntaxReceiver reciever))
                return;

            var state = new VTableCreationState(context, reciever);

            if (context.CancellationToken.IsCancellationRequested)
                return;

            state.GenerateSources();
        }
    }
}
