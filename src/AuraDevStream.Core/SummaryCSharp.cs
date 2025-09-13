using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AuraDevStream.Core
{
	public class SummaryCSharp : SummaryLanguage
	{
		public int AbstractClassCount { get; set; }
		public int InterfaceCount { get; set; }
		public int EnumCount { get; set; }
		public int ClassCount { get; set; }
		/// <summary>
		/// Detect inheritance (polymorphism hint)
		/// </summary>
		public bool Inheritance { get; set; }
		public string Summary
		{
			get
			{
				var summaryBuilder = new StringBuilder();
				summaryBuilder.AppendLine("// --- C# Analysis Summary ---");
				summaryBuilder.AppendLine($"// Interfaces found: {InterfaceCount}");
				summaryBuilder.AppendLine($"// Abstract classes found: {AbstractClassCount}");
				summaryBuilder.AppendLine($"// Classes found: {ClassCount}");
				summaryBuilder.AppendLine($"// Enums found: {EnumCount}");

				if(Inheritance)
				{
					summaryBuilder.AppendLine($"// Inheritance detected. Potential for polymorphism exists.");
				}

				return summaryBuilder.ToString();
			}
		}
	}
}
