using System;
using System.IO;
using System.Text;
using CommandDotNet;
using CommandDotNet.Attributes;
using Generator.CLI.SourceGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;

namespace Generator.CLI
{
	public class ConsoleShell
	{
		[ApplicationMetadata(
			Name = "generate",
			Description = "Generates the source code for TaskTupleAwaiter")]
		[SubCommand]
		public class Generate
		{
			[ApplicationMetadata(
				Name = "f",
				Description = "Generates a file using the given parameters")]
			public int File(
				[Argument(Name = "f", Description = "Path where the generated file should be located.")]
				string file,
				[Option(ShortName = "a", Description = "Level of arity used to generate the file")]
				int arity = 16,
				[Option(ShortName = "v", Description = "Verifies if the code compiles", BooleanMode = BooleanMode.Explicit)]
				bool verifyCompilation = true,
				[Option(Description = "Specifies if the file should be formatted", BooleanMode = BooleanMode.Explicit)]
				bool format = false
				)
			{
				var content = TaskTupleAwaiterGenerator.GenerateContent(arity);

				try
				{
					if (verifyCompilation)
					{
						Console.WriteLine("Verifying if file can compile.");
						if (!TestCompiler.Verify(content))
						{
							Console.WriteLine("Test compilation failed.");
							return 1;
						}
					}

					if (!System.IO.Directory.Exists(Path.GetDirectoryName(file)))
					{
						Console.WriteLine($"Creating directory {Path.GetDirectoryName(file)}.");
						System.IO.Directory.CreateDirectory(Path.GetDirectoryName(file));
					}

					try
					{
						SaveContent(file, content, format);
						return 0;
					}
					catch (Exception e)
					{
						Console.WriteLine("File write process failed.");
						Console.WriteLine(e);
						return 2;
					}
				}
				catch (Exception e)
				{
					Console.WriteLine("Test compilation failed.");
					Console.WriteLine(e);
					return 1;
				}
			}

			private static void SaveContent(string file, string content, bool format)
			{
				if (format)
				{
					Console.WriteLine($"Formatting file.");
					var ws = new AdhocWorkspace();
					content = Formatter
						.Format(CSharpSyntaxTree.ParseText(content, CSharpParseOptions.Default).GetRoot(), ws, ws.Options)
						.ToFullString();
				}

				Console.WriteLine($"Writing file {file}.");
				System.IO.File.WriteAllText(file, content, Encoding.UTF8);
				Console.WriteLine($"success.");
			}

			[ApplicationMetadata(
				Name = "d",
				Description = "Generates a file using the given parameters at the specified directory with the file name TaskTupleGenerator.cs")]
			public int Directory(
				[Argument(Name = "d", Description = "Folder at which the file should be generated.")]
				string directory,
				[Option(ShortName = "a", Description = "Level of arity used to generate the file")]
				int arity = 16,
				[Option(ShortName = "v", Description = "Verifies if the code compiles", BooleanMode = BooleanMode.Explicit)]
				bool verifyCompilation = true,
				[Option(Description = "Specifies if the file should be formatted", BooleanMode = BooleanMode.Explicit)]
				bool format = false
				)
			{
				return File(Path.Combine(directory, "TaskTupleExtensions.cs"), arity, verifyCompilation, format);
			}
		}
	}
}
