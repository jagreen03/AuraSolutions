using Microsoft.VisualStudio.TestTools.UnitTesting;
using AuraDevStream.Core;

namespace AuraDevStream.Core.MSTest
{
	[TestClass]
	public class BatchAnalyzerTests
	{
		[TestMethod]
		public void Analyze_BatchFileContent_ReturnsCorrectSummary()
		{
			// Arrange
			BatchAnalyzer analyzer = new BatchAnalyzer();
			string fileContent = "@echo off\r\nrem This is a remark\r\n@echo Hello World\r\nexit";

			// Act
			var analysis = analyzer.Analyze<SummaryBatchFile>("test.bat", fileContent);
			string summary = analysis.Summary;

			// Assert
			StringAssert.Contains(summary, "// --- Batch Analysis Summary ---");
			StringAssert.Contains(summary, "// Total lines: 4");
			StringAssert.Contains(summary, "// Lines with @echo: 2");
		}

		[TestMethod]
		public void Analyze_EmptyBatchFile_ReturnsCorrectSummary()
		{
			// Arrange
			BatchAnalyzer analyzer = new BatchAnalyzer();
			string fileContent = "";

			// Act
			string summary = analyzer.Analyze<SummaryBatchFile>("empty.bat", fileContent).Summary;

			// Assert
			StringAssert.Contains(summary, "// --- Batch Analysis Summary ---");
			StringAssert.Contains(summary, "// Total lines: 1"); // Empty string still counts as one line in this logic
			StringAssert.Contains(summary, "// Lines with @echo: 0");
		}
	}
}
