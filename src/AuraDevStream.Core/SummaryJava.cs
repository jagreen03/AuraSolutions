using System.Text;

namespace AuraDevStream.Core
{
	public class SummaryJava : SummaryLanguage
	{
		public int InterfaceCount { get; set; } = 0;
		public int AbstractClassCount { get; set; } = 0;
		public int ClassCount { get; set; } = 0;
		public int InheritanceCount { get; set; } = 0;

		public string Summary
		{
			get
			{
				StringBuilder summaryBuilder = new StringBuilder();
				summaryBuilder.AppendLine("// --- Java Analysis Summary ---");
				summaryBuilder.AppendLine($"// Interfaces found: {InterfaceCount}");
				summaryBuilder.AppendLine($"// Abstract classes found: {AbstractClassCount}");
				summaryBuilder.AppendLine($"// Classes found: {ClassCount}");
				summaryBuilder.AppendLine($"// Inheritance detected via 'extends': {InheritanceCount}");
				return summaryBuilder.ToString();
			}
		}
	}

}
