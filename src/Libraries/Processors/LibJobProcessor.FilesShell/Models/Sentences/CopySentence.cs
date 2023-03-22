namespace Bau.Libraries.LibJobProcessor.FilesShell.Models.Sentences;

/// <summary>
///		Sentencia para copiar archivos o directorios
/// </summary>
internal class CopySentence : BaseFileShellSentence
{
	internal CopySentence(string providerFile, string providerFileKey, string source, string target, string mask) : base(providerFile, providerFileKey)
	{
		Source = source;
		Target = target;
		Mask = mask;
	}

	/// <summary>
	///		Archivo / directorio origen
	/// </summary>
	internal string Source { get; }

	/// <summary>
	///		Archivo / directorio destino
	/// </summary>
	internal string Target { get; }

	/// <summary>
	///		Máscara de archivos
	/// </summary>
	internal string Mask { get; }

	/// <summary>
	///		Indica si se tienen que copiar recursivamente los directorios
	/// </summary>
	internal bool Recursive { get; set; }

	/// <summary>
	///		Indica si al copiar directorios se deben pegar sobre un único directorio
	/// </summary>
	internal bool FlattenPaths { get; set; }
}
