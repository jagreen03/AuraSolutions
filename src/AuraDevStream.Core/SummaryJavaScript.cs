using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuraDevStream.Core
{
	public class SummaryJavaScript : SummaryLanguage
	{
		public int ConstCount { get; set; } = 0;
		public int LetCount { get; set; } = 0;
		public int ClassCount { get; set; } = 0;
		public int ArrowFunctionCount { get; set; } = 0;

		public string Summary
		{
			get
			{
				var summaryBuilder = new System.Text.StringBuilder();
				summaryBuilder.AppendLine("// --- JavaScript Analysis Summary ---");
				summaryBuilder.AppendLine($"// ES6 classes found: {ClassCount}");
				summaryBuilder.AppendLine($"// 'const' declarations found: {ConstCount}");
				summaryBuilder.AppendLine($"// 'let' declarations found: {LetCount}");
				summaryBuilder.AppendLine($"// Arrow functions found: {ArrowFunctionCount}");
				return summaryBuilder.ToString();
			}
		}
	}
}