namespace Bau.Libraries.LibJobProcessor.FilesStructured.Models.Sentences;

/// <summary>
///		Sentencia para abrir un reader sobre un archivo
/// </summary>
internal abstract class BaseOpenFileReaderSentence : BaseFileSentence
{
	protected BaseOpenFileReaderSentence(string name, string source, FileType type) : base(type)
	{
		Name = name;
		Source = source;
	}

	/// <summary>
	///		Nombre de la variable que se va a utilizar para mantener el lector
	/// </summary>
	internal string Name { get; }

	/// <summary>
	///		Nombre de variable sobre la que se ha abierto el archivo
	/// </summary>
	internal string Source { get; }

	/// <summary>
	///		Sentencias 
	/// </summary>
	internal List<LibInterpreter.Models.Sentences.SentenceBase> Sentences { get; } = new();
}
