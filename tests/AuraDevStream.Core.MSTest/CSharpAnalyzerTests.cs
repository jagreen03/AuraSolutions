using Microsoft.VisualStudio.TestTools.UnitTesting;
using AuraDevStream.Core;

namespace AuraDevStream.Core.Tests
{
	[TestClass]
	public class CSharpAnalyzerTests
	{
		[TestMethod]
		public void Analyze_CSharpFileContent_ReturnsCorrectSummary()
		{
			// Arrange
			CSharpAnalyzer analyzer = new CSharpAnalyzer();
			string fileContent = @"
                namespace MyNamespace
                {
                    interface IMyInterface {}
                    public abstract class AbstractClass {}
                    enum MyEnum { Value1, Value2 }
                    class MyClass : AbstractClass, IMyInterface {}
                }";

			// Act
			string summary = analyzer.Analyze<SummaryCSharp>("test.cs", fileContent).Summary;

			// Assert
			StringAssert.Contains(summary, "// --- C# Analysis Summary ---");
			StringAssert.Contains(summary, "// Interfaces found: 1");
			StringAssert.Contains(summary, "// Abstract classes found: 1");
			StringAssert.Contains(summary, "// Enums found: 1");
			StringAssert.Contains(summary, "// Inheritance detected. Potential for polymorphism exists.");
		}

		[TestMethod]
		public void Analyze_CSharpFileWithoutInheritance_NoPolymorphismHint()
		{
			// Arrange
			CSharpAnalyzer analyzer = new CSharpAnalyzer();
			string fileContent = @"
                namespace MyNamespace
                {
                    public class SimpleClass {}
                }";

			// Act
			string summary = analyzer.Analyze<SummaryCSharp>("simple.cs", fileContent).Summary;

			// Assert
			// Changed: Use Assert.IsFalse with string.Contains
			Assert.IsFalse(summary.Contains("// Inheritance detected. Potential for polymorphism exists."), "Polymorphism hint should not be present.");
		}
	}
}
