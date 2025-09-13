namespace AuraDevStream.Core.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {

    }
}
﻿namespace AuraDevStream.Core.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {

    }
}
﻿// Program.cs
using Microsoft.Extensions.Logging;

class Program
{
	static void Main(string[] args)
	{
		// Set up logging
		using var loggerFactory = LoggerFactory.Create(builder =>
		{
			builder.AddConsole();
		});
		ILogger logger = loggerFactory.CreateLogger<Program>();

		// 1. Parse command-line arguments
		ProgramArguments parsedArgs = ArgumentHandler.Parse(args);
		if(parsedArgs == null)
		{
			return;
		}

		if(!Directory.Exists(parsedArgs.SourceDirectory))
		{
			logger.LogError($"Source directory not found at '{parsedArgs.SourceDirectory}'");
			return;
		}

		// 2. Run the aggregation process
		var aggregator = new FileAggregator(logger);
		aggregator.Aggregate(parsedArgs);
	}
}
﻿namespace AuraDevStream.Core
{
	public class ArgumentHandler
	{
		private static readonly Dictionary<string, string> _fileExtensions = new Dictionary<string, string>
	{
		{"-cs", "*.cs"},
		{"-bat", "*.bat"},
		{"-ps1", "*.ps1"},
		{"-java", "*.java"},
		{"-js", "*.js"}
	};

		public static ProgramArguments Parse(string[] args)
		{
			if(args.Length < 2)
			{
				Console.WriteLine("Usage: AIDevStream <source_directory> <output_file> [-cs] [-bat] [-ps1]... [-verbose] [-indent <spaces>] [-keywords <term1> <term2>...] [-headerformat <format>]");
				return null;
			}

			string sourceDirectory = args[0];
			string outputFile = args[1];
			bool isVerbose = false;
			int indentSize = 0;
			List<string> keywords = new List<string>();
			string headerFormat = "// {0}";
			List<string> searchPatterns = new List<string>();

			for(int i = 2; i < args.Length; i++)
			{
				string arg = args[i].ToLower();
				if(_fileExtensions.ContainsKey(arg))
				{
					searchPatterns.Add(_fileExtensions[arg]);
				}
				else if(arg == "-verbose")
				{
					isVerbose = true;
				}
				else if(arg == "-indent" && i + 1 < args.Length && int.TryParse(args[i + 1], out int indent))
				{
					indentSize = indent;
					i++;
				}
				else if(arg == "-keywords" && i + 1 < args.Length)
				{
					while(i + 1 < args.Length && !args[i + 1].StartsWith("-"))
					{
						keywords.Add(args[i + 1]);
						i++;
					}
				}
				else if(arg == "-headerformat" && i + 1 < args.Length)
				{
					headerFormat = args[i + 1];
					i++;
				}
			}

			if(!searchPatterns.Any())
			{
				searchPatterns.Add("*.cs");
			}

			return new ProgramArguments(sourceDirectory, outputFile, isVerbose, indentSize, keywords, headerFormat, searchPatterns);
		}
	}
}﻿
namespace AuraDevStream.Core
{
	public class BatchAnalyzer : IFileAnalyzer
	{
		public string Analyze(string filePath, string fileContent)
		{
			var summaryBuilder = new System.Text.StringBuilder();
			summaryBuilder.AppendLine("// --- Batch Analysis Summary ---");
			var lineCount = fileContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).Length;
			var linesWithEcho = fileContent.Split('\n').Count(line => line.Trim().StartsWith("@echo", StringComparison.OrdinalIgnoreCase));

			summaryBuilder.AppendLine($"// Total lines: {lineCount}");
			summaryBuilder.AppendLine($"// Lines with @echo: {linesWithEcho}");

			return summaryBuilder.ToString();
		}
	}
}﻿namespace AuraDevStream.Core
{
	// CSharpAnalyzer.cs
	using System.Linq;
	using System.Text.RegularExpressions;

	public class CSharpAnalyzer : IFileAnalyzer
	{
		public string Analyze(string filePath, string fileContent)
		{
			var summaryBuilder = new System.Text.StringBuilder();
			summaryBuilder.AppendLine("// --- C# Analysis Summary ---");

			// Count basic OOP constructs
			var interfaceCount = Regex.Matches(fileContent, @"\s+interface\s+", RegexOptions.IgnoreCase).Count;
			var abstractClassCount = Regex.Matches(fileContent, @"\s+abstract\s+class\s+", RegexOptions.IgnoreCase).Count;
			var enumCount = Regex.Matches(fileContent, @"\s+enum\s+", RegexOptions.IgnoreCase).Count;

			summaryBuilder.AppendLine($"// Interfaces found: {interfaceCount}");
			summaryBuilder.AppendLine($"// Abstract classes found: {abstractClassCount}");
			summaryBuilder.AppendLine($"// Enums found: {enumCount}");

			// Detect inheritance (polymorphism hint)
			var inheritanceMatches = Regex.Matches(fileContent, @":\s+\w+", RegexOptions.IgnoreCase);
			if(inheritanceMatches.Any())
			{
				summaryBuilder.AppendLine($"// Inheritance detected. Potential for polymorphism exists.");
			}

			return summaryBuilder.ToString();
		}
	}

}
﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Extensions.Logging;

namespace AuraDevStream.Core
{
	public class FileAggregator
	{
		private readonly ILogger _logger;
		private readonly Dictionary<string, IFileAnalyzer> _analyzers;

		public FileAggregator(ILogger logger)
		{
			_logger = logger;
			_analyzers = new Dictionary<string, IFileAnalyzer>(StringComparer.OrdinalIgnoreCase)
		{
			{".cs", new CSharpAnalyzer()},
			{".bat", new BatchAnalyzer()},
			{".ps1", new PowershellAnalyzer()},
			{".java", new JavaAnalyzer()},
			{".js", new JavaScriptAnalyzer()}
		};
		}

		public void Aggregate(ProgramArguments args)
		{
			var builder = new StringBuilder();

			try
			{
				var filesFound = args.SearchPatterns
					.SelectMany(pattern => Directory.GetFiles(args.SourceDirectory, pattern, SearchOption.AllDirectories))
					.Distinct()
					.ToList();

				foreach(string file in filesFound)
				{
					// Read content and apply keyword filter
					string fileContent = File.ReadAllText(file);
					if(args.Keywords.Any() && !args.Keywords.Any(k => fileContent.Contains(k, StringComparison.OrdinalIgnoreCase)))
					{
						if(args.IsVerbose)
						{
							_logger.LogInformation($"Skipping file '{file}' as it does not contain the specified keywords.");
						}
						continue;
					}

					if(args.IsVerbose)
					{
						_logger.LogInformation($"Processing file: '{file}'");
					}

					// Append the formatted file path.
					builder.AppendLine(string.Format(args.HeaderFormat, file));

					// Perform language-specific analysis and append summary
					var extension = Path.GetExtension(file);
					if(_analyzers.TryGetValue(extension, out var analyzer))
					{
						builder.AppendLine(analyzer.Analyze(file, fileContent));
					}

					// Apply indentation if requested
					if(args.IndentSize > 0)
					{
						string indentString = new string(' ', args.IndentSize);
						fileContent = string.Join(Environment.NewLine, fileContent.Split(Environment.NewLine).Select(line => indentString + line));
					}

					builder.AppendLine(fileContent);
					builder.AppendLine();
				}

				File.WriteAllText(args.OutputFile, builder.ToString());
				_logger.LogInformation($"Successfully wrote content of {filesFound.Count} files to '{args.OutputFile}'");
			}
			catch(Exception ex)
			{
				_logger.LogError($"An error occurred: {ex.Message}");
			}
		}
	}
}﻿namespace AuraDevStream.Core
{
	// IFileAnalyzer.cs
	public interface IFileAnalyzer
	{
		// A method to analyze the content of a file
		string Analyze(string filePath, string fileContent);
	}

}
﻿// JavaAnalyzer.cs
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AuraDevStream.Core
{
	public class JavaAnalyzer : IFileAnalyzer
	{
		public string Analyze(string filePath, string fileContent)
		{
			var summaryBuilder = new System.Text.StringBuilder();
			summaryBuilder.AppendLine("// --- Java Analysis Summary ---");

			var interfaceCount = Regex.Matches(fileContent, @"\s+interface\s+", RegexOptions.IgnoreCase).Count;
			var abstractClassCount = Regex.Matches(fileContent, @"\s+abstract\s+class\s+", RegexOptions.IgnoreCase).Count;
			var classCount = Regex.Matches(fileContent, @"\s+class\s+", RegexOptions.IgnoreCase).Count;
			var inheritanceCount = Regex.Matches(fileContent, @"\s+extends\s+\w+", RegexOptions.IgnoreCase).Count;

			summaryBuilder.AppendLine($"// Interfaces found: {interfaceCount}");
			summaryBuilder.AppendLine($"// Abstract classes found: {abstractClassCount}");
			summaryBuilder.AppendLine($"// Classes found: {classCount}");
			summaryBuilder.AppendLine($"// Inheritance detected: {inheritanceCount}");

			return summaryBuilder.ToString();
		}
	}

}
﻿// JavaScriptAnalyzer.cs
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AuraDevStream.Core
{
	public class JavaScriptAnalyzer : IFileAnalyzer
	{
		public string Analyze(string filePath, string fileContent)
		{
			var summaryBuilder = new System.Text.StringBuilder();
			summaryBuilder.AppendLine("// --- JavaScript Analysis Summary ---");

			var classCount = Regex.Matches(fileContent, @"\s+class\s+", RegexOptions.IgnoreCase).Count;
			var constCount = Regex.Matches(fileContent, @"\s+const\s+", RegexOptions.IgnoreCase).Count;
			var letCount = Regex.Matches(fileContent, @"\s+let\s+", RegexOptions.IgnoreCase).Count;
			var arrowFunctionCount = Regex.Matches(fileContent, @"=>", RegexOptions.IgnoreCase).Count;

			summaryBuilder.AppendLine($"// ES6 classes found: {classCount}");
			summaryBuilder.AppendLine($"// 'const' declarations found: {constCount}");
			summaryBuilder.AppendLine($"// 'let' declarations found: {letCount}");
			summaryBuilder.AppendLine($"// Arrow functions found: {arrowFunctionCount}");

			return summaryBuilder.ToString();
		}
	}

}
﻿// PowershellAnalyzer.cs
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AuraDevStream.Core
{
	public class PowershellAnalyzer : IFileAnalyzer
	{
		public string Analyze(string filePath, string fileContent)
		{
			var summaryBuilder = new System.Text.StringBuilder();
			summaryBuilder.AppendLine("// --- PowerShell Analysis Summary ---");

			var functionCount = Regex.Matches(fileContent, @"function\s+\w+", RegexOptions.IgnoreCase).Count;
			var cmdletCount = Regex.Matches(fileContent, @"Get-|Set-|New-|Remove-|Invoke-", RegexOptions.IgnoreCase).Count;

			summaryBuilder.AppendLine($"// Functions found: {functionCount}");
			summaryBuilder.AppendLine($"// Common cmdlets used: {cmdletCount}");

			return summaryBuilder.ToString();
		}
	}

}
﻿namespace AuraDevStream.Core
{
	public class ProgramArguments
	{
		public string SourceDirectory { get; }
		public string OutputFile { get; }
		public bool IsVerbose { get; }
		public int IndentSize { get; }
		public IReadOnlyList<string> Keywords { get; }
		public string HeaderFormat { get; }
		public IReadOnlyList<string> SearchPatterns { get; }

		public ProgramArguments(string sourceDirectory, string outputFile, bool isVerbose, int indentSize, List<string> keywords, string headerFormat, List<string> searchPatterns)
		{
			SourceDirectory = sourceDirectory;
			OutputFile = outputFile;
			IsVerbose = isVerbose;
			IndentSize = indentSize;
			Keywords = keywords.AsReadOnly();
			HeaderFormat = headerFormat;
			SearchPatterns = searchPatterns.AsReadOnly();
		}
	}
}﻿namespace AuraDevStream.Shared;

public class Class1
{

}
﻿namespace AuraDevStream.Core.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {

    }
}
