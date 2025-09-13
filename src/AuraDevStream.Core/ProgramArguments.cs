namespace AuraDevStream.Core
{
	public class ProgramArguments
	{
		public string SourceDirectory { get; }
		public string OutputFile { get; }
		public bool IsVerbose { get; }
		public int IndentSize { get; }
		public IReadOnlyList<string> Keywords { get; }
		public string HeaderFormat { get; }
		public IReadOnlyList<string> SearchPatterns { get; }

		public ProgramArguments(string sourceDirectory, string outputFile, bool isVerbose, int indentSize, List<string> keywords, string headerFormat, List<string> searchPatterns)
		{
			SourceDirectory = sourceDirectory;
			OutputFile = outputFile;
			IsVerbose = isVerbose;
			IndentSize = indentSize;
			Keywords = keywords.AsReadOnly();
			HeaderFormat = headerFormat;
			SearchPatterns = searchPatterns.AsReadOnly();
		}
	}
}