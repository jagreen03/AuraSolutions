// This file is the core Roslyn-based analyzer. It has been updated to correctly
// count partial classes as a single logical entity.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Linq;

namespace AuraDevStream.Core
{
	// A custom exception to provide more descriptive errors during Roslyn analysis.
	public class RoslynAnalysisException : Exception
	{
		public CodeEvaluationStage Status { get; }

		public RoslynAnalysisException(string message, CodeEvaluationStage status)
			: base(message)
		{
			Status = status;
		}
	}

	public class RoslynAnalyzer : IFileAnalyzer
	{
		public T Analyze<T>(string filePath, string fileContent) where T : SummaryLanguage, new()
		{
			CodeEvaluationStage status = CodeEvaluationStage.None;
			SyntaxTree tree = null;
			CompilationUnitSyntax root = null;
			var analysis = new SummaryCSharp();

			try
			{
				status = CodeEvaluationStage.AssemblyLoaded;
				tree = CSharpSyntaxTree.ParseText(text: fileContent, path: filePath);
				status = CodeEvaluationStage.Parsed;
				root = tree.GetCompilationUnitRoot();
				status = CodeEvaluationStage.Compiled;

				// Correctly count classes by grouping partial declarations by name.
				var allClasses = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
				int classCount = allClasses.GroupBy(c => c.Identifier.Text)
										   .Count(g => !g.Any(c => c.Modifiers.Any(SyntaxKind.AbstractKeyword)));

				int abstractClassCount = allClasses.Count(c => c.Modifiers.Any(SyntaxKind.AbstractKeyword));
				int interfaceCount = root.DescendantNodes().OfType<InterfaceDeclarationSyntax>().Count();
				int enumCount = root.DescendantNodes().OfType<EnumDeclarationSyntax>().Count();
				bool inheritance = root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>().Any(c => c.BaseList != null);

				analysis.InterfaceCount = interfaceCount;
				analysis.AbstractClassCount = abstractClassCount;
				analysis.EnumCount = enumCount;
				analysis.ClassCount = classCount;
				analysis.Inheritance = inheritance;

				status = CodeEvaluationStage.Analyzed;

				return (T)(object)analysis;
			}
			catch(Exception ex)
			{
				// Throw a custom exception with detailed status information
				string message = $"Roslyn analysis failed. Last known stage: {status}. Error: {ex.Message}";
				throw new RoslynAnalysisException(message, status);
			}
		}
	}
}
