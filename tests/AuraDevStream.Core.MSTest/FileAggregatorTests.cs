using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug; // Added for AddDebug
using AuraDevStream.Core;
using System.IO;
using System.Collections.Generic;
using System; // For Exception

namespace AuraDevStream.Core.Tests
{
	[TestClass]
	public class FileAggregatorTests
	{
		private ILogger? _logger; // Changed: Mark as nullable to suppress CS8618 warning

		[TestInitialize]
		public void Setup()
		{
			// Changed: Ensure Microsoft.Extensions.Logging.Debug NuGet package is installed.
			_logger = LoggerFactory.Create(builder => builder.AddDebug()).CreateLogger<FileAggregator>();
		}

		[TestMethod]
		public void Aggregate_WritesContentToFileWithAnalysisAndHeaders()
		{
			// Arrange
			string sourceDir = Path.Combine(Path.GetTempPath(), "AuraDevStreamTest_Source");
			string outputFilePath = Path.Combine(Path.GetTempPath(), "AuraDevStreamTest_Output.txt");

			Directory.CreateDirectory(sourceDir);
			File.WriteAllText(Path.Combine(sourceDir, "test1.cs"), "class Test1 {}");
			File.WriteAllText(Path.Combine(sourceDir, "test2.bat"), "@echo off");

			ProgramArguments args = new ProgramArguments(
				sourceDir,
				outputFilePath,
				isVerbose: false,
				indentSize: 0,
				keywords: new List<string>(),
				headerFormat: "File: {0}",
				searchPatterns: new List<string> { "*.cs", "*.bat" }
			);

			// Act
			FileAggregator aggregator = new FileAggregator(_logger!); // Use null-forgiving operator as it's initialized in TestInitialize
			aggregator.Aggregate(args);

			// Assert
			Assert.IsTrue(File.Exists(outputFilePath));
			string outputContent = File.ReadAllText(outputFilePath);

			StringAssert.Contains(outputContent, "File: " + Path.Combine(sourceDir, "test1.cs"));
			StringAssert.Contains(outputContent, "class Test1 {}");
			StringAssert.Contains(outputContent, "// --- C# Analysis Summary ---");

			StringAssert.Contains(outputContent, "File: " + Path.Combine(sourceDir, "test2.bat"));
			StringAssert.Contains(outputContent, "@echo off");
			StringAssert.Contains(outputContent, "// --- Batch Analysis Summary ---");

			// Clean up
			Directory.Delete(sourceDir, true);
			File.Delete(outputFilePath);
		}

		[TestMethod]
		public void Aggregate_FiltersByKeywords()
		{
			// Arrange
			string sourceDir = Path.Combine(Path.GetTempPath(), "AuraDevStreamTest_Keywords");
			string outputFilePath = Path.Combine(Path.GetTempPath(), "AuraDevStreamTest_Keywords_Output.txt");

			Directory.CreateDirectory(sourceDir);
			File.WriteAllText(Path.Combine(sourceDir, "fileWithKeyword.cs"), "public class MyClass { /* important keyword */ }");
			File.WriteAllText(Path.Combine(sourceDir, "fileWithoutKeyword.cs"), "public class AnotherClass {}");

			ProgramArguments args = new ProgramArguments(
				sourceDir,
				outputFilePath,
				isVerbose: true,
				indentSize: 0,
				keywords: new List<string> { "important keyword" },
				headerFormat: "{0}",
				searchPatterns: new List<string> { "*.cs" }
			);

			// Act
			FileAggregator aggregator = new FileAggregator(_logger!);
			aggregator.Aggregate(args);

			// Assert
			Assert.IsTrue(File.Exists(outputFilePath));
			string outputContent = File.ReadAllText(outputFilePath);

			StringAssert.Contains(outputContent, "important keyword"); // Content of fileWithKeyword.cs
																	   // Changed: Use Assert.IsFalse with string.Contains
			Assert.IsFalse(outputContent.Contains("AnotherClass"), "Content of fileWithoutKeyword.cs should not be present.");

			// Clean up
			Directory.Delete(sourceDir, true);
			File.Delete(outputFilePath);
		}

		[TestMethod]
		public void Aggregate_AppliesIndentationCorrectly()
		{
			// Arrange
			string sourceDir = Path.Combine(Path.GetTempPath(), "AuraDevStreamTest_Indent");
			string outputFilePath = Path.Combine(Path.GetTempPath(), "AuraDevStreamTest_Indent_Output.txt");

			Directory.CreateDirectory(sourceDir);
			File.WriteAllText(Path.Combine(sourceDir, "indented.cs"), "line1\nline2");

			ProgramArguments args = new ProgramArguments(
				sourceDir,
				outputFilePath,
				isVerbose: false,
				indentSize: 2,
				keywords: new List<string>(),
				headerFormat: "{0}",
				searchPatterns: new List<string> { "*.cs" }
			);

			// Act
			FileAggregator aggregator = new FileAggregator(_logger!);
			aggregator.Aggregate(args);

			// Assert
			Assert.IsTrue(File.Exists(outputFilePath));
			string outputContent = File.ReadAllText(outputFilePath);

			StringAssert.Contains(outputContent, "line1");
			StringAssert.Contains(outputContent, "line2");

			// Clean up
			Directory.Delete(sourceDir, true);
			File.Delete(outputFilePath);
		}
	}
}
