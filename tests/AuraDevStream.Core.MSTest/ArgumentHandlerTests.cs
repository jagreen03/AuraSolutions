using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq; // Added for .ToList()
using AuraDevStream.Core;

namespace AuraDevStream.Core.Tests
{
	[TestClass]
	public class ArgumentHandlerTests
	{
		[TestMethod]
		public void Parse_ValidArguments_ReturnsProgramArguments()
		{
			// Arrange
			string[] args = { "C:\\Source", "C:\\Output\\all.cs", "-cs", "-verbose", "-indent", "4", "-keywords", "term1", "term2", "-headerformat", "# {0}" };

			// Act
			ProgramArguments? result = ArgumentHandler.Parse(args);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual("C:\\Source", result.SourceDirectory);
			Assert.AreEqual("C:\\Output\\all.cs", result.OutputFile);
			Assert.IsTrue(result.IsVerbose);
			Assert.AreEqual(4, result.IndentSize);
			// Changed: Convert to List<string> for CollectionAssert
			CollectionAssert.Contains(result.Keywords.ToList(), "term1");
			CollectionAssert.Contains(result.Keywords.ToList(), "term2");
			CollectionAssert.Contains(result.SearchPatterns.ToList(), "*.cs");
			Assert.AreEqual("# {0}", result.HeaderFormat);
		}

		[TestMethod]
		public void Parse_MissingArguments_ReturnsNull()
		{
			// Arrange
			string[] args = { "C:\\Source" }; // Missing output file

			// Act
			ProgramArguments? result = ArgumentHandler.Parse(args);

			// Assert
			Assert.IsNull(result);
		}

		[TestMethod]
		public void Parse_NoFileExtensionSpecified_DefaultsToCs()
		{
			// Arrange
			string[] args = { "C:\\Source", "C:\\Output\\all.cs" };

			// Act
			ProgramArguments? result = ArgumentHandler.Parse(args);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.SearchPatterns.Count);
			// Changed: Convert to List<string> for CollectionAssert
			CollectionAssert.Contains(result.SearchPatterns.ToList(), "*.cs");
		}

		[TestMethod]
		public void Parse_MultipleFileExtensionsSpecified_IncludesAll()
		{
			// Arrange
			string[] args = { "C:\\Source", "C:\\Output\\all.cs", "-cs", "-java", "-ps1" };

			// Act
			ProgramArguments? result = ArgumentHandler.Parse(args);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.SearchPatterns.Count);
			// Changed: Convert to List<string> for CollectionAssert
			CollectionAssert.Contains(result.SearchPatterns.ToList(), "*.cs");
			CollectionAssert.Contains(result.SearchPatterns.ToList(), "*.java");
			CollectionAssert.Contains(result.SearchPatterns.ToList(), "*.ps1");
		}

		[TestMethod]
		public void Parse_KeywordsWithoutTerms_HandlesCorrectly()
		{
			// Arrange
			string[] args = { "C:\\Source", "C:\\Output\\all.cs", "-keywords", "-verbose" };

			// Act
			ProgramArguments? result = ArgumentHandler.Parse(args);

			// Assert
			Assert.IsNotNull(result);
			// Changed: Use Assert.AreEqual for empty collection
			Assert.AreEqual(0, result.Keywords.Count); // No keywords should be added if no terms follow
			Assert.IsTrue(result.IsVerbose);
		}
	}
}
