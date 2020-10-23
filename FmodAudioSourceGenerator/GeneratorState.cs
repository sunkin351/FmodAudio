using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace FmodAudioSourceGenerator
{
    internal abstract class GeneratorState
    {
        protected readonly GeneratorExecutionContext Context;
        
        public Compilation Compilation { get; protected set; }

        private readonly Dictionary<SyntaxTree, SemanticModel> SemanticModels = new Dictionary<SyntaxTree, SemanticModel>();

        protected GeneratorState(in GeneratorExecutionContext context)
        {
            Context = context;
            Compilation = context.Compilation;

            var trees = new List<SyntaxTree>();

            foreach (var info in AttributeSources)
            {
                var source = SourceText.From(info.SourceText, Encoding.UTF8);

                context.AddSource(info.FileNameHint, source);

                var tree = SyntaxFactory.ParseSyntaxTree(source, context.ParseOptions);

                trees.Add(tree);
            }

            if (trees.Count > 0)
                Compilation = Compilation.AddSyntaxTrees(trees);
        }

        public abstract void GenerateSources();

        protected abstract IEnumerable<(string FileNameHint, string SourceText)> AttributeSources { get; }

        public SemanticModel GetSemanticModel(SyntaxTree tree)
        {
            if (!SemanticModels.TryGetValue(tree, out var model))
            {
                model = Compilation.GetSemanticModel(tree);
                SemanticModels.Add(tree, model);
            }

            return model;
        }
    }
}
