
using System.Text.RegularExpressions;

namespace AuraDevStream.Core
{
	public class BatchAnalyzer : IFileAnalyzer
	{
		public T Analyze<T>(string filePath, string fileContent) where T : SummaryLanguage, new()
		{
			var analysis = new SummaryBatchFile()
			{
				LineCount = fileContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).Length,
				LinesWithEcho = fileContent.Split('\n').Count(line => line.Trim().StartsWith("@echo", StringComparison.OrdinalIgnoreCase))
			};
			
			return (T)(object)analysis;
		}
	}
}
