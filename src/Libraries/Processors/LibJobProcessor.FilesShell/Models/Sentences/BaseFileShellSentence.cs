using Bau.Libraries.LibInterpreter.Models.Sentences;

namespace Bau.Libraries.LibJobProcessor.FilesShell.Models.Sentences;

/// <summary>
///		Sentencia base para las instrucciones del procesador
/// </summary>
internal abstract class BaseFileShellSentence : SentenceBase
{
	internal BaseFileShellSentence(string providerFile, string providerFileKey)
	{
		ProviderFile = providerFile;
		ProviderFileKey = providerFileKey;
	}

	/// <summary>
	///		Proveedor de archivos
	/// </summary>
	internal string ProviderFile { get; }

	/// <summary>
	///		Clave del proveedor de archivos
	/// </summary>
	internal string ProviderFileKey { get; }
}
