namespace Bau.Libraries.LibJobProcessor.FilesShell.Models.Sentences;

/// <summary>
///		Sentencia de ejecución de un proceso
/// </summary>
internal class ExecuteSentence : BaseFileShellSentence
{
	/// <summary>
	///		Argumento de una sentencia de ejecución
	/// </summary>
	internal record ExecuteSentenceArgument(string Key, string Value, bool TransformFileName);

	internal ExecuteSentence(string providerFile, string providerFileKey, string process) : base(providerFile, providerFileKey)
	{
		Process = process;
	}

	/// <summary>
	///		Proceso que se va a ejecutar
	/// </summary>
	internal string Process { get; }

	/// <summary>
	///		Argumentos del proceso
	/// </summary>
	internal List<ExecuteSentenceArgument> Arguments = new();

	/// <summary>
	///		Tiempo de espera de la ejecución
	/// </summary>
	internal TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(5);
}
