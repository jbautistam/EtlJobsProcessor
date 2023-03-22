using Bau.Libraries.LibInterpreter.Models.Sentences;

namespace Bau.Libraries.LibJobProcessor.Manager.Models.Sentences;

/// <summary>
///		Sentencia de utilización de un archivo
/// </summary>
public class SentenceUsingFile : SentenceBase
{
	public SentenceUsingFile(string providerFile, string providerFileKey, string key, Core.Models.Files.StreamUnionModel.OpenMode mode, string fileName)
	{
		ProviderFile = providerFile;
		ProviderFileKey = providerFileKey;
		Key = key;
		Mode = mode;
		FileName = fileName;
	}

	/// <summary>
	///		Proveedor de archivos
	/// </summary>
	public string ProviderFile { get; }

	/// <summary>
	///		Clave del proveedor de archivos
	/// </summary>
	public string ProviderFileKey { get; }

	/// <summary>
	///		Clave del archivo
	/// </summary>
	public string Key { get; }

	/// <summary>
	///		Modo de apertura
	/// </summary>
	public Core.Models.Files.StreamUnionModel.OpenMode Mode { get; }

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName { get; }

	/// <summary>
	///		Instrucciones del bloque
	/// </summary>
	public List<SentenceBase> Sentences { get; } = new();
}
