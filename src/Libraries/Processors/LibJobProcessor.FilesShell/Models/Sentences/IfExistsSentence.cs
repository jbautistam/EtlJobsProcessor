using Bau.Libraries.LibInterpreter.Models.Sentences;

namespace Bau.Libraries.LibJobProcessor.FilesShell.Models.Sentences;

/// <summary>
///		Sentencia que comprueba si existe un directorio o archivo
/// </summary>
internal class IfExistsSentence : BaseFileShellSentence
{
	internal IfExistsSentence(string providerFile, string providerFileKey, string path) : base(providerFile, providerFileKey)
	{
		Path = path;
	}

	/// <summary>
	///		Directorio / archivo
	/// </summary>
	internal string Path { get; }

	/// <summary>
	///		Sentencias que se ejecutan si existe el archivo
	/// </summary>
	internal List<SentenceBase> ThenSentences { get; } = new();

	/// <summary>
	///		Sentencias que se ejecutan si no existe el archivo
	/// </summary>
	internal List<SentenceBase> ElseSentences { get; } = new();
}
