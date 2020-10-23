using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace FmodAudioSourceGenerator.Tests
{
    public class SourceGenerationTests
    {
        CSharpParseOptions options = new CSharpParseOptions(LanguageVersion.Preview);
        CSharpCompilation compilation;
        ISourceGenerator[] generators;

        public SourceGenerationTests()
        {
            var sources = new[] { TestFileStrings.SimpleTestFile };

            compilation = CSharpCompilation.Create("TestCompilation",
                sources.Select(source => CSharpSyntaxTree.ParseText(source, options)),
                new[] { MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true));

            generators = new ISourceGenerator[] { new PrimarySourceGenerator(), new EqualitySourceGenerator() };
        }

        [Fact]
        public void GenerationTestSimple()
        {
            var driver = CSharpGeneratorDriver.Create(generators, parseOptions: options);

            driver.RunGeneratorsAndUpdateCompilation(compilation, out var newCompile, out var diags);

            var compileDiags = newCompile.GetDiagnostics();

            Assert.False(compileDiags.Any(diag => diag.Severity == DiagnosticSeverity.Error));
            Assert.Empty(diags);
        }
    }
}
