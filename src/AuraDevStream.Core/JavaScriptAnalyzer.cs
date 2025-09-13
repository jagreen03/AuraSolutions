// JavaScriptAnalyzer.cs
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AuraDevStream.Core
{
	public class JavaScriptAnalyzer : IFileAnalyzer
	{
		public T Analyze<T>(string filePath, string fileContent) where T : SummaryLanguage, new()
		{
			var analysis = new SummaryJavaScript()
			{
				ClassCount = Regex.Matches(fileContent, @"\s+class\s+", RegexOptions.IgnoreCase).Count,
				ConstCount = Regex.Matches(fileContent, @"\s+const\s+", RegexOptions.IgnoreCase).Count,
				LetCount = Regex.Matches(fileContent, @"\s+let\s+", RegexOptions.IgnoreCase).Count,
				ArrowFunctionCount = Regex.Matches(fileContent, @"=>", RegexOptions.IgnoreCase).Count
			};

			return (T)(object)analysis;
		}
	}

}
