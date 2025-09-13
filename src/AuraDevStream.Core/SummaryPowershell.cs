using System.Text;

namespace AuraDevStream.Core
{
	public class SummaryPowershell : SummaryLanguage
	{
		public int FunctionCount { get; set; }
		public int CmdletCount { get; set; }
		public string Summary
		{
			get
			{
				var summaryBuilder = new StringBuilder();
				summaryBuilder.AppendLine("// --- PowerShell Analysis Summary ---");
				summaryBuilder.AppendLine($"// Functions found: {FunctionCount}");
				summaryBuilder.AppendLine($"// Common cmdlets used: {CmdletCount}");

				return summaryBuilder.ToString();
			}
		}
	}
}
