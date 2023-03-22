namespace Bau.Libraries.LibJobProcessor.Core.Interfaces.FilesManager;

/// <summary>
///		Manager de archivos (local, Azure blob storage, AWS...)
/// </summary>
public interface IFileManager
{
	/// <summary>
	///		Carga el contexto de un nodo
	/// </summary>
	void LoadContext(LibMarkupLanguage.MLNode rootML);

	/// <summary>
	///		Abre un stream de lectura o escritura
	/// </summary>
	Task<Models.Files.StreamUnionModel> OpenAsync(string providerFileKey, string fileName, Models.Files.StreamUnionModel.OpenMode mode, CancellationToken cancellationToken);

	/// <summary>
	///		Obtiene los archivos de un directorio
	/// </summary>
	Task<List<Models.Files.FileModel>> GetFilesAsync(string providerFileKey, string folder, string mask, CancellationToken cancellationToken);

	/// <summary>
	///		Obtiene un nombre de archivo completo adecuado a este administrador de archivos
	/// </summary>
	string GetFullFileName(Interpreter.IProgramInterpreter programIntepreter, string providerFileKey, string fileName);

	/// <summary>
	///		Clave del proveedor
	/// </summary>
	string Key { get; }
}
