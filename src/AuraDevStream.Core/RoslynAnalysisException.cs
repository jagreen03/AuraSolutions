namespace AuraDevStream.Core
{
	/// <summary>
	/// A custom exception to provide more descriptive errors during Roslyn analysis.
	/// </summary>
	public class RoslynAnalysisException : Exception
	{
		public CodeEvaluationStage Status { get; }

		public RoslynAnalysisException(string message, CodeEvaluationStage status)
			: base(message)
		{
			Status = status;
		}
	}

}
