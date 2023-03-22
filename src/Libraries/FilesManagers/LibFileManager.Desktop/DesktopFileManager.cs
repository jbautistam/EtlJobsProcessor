using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Interpreter;
using Bau.Libraries.LibJobProcessor.Core.Models.Files;

namespace Bau.Libraries.LibFileManager.Desktop;

/// <summary>
///		Manager de archivos de escritorio
/// </summary>
public class DesktopFileManager : LibJobProcessor.Core.Interfaces.FilesManager.IFileManager
{
	/// <summary>
	///		Carga el contexto de un nodo
	/// </summary>
	public void LoadContext(LibMarkupLanguage.MLNode rootML)
	{
		// ... carga el contexto. En este caso no necesita ninguno
	}

	/// <summary>
	///		Abre un stream de lectura
	/// </summary>
	public async Task<StreamUnionModel> OpenAsync(string providerFileKey, string fileName, StreamUnionModel.OpenMode mode, CancellationToken cancellationToken)
	{
		return mode switch
			{
				StreamUnionModel.OpenMode.Read => new StreamUnionModel(this, mode, fileName, await GetReaderAsync(fileName, cancellationToken)),
				StreamUnionModel.OpenMode.Write => new StreamUnionModel(this, mode, fileName, await GetWriterAsync(fileName, cancellationToken)),
				_ => throw new NotImplementedException($"Open mode unknown {mode.ToString()}. Provider: {Key}. File name: {fileName}")
			};
    }

	/// <summary>
	///		Obtiene un stream de lectura sobre un archivo
	/// </summary>
	private async Task<Stream> GetReaderAsync(string fileName, CancellationToken cancellationToken)
	{
		await Task.Delay(1, cancellationToken);
		return new FileStream(fileName, FileMode.Open, FileAccess.Read);
	}

	/// <summary>
	///		Obtiene un stream de escritura sobre un archivo
	/// </summary>
	private async Task<Stream> GetWriterAsync(string fileName, CancellationToken cancellationToken)
	{
		await Task.Delay(1, cancellationToken);
		return new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
	}

	/// <summary>
	///		Obtiene los archivos y directorios de una carpeta
	/// </summary>
	public async Task<List<FileModel>> GetFilesAsync(string providerFileKey, string folder, string mask, CancellationToken cancellationToken)
	{
		List<FileModel> files = new();

			// Evita las advertencias
			await Task.Delay(1, cancellationToken);
			// Añade los directorios de la carpeta
			foreach (string path in Directory.GetDirectories(folder))
				if (!cancellationToken.IsCancellationRequested)
					files.Add(new FileModel(path, true));
			// Añade los archivos de la carpeta
			foreach (string file in Directory.GetFiles(folder, mask))
				if (!cancellationToken.IsCancellationRequested)
					files.Add(new FileModel(file, false));
			// Devuelve la lista
			return files;
	}

	/// <summary>
	///		Obtiene un nombre de archivo completo adecuado a este administrador de archivos
	/// </summary>
	public string GetFullFileName(IProgramInterpreter programIntepreter, string providerFileKey, string fileName)
	{
		string fullFileName = fileName ?? string.Empty;
		bool isUncPath = false;

			// Comprueba si es un directorio UNC
			if (fullFileName.StartsWith("\\\\"))
			{
				isUncPath = true;
				fullFileName = fullFileName[2..];
			}
			// Reemplaza los caracteres / por \ y quita los duplicados
			fullFileName = fullFileName.Replace('/', '\\');
			while (fullFileName.Contains("\\\\"))
				fullFileName = fullFileName.Replace("\\\\", "\\");
			// Añade los caracteres de un directorio UNC
			if (isUncPath)
				fullFileName = "\\\\" + fullFileName;
			// Devuelve el nombre de archivo aplicando el resto de variables
			return programIntepreter.ApplyVariables(fullFileName);
	}

	/// <summary>
	///		Clave del proveedor
	/// </summary>
	public string Key => "Desktop";
}