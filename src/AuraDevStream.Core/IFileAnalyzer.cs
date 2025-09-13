namespace AuraDevStream.Core
{
	public interface IFileAnalyzer
	{
		T Analyze<T>(string filePath, string fileContent) where T : SummaryLanguage, new();
	}

}
