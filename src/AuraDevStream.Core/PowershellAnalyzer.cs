// PowershellAnalyzer.cs
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AuraDevStream.Core
{
	public class PowershellAnalyzer : IFileAnalyzer
	{
		public T Analyze<T>(string filePath, string fileContent) where T : SummaryLanguage, new()
		{
			SummaryPowershell analysis = new SummaryPowershell();
			var summaryBuilder = new System.Text.StringBuilder();
			analysis.FunctionCount = Regex.Matches(fileContent, @"function\s+\w+", RegexOptions.IgnoreCase).Count;
			analysis.CmdletCount = Regex.Matches(fileContent, @"Get-|Set-|New-|Remove-|Invoke-", RegexOptions.IgnoreCase).Count;

			return (T)(object)analysis;
		}
	}

}
