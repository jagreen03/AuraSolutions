namespace AuraDevStream.Core
{
	public class ArgumentHandler
	{
		private static readonly Dictionary<string, string> _fileExtensions = new Dictionary<string, string>
	{
		{"-cs", "*.cs"},
		{"-bat", "*.bat"},
		{"-ps1", "*.ps1"},
		{"-java", "*.java"},
		{"-js", "*.js"}
	};

		public static ProgramArguments? Parse(string[] args)
		{
			if(args.Length < 2)
			{
				Console.WriteLine("Usage: AIDevStream <source_directory> <output_file> [-cs] [-bat] [-ps1]... [-verbose] [-indent <spaces>] [-keywords <term1> <term2>...] [-headerformat <format>]");
				return null;
			}

			string sourceDirectory = args[0];
			string outputFile = args[1];
			bool isVerbose = false;
			int indentSize = 0;
			List<string> keywords = new List<string>();
			string headerFormat = "// {0}";
			List<string> searchPatterns = new List<string>();

			for(int i = 2; i < args.Length; i++)
			{
				string arg = args[i].ToLower();
				if(_fileExtensions.ContainsKey(arg))
				{
					searchPatterns.Add(_fileExtensions[arg]);
				}
				else if(arg == "-verbose")
				{
					isVerbose = true;
				}
				else if(arg == "-indent" && i + 1 < args.Length && int.TryParse(args[i + 1], out int indent))
				{
					indentSize = indent;
					i++;
				}
				else if(arg == "-keywords" && i + 1 < args.Length)
				{
					while(i + 1 < args.Length && !args[i + 1].StartsWith("-"))
					{
						keywords.Add(args[i + 1]);
						i++;
					}
				}
				else if(arg == "-headerformat" && i + 1 < args.Length)
				{
					headerFormat = args[i + 1];
					i++;
				}
			}

			if(!searchPatterns.Any())
			{
				searchPatterns.Add("*.cs");
			}

			return new ProgramArguments(sourceDirectory, outputFile, isVerbose, indentSize, keywords, headerFormat, searchPatterns);
		}
	}
}