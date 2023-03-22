namespace Bau.Libraries.LibJobProcessor.FilesStructured.Models.Sentences;

/// <summary>
///		Sentencia para escribir sobre un archivo
/// </summary>
internal abstract class BaseWriteToSentence : BaseFileSentence
{
	protected BaseWriteToSentence(string source, string target, FileType type) : base(type)
	{
		Source = source;
		Target = target;
	}

	/// <summary>
	///		Nombre de variable sobre la que se ha abierto el reader
	/// </summary>
	internal string Source { get; }

	/// <summary>
	///		Nombre de la variable donde se ha abierto el archivo de escritura
	/// </summary>
	internal string Target { get; }

	/// <summary>
	///		Mapeos
	/// </summary>
	internal List<(string source, string target)> Mappings { get; } = new();
}
