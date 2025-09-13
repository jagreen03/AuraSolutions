using System.Text;

namespace AuraDevStream.Core
{
	public class SummaryBatchFile : SummaryLanguage
	{
		public int LineCount { get; internal set; }
		public int LinesWithEcho { get; internal set; }

		public string Summary
		{
			get
			{
				var summaryBuilder = new StringBuilder();

				summaryBuilder.AppendLine("// --- Batch Analysis Summary ---");
				summaryBuilder.AppendLine($"// Total lines: {LineCount}");
				summaryBuilder.AppendLine($"// Lines with @echo: {LinesWithEcho}");

				return summaryBuilder.ToString();
			}
		}
	}
}