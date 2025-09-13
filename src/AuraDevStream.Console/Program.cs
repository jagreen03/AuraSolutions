using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging;
using AuraDevStream.Core;

class Program
{
	static void Main(string[] args)
	{
		// Set up logging
		using var loggerFactory = LoggerFactory.Create(builder =>
		{
			builder.AddConsole();
		});
		ILogger logger = loggerFactory.CreateLogger<Program>();

		// 1. Parse command-line arguments
		ProgramArguments? parsedArgs = ArgumentHandler.Parse(args);
		if(parsedArgs == null)
		{
			return;
		}

		if(!Directory.Exists(parsedArgs.SourceDirectory))
		{
			logger.LogError($"Source directory not found at '{parsedArgs.SourceDirectory}'");
			return;
		}

		// 2. Run the aggregation process
		var aggregator = new FileAggregator(logger);
		aggregator.Aggregate(parsedArgs);
	}
}
