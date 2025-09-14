// This file is the core Roslyn-based analyzer. It has been updated to include
// a custom exception for robust error handling.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Linq;

namespace AuraDevStream.Core
{
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

				// Analyze for interfaces, classes, and enums using Roslyn syntax tree
				int interfaceCount = root.DescendantNodes().OfType<InterfaceDeclarationSyntax>().Count();
				int abstractClassCount = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Count(c => c.Modifiers.Any(SyntaxKind.AbstractKeyword));
				int enumCount = root.DescendantNodes().OfType<EnumDeclarationSyntax>().Count();
				int classCount = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Count(c => !c.Modifiers.Any(SyntaxKind.AbstractKeyword));
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
