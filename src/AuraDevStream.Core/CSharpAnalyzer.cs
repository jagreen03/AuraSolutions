namespace AuraDevStream.Core
{
	// CSharpAnalyzer.cs
	using System.Linq;
	using System.Text.RegularExpressions;

	public class CSharpAnalyzer : IFileAnalyzer
	{
		public T Analyze<T>(string filePath, string fileContent) where T : SummaryLanguage, new()
		{
			var inheritanceMatches = Regex.Matches(fileContent, @":\s+\w+", RegexOptions.IgnoreCase);
			
			var analysis = new SummaryCSharp
			{
				InterfaceCount = Regex.Matches(fileContent, @"\s+interface\s+", RegexOptions.IgnoreCase).Count,
				AbstractClassCount = Regex.Matches(fileContent, @"\s+abstract\s+class\s+", RegexOptions.IgnoreCase).Count,
				EnumCount = Regex.Matches(fileContent, @"\s+enum\s+", RegexOptions.IgnoreCase).Count,
				ClassCount = Regex.Matches(fileContent, @"\s+class\s+\w+", RegexOptions.IgnoreCase).Count,
				Inheritance = inheritanceMatches?.Any() ?? false,
			};

			return (T)(object)analysis;
		}
	}

}
