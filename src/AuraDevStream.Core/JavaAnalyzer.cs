// JavaAnalyzer.cs
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AuraDevStream.Core
{
	public class JavaAnalyzer : IFileAnalyzer
	{
		public T Analyze<T>(string filePath, string fileContent) where T : SummaryLanguage, new()
		{
			string classPattern = @"^\s*(?<modifiers>(public|private|protected|internal|abstract|sealed|static|unsafe|partial)\s+)*class\s+(?<className>\w+)\s*(?<generics><[^>]*>)?\s*(?::\s*(?<inheritance>[^{]+))?\s*{\s*";

			
			HashSet<string> distinctClassNames = new HashSet<string>();

			MatchCollection matches = Regex.Matches(fileContent, classPattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
			
			try
			{
				foreach(Match match in matches)
				{
					if(match.Success)
					{
						// Direct access to the "className" named capture group
						Group classNameGroup = match.Groups["className"];

						if(classNameGroup.Success) // Check if "className" group actually captured something
						{
							string className = classNameGroup.Value; // Get the captured string value
							distinctClassNames.Add(className); // Add to your HashSet
						}
					}
				}
			}
			catch(Exception ex)
			{
			}
			int classCount = distinctClassNames.Count;
			int interfaceCount = Regex.Matches(fileContent, @"\s+interface\s+", RegexOptions.IgnoreCase).Count;
			int abstractClassCount = Regex.Matches(fileContent, @"\s+abstract\s+class\s+", RegexOptions.IgnoreCase).Count;
			int inheritanceCount = Regex.Matches(fileContent, @"\s+extends\s+\w+", RegexOptions.IgnoreCase).Count;
			
			SummaryJava analysis = new SummaryJava()
			{
				InterfaceCount = interfaceCount,
				AbstractClassCount = abstractClassCount,
				ClassCount = classCount,
				InheritanceCount = inheritanceCount
			};

			return (T)(object)analysis;
		}
	}

}
