using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Generator.CLI
{
	public static class TestCompiler
	{
		private static readonly IEnumerable<string> DefaultNamespaces =
			new[]
			{
				"System",
				"System.IO",
				"System.Net",
				"System.Linq",
				"System.Text",
				"System.Text.RegularExpressions",
				"System.Collections.Generic"
			};

		private static string RuntimePath = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.1\{0}.dll";

		private static readonly IEnumerable<MetadataReference> DefaultReferences =
			new[]
			{
				MetadataReference.CreateFromFile(string.Format(RuntimePath, "mscorlib")),
				MetadataReference.CreateFromFile(string.Format(RuntimePath, "System")),
				MetadataReference.CreateFromFile(string.Format(RuntimePath, "System.Core")),
			};

		/*
		 *System.ValueTuple.dll, mscorlib.dll, netstandard.dll, System.Runtime.dll
		 *
		 */

		private static readonly CSharpCompilationOptions DefaultCompilationOptions =
			new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
				.WithOverflowChecks(true)
				.WithOptimizationLevel(OptimizationLevel.Release)
				.WithUsings(DefaultNamespaces);

		private static SyntaxTree Parse(string text, string filename = "", CSharpParseOptions options = null)
		{
			var source = SourceText.From(text, Encoding.UTF8);
			return SyntaxFactory.ParseSyntaxTree(source, options, filename);
		}

		private static SyntaxTree[] MakeTrees(SyntaxTree parsedSyntaxTree) =>
			new SyntaxTree[]
			{
				parsedSyntaxTree
			};

		public static bool Verify(string content, string assemblyName = "test.dll", string assemblyPath = null)
		{
			assemblyPath = assemblyName ?? Path.GetTempFileName();

			var parsedSyntaxTree = Parse(content, "", CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Default));
			var compilation = CSharpCompilation.Create(
				assemblyName,
				MakeTrees(parsedSyntaxTree),
				DefaultReferences,
				DefaultCompilationOptions);

			var result = compilation.Emit(assemblyPath);

			if (!result.Success)
			{
				foreach (var diagnostic in result.Diagnostics)
				{
					Console.WriteLine(diagnostic.ToString());
				}
			}

			return result.Success;
		}
	}
}
