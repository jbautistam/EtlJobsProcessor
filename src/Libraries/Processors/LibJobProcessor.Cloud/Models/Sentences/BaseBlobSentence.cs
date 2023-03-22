namespace Bau.Libraries.LibJobProcessor.Cloud.Models.Sentences;

/// <summary>
///		Sentencia base para las sentencias de blob
/// </summary>
internal abstract class BaseBlobSentence : LibInterpreter.Models.Sentences.SentenceBase
{
	internal BaseBlobSentence(string providerFile, string providerFileKey)
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

	/// <summary>
	///		Indica si se debe ejecutar la instrucción
	/// </summary>
	internal bool Enabled { get; set; } = true;

	/// <summary>
	///		Tiempo de espera
	/// </summary>
	internal TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(5);
}
