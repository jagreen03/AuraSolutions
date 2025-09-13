using Microsoft.VisualStudio.TestTools.UnitTesting;
using AuraDevStream.Core;

namespace AuraDevStream.Core.MSTest
{
	[TestClass]
	public class JavaAnalyzerTests
	{
		[TestMethod]
		public void Analyze_JavaFileContent_ReturnsCorrectSummary()
		{
			// Arrange
			JavaAnalyzer analyzer = new JavaAnalyzer();
			string fileContent = @"
                package com.example;

                interface MyJavaInterface {}
                public abstract class AbstractJavaClass {}
                class ConcreteClass extends AbstractJavaClass {}
                public class AnotherClass {}";

			// Act
			var analysis = analyzer.Analyze<SummaryJava>("test.java", fileContent);
			string summary = analysis.Summary;

			Assert.AreEqual(1, analysis.InterfaceCount);
			Assert.AreEqual(1, analysis.AbstractClassCount);
			Assert.AreEqual(2, analysis.ClassCount);
			Assert.AreEqual(1, analysis.InheritanceCount);

			//// Assert
			//StringAssert.Contains(summary, "// --- Java Analysis Summary ---");
			//StringAssert.Contains(summary, "// Interfaces found: 1");
			//StringAssert.Contains(summary, "// Abstract classes found: 1");
			//StringAssert.Contains(summary, "// Classes found: 2");
			//StringAssert.Contains(summary, "// Inheritance detected via 'extends': 1");
		}

		[TestMethod]
		public void Analyze_EmptyJavaFile_ReturnsZeros()
		{
			// Arrange
			JavaAnalyzer analyzer = new JavaAnalyzer();
			string fileContent = "";

			// Act
			var analysis = analyzer.Analyze<SummaryJava>("empty.java", fileContent);
			string summary = analysis.Summary;

			// Assert

			Assert.AreEqual(analysis.InterfaceCount, 0);
			Assert.AreEqual(analysis.AbstractClassCount, 0);
			Assert.AreEqual(analysis.ClassCount, 0);
			Assert.AreEqual(analysis.InheritanceCount, 0);

			StringAssert.Contains(summary, "// Interfaces found: 0");
			StringAssert.Contains(summary, "// Abstract classes found: 0");
			StringAssert.Contains(summary, "// Classes found: 0");
			StringAssert.Contains(summary, "// Inheritance detected via 'extends': 0");
		}
	}
}
