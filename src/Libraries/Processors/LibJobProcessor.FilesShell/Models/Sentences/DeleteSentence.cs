namespace Bau.Libraries.LibJobProcessor.FilesShell.Models.Sentences;

/// <summary>
///		Sentencia para borrar archivos o directorios
/// </summary>
internal class DeleteSentence : BaseFileShellSentence
{
	internal DeleteSentence(string providerFile, string providerFileKey, string path, string mask) : base(providerFile, providerFileKey)
	{
		Path = path;
		Mask = mask;
	}

	/// <summary>
	///		Archivo / directorio
	/// </summary>
	internal string Path { get; }

	/// <summary>
	///		Máscara de archivos
	/// </summary>
	internal string Mask { get; }
}
