using Bau.Libraries.LibJobProcessor.Core.Models.Files;
using Bau.Libraries.LibBlobStorage;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Interpreter;

namespace Bau.Libraries.LibFileManager.BlobStorage;

/// <summary>
///		Manager de archivos de Azure BlobStorage
/// </summary>
public class BlobStorageFileManager : LibJobProcessor.Core.Interfaces.FilesManager.IFileManager
{
	/// <summary>
	///		Carga el contexto de un nodo
	/// </summary>
	public void LoadContext(LibMarkupLanguage.MLNode rootML)
	{
		Models.ConnectionModel? connection = new Repository.BlobStorageContextRepository().Load(rootML);

			if (connection is not null)
				Connections.Add(connection.Key, connection);
	}

	/// <summary>
	///		Abre un stream de lectura
	/// </summary>
	public async Task<StreamUnionModel> OpenAsync(string providerFileKey, string fileName, StreamUnionModel.OpenMode mode, CancellationToken cancellationToken)
	{
		return mode switch
			{
				StreamUnionModel.OpenMode.Read => new StreamUnionModel(this, mode, fileName, 
																		await GetReaderAsync(GetConnection(providerFileKey), fileName, cancellationToken)),
				StreamUnionModel.OpenMode.Write => new StreamUnionModel(this, mode, fileName, 
																		await GetWriterAsync(GetConnection(providerFileKey), fileName, cancellationToken)),
				_ => throw new NotImplementedException($"Open mode unknown {mode.ToString()}. Provider: {Key}. File name: {fileName}")
			};
    }

	/// <summary>
	///		Obtiene la conexión
	/// </summary>
	private Models.ConnectionModel GetConnection(string key)
	{
		if (Connections.TryGetValue(key, out Models.ConnectionModel? connection))
			return connection;
		else
			throw new ArgumentException($"Can't find a blob storage connection with key {key}");
	}

	/// <summary>
	///		Obtiene el manager del storage
	/// </summary>
	private AzureStorageBlobManager GetStorageManager(Models.ConnectionModel connection)
	{
		return new AzureStorageBlobManager(connection.ConnectionString);
	}

	/// <summary>
	///		Obtiene un stream de lectura sobre un archivo
	/// </summary>
	private async Task<Stream> GetReaderAsync(Models.ConnectionModel connection, string fileName, CancellationToken cancellationToken)
	{
		return await GetStorageManager(connection).OpenReadAsync(connection.Container, fileName, cancellationToken);
	}

	/// <summary>
	///		Obtiene un stream de escritura sobre un archivo
	/// </summary>
	private async Task<Stream> GetWriterAsync(Models.ConnectionModel connection, string fileName, CancellationToken cancellationToken)
	{
		return await GetStorageManager(connection).OpenWriteAsync(connection.Container, fileName, true, cancellationToken);
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
		// Aplica las variables
		if (!string.IsNullOrWhiteSpace(fileName))
		{
			// Quita las variables de proyecto
			fileName = fileName.Replace("{{ProjectWorkPath}}", string.Empty, StringComparison.CurrentCultureIgnoreCase);
			// Aplica las variables
			fileName = programIntepreter.ApplyVariables(fileName);
			// Cambia los separadores
			fileName = fileName.Replace('\\', '/');
			// Quita los separadores duplicados
			while (!string.IsNullOrWhiteSpace(fileName) && fileName.Contains("//"))
				fileName = fileName.Replace("//", "/");
		}
		// Devuelve el nombre de archivo interpretado
		return fileName;
	}

	/// <summary>
	///		Diccionario de conexiones
	/// </summary>
	internal Dictionary<string, Models.ConnectionModel> Connections { get; } = new(StringComparer.InvariantCultureIgnoreCase);

	/// <summary>
	///		Clave del proveedor
	/// </summary>
	public string Key => "BlobStorage";
}