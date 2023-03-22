namespace Bau.Libraries.LibJobProcessor.Core.Models.Files;

/// <summary>
///		Modelo con los datos de un archivo
/// </summary>
public class FileModel
{
	public FileModel(string fileName, bool isFolder)
	{
		FileName = fileName;
		IsFolder = isFolder;
	}

	/// <summary>
	///		Indica si es un directorio
	/// </summary>
	public bool IsFolder { get; set; }

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName { get; set; }

	/// <summary>
	///		Tamaño del archivo
	/// </summary>
	public long Length { get; set; }
}
