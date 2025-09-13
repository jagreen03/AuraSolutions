using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Extensions.Logging;

namespace AuraDevStream.Core
{
	public class FileAggregator
	{
		private readonly ILogger _logger;
		private readonly Dictionary<string, IFileAnalyzer> _analyzers;

		public FileAggregator(ILogger logger)
		{
			_logger = logger;
			_analyzers = new Dictionary<string, IFileAnalyzer>(StringComparer.OrdinalIgnoreCase)
		{
			{".cs", new CSharpAnalyzer()},
			{".bat", new BatchAnalyzer()},
			{".ps1", new PowershellAnalyzer()},
			{".java", new JavaAnalyzer()},
			{".js", new JavaScriptAnalyzer()}
		};
		}

		public void Aggregate(ProgramArguments args)
		{
			var builder = new StringBuilder();

			try
			{
				var filesFound = args.SearchPatterns
					.SelectMany(pattern => Directory.GetFiles(args.SourceDirectory, pattern, SearchOption.AllDirectories))
					.Distinct()
					.ToList();

				foreach(string file in filesFound)
				{
					// Read content and apply keyword filter
					string fileContent = File.ReadAllText(file);
					if(args.Keywords.Any() && !args.Keywords.Any(k => fileContent.Contains(k, StringComparison.OrdinalIgnoreCase)))
					{
						if(args.IsVerbose)
						{
							_logger.LogInformation($"Skipping file '{file}' as it does not contain the specified keywords.");
						}
						continue;
					}

					if(args.IsVerbose)
					{
						_logger.LogInformation($"Processing file: '{file}'");
					}

					// Append the formatted file path.
					builder.AppendLine(string.Format(args.HeaderFormat, file));

					// Perform language-specific analysis and append summary
					var extension = Path.GetExtension(file);
					if(_analyzers.TryGetValue(extension, out var analyzer))
					{
						string type = analyzer.GetType().Name;
						switch(type)
						{
							case nameof(BatchAnalyzer):
								builder.AppendLine(analyzer.Analyze<SummaryBatchFile>(file, fileContent).Summary);
								break;
							case nameof(CSharpAnalyzer):
								builder.AppendLine(analyzer.Analyze<SummaryCSharp>(file, fileContent).Summary);
								break;
							case nameof(JavaAnalyzer):
								builder.AppendLine(analyzer.Analyze<SummaryJava>(file, fileContent).Summary);
								break;
							case nameof(JavaScriptAnalyzer):
								builder.AppendLine(analyzer.Analyze<SummaryJavaScript>(file, fileContent).Summary);
								break;
							case nameof(PowershellAnalyzer):
								builder.AppendLine(analyzer.Analyze<SummaryPowershell>(file, fileContent).Summary);
								break;
							default:
								throw new ArgumentException();
						}
						
					}

					// Apply indentation if requested
					if(args.IndentSize > 0)
					{
						string indentString = new string(' ', args.IndentSize);
						fileContent = string.Join(Environment.NewLine, fileContent.Split(Environment.NewLine).Select(line => indentString + line));
					}

					builder.AppendLine(fileContent);
					builder.AppendLine();
				}

				File.WriteAllText(args.OutputFile, builder.ToString());
				_logger.LogInformation($"Successfully wrote content of {filesFound.Count} files to '{args.OutputFile}'");
			}
			catch(Exception ex)
			{
				_logger.LogError($"An error occurred: {ex.Message}");
			}
		}
	}
}