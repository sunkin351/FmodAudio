using System;
using Xunit;

using FmodAudioSourceGenerator;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection;
using System.Linq;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Diagnostics;

namespace SourceGeneratorTests
{
    public class SourceGenerationTests
    {
        CSharpParseOptions options = new CSharpParseOptions(LanguageVersion.Preview);
        CSharpCompilation compilation;
        ImmutableArray<ISourceGenerator> generators;

        public SourceGenerationTests()
        {
            var sources = new[] { TestFileStrings.VTableAttribute, TestFileStrings.SimpleTestFile };

            compilation = CSharpCompilation.Create("TestCompilation",
                sources.Select(source => CSharpSyntaxTree.ParseText(source, options)),
                new[] { MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true));

            generators = new ISourceGenerator[] { new PrimarySourceGenerator() }.ToImmutableArray();
        }

        [Fact]
        public void GenerationTestSimple()
        {
            var driver = new CSharpGeneratorDriver(options, generators, default, ImmutableArray<AdditionalText>.Empty);

            driver.RunFullGeneration(compilation, out var newCompile, out var diags);

            var compileDiags = newCompile.GetDiagnostics();

            Assert.False(compileDiags.Any(diag => diag.Severity == DiagnosticSeverity.Error));
            Assert.Empty(diags);
        }
    }
}
