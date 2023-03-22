using Bau.Libraries.LibBlobStorage;
using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibJobProcessor.Cloud.Models;
using Bau.Libraries.LibJobProcessor.Cloud.Models.Sentences;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Interpreter;

namespace Bau.Libraries.LibJobProcessor.Cloud.Manager;

/// <summary>
///		Intérprete del script
/// </summary>
internal class ScriptInterpreter : BaseJobSentenceIntepreter
{
	internal ScriptInterpreter(IProgramInterpreter programInterpreter) : base(programInterpreter) {}

	/// <summary>
	///		Inicializa el intérprete
	/// </summary>
	public override async Task InitializeAsync(CancellationToken cancellationToken)
	{
		await Task.Delay(1, cancellationToken);
	}

	/// <summary>
	///		Procesa una sentencia
	/// </summary>
	public async override Task<bool> ProcessAsync(SentenceBase sentenceBase, CancellationToken cancellationToken)
	{
		bool executed = false;

			// Ejecuta la sentencia
			switch (sentenceBase)
			{
				case UploadBlobSentence sentence:
						await ProcessUploadAsync(sentence);
					break;
				case DownloadBlobSentence sentence:
						await ProcessDownloadAsync(sentence);
					break;
				case DownloadBlobFolderSentence sentence:
						await ProcessDownloadFolderAsync(sentence, cancellationToken);
					break;
				case CopyBlobSentence sentence:
						await ProcessCopyAsync(sentence, cancellationToken);
					break;
				default:
						executed = false;
					break;
			}
		// Devuelve el valor que indica si se ha ejecutado la sentencia
		return executed;
	}

	/// <summary>
	///		Sube un archivo al blob
	/// </summary>
	private async Task ProcessUploadAsync(UploadBlobSentence sentence)
	{
		CloudConnection? connection = GetConnection(sentence.ProviderFileKey);

			// Ejecuta el proceso
			if (connection is null)
				AddError($"Can't find the connection for '{sentence.ProviderFileKey}'");
			else
			{
				string? fileName = GetFullFileName(sentence.ProviderFile, sentence.ProviderFileKey, sentence.FileName);

					if (!File.Exists(fileName))
						AddError($"Can't find the file '{fileName}'");
					else
						try
						{
							using (AzureStorageBlobManager manager = new(connection.StorageConnectionString))
							{
								// Sube el archivo
								await manager.UploadAsync(GetContainerName(sentence.Target.Container), sentence.Target.Blob, fileName);
								// Log
								LogInfo($"Uploaded file '{sentence.FileName}' to '{sentence.Target.ToString()}'");
							}
						}
						catch (Exception exception)
						{
							AddError($"Error when upload '{sentence.FileName}' to '{sentence.Target.ToString()}'", exception);
						}
			}
	}

	/// <summary>
	///		Descarga un archivo del blob
	/// </summary>
	private async Task ProcessDownloadAsync(DownloadBlobSentence sentence)
	{
		CloudConnection? connection = GetConnection(sentence.ProviderFileKey);

			// Log
			LogInfo($"Start downloading from '{sentence.Source.ToString()}'");
			// Descarga los datos
			if (connection is null)
				AddError($"Can't find the connection for '{sentence.ProviderFileKey}'");
			else
				try
				{
					// Descarga el archivo
					using (AzureStorageBlobManager manager = new(connection.StorageConnectionString))
					{
						string? fileName = GetFullFileName(sentence.ProviderFile, sentence.ProviderFileKey, sentence.FileName);

							if (!string.IsNullOrWhiteSpace(fileName))
							{
								// Crea el directorio
								LibHelper.Files.HelperFiles.MakePath(Path.GetDirectoryName(fileName));
								// Descarga el archivo
								await manager.DownloadAsync(GetContainerName(sentence.Source.Container), sentence.Source.Blob, fileName);
							}
					}
					// Log
					LogInfo($"Downloaded file '{sentence.FileName}' to '{sentence.Source.ToString()}'");
				}
				catch (Exception exception)
				{
					AddError($"Error when download '{sentence.FileName}' to '{sentence.Source.ToString()}'", exception);
				}
	}

	/// <summary>
	///		Procesa la sentencia de descarga de una carpeta
	/// </summary>
	private async Task ProcessDownloadFolderAsync(DownloadBlobFolderSentence sentence, CancellationToken cancellationToken)
	{
		CloudConnection? connection = GetConnection(sentence.ProviderFileKey);

			// Log
			LogInfo($"Start downloading from '{sentence.Source.ToString()}'");
			// Procesa la descarga
			if (connection is null)
				AddError($"Can't find the connection for '{sentence.ProviderFileKey}");
			else
				try
				{
					using (AzureStorageBlobManager manager = new(connection.StorageConnectionString))
					{
						string? path = GetFullFileName(sentence.ProviderFile, sentence.ProviderFileKey, sentence.Path);
						string? container = GetContainerName(sentence.Source.Container);

							// Log
							LogInfo($"Start download '{container}/{sentence.Source.Blob}' to '{path}'");
							// Crea la carpeta
							LibHelper.Files.HelperFiles.MakePath(path);
							// Descarga la carpeta
							foreach (LibBlobStorage.Metadata.BlobModel blob in await manager.ListBlobsAsync(container, sentence.Source.Blob))
								if (blob.Length != 0 && !cancellationToken.IsCancellationRequested)
								{
									string fileName = System.IO.Path.Combine(path, blob.LocalFileName);

										// Log
										LogInfo($"Download '{blob.FullFileName}'");
										// Crea el directorio
										LibHelper.Files.HelperFiles.MakePath(System.IO.Path.GetDirectoryName(fileName));
										// Descarga el archivo
										await manager.DownloadAsync(container, blob.FullFileName, fileName);
								}
							// Log
							LogInfo($"End download '{container}/{sentence.Source.Blob}' to '{path}'");
					}
				}
				catch (Exception exception)
				{
					AddError($"Error when download to '{sentence.Path}'", exception);
				}
	}

	/// <summary>
	///		Procesa una copia de archivos
	/// </summary>
	private async Task ProcessCopyAsync(CopyBlobSentence sentence, CancellationToken cancellationToken)
	{
		CloudConnection? connection = GetConnection(sentence.ProviderFileKey);
		string processType = sentence.Move ? "move" : "copy";
		string? blobTarget = sentence.Target.Blob;

			// Obtiene el nombre del archivo destino si hay que transformarlo en un nombre único
			if (sentence.TransformFileName)
				blobTarget = System.IO.Path.GetFileNameWithoutExtension(sentence.Target.Blob) +
								$" {DateTime.UtcNow:yyyy-MM-dd HH_mm_ss_ms}" +
								System.IO.Path.GetExtension(sentence.Target.Blob);
			// Log
			LogInfo($"Start {processType} from '{sentence.Source.ToString()}' to '{sentence.Target.Container}/{blobTarget}'");
			// Procesa la instrucción
			if (connection == null)
				AddError($"Can't find the connection for '{sentence.ProviderFileKey}'");
			else
				try
				{
					using (AzureStorageBlobManager manager = new(connection.StorageConnectionString))
					{
						// Copia / mueve el archivo
						if (sentence.Move)
							await manager.MoveAsync(GetContainerName(sentence.Source.Container), sentence.Target.Blob, 
													GetContainerName(sentence.Target.Container), blobTarget, cancellationToken);
						else
							await manager.CopyAsync(GetContainerName(sentence.Source.Container), sentence.Target.Blob, 
													GetContainerName(sentence.Target.Container), blobTarget, cancellationToken);
						// Log
						LogInfo($"End {processType} from '{sentence.Source.ToString()}' to '{sentence.Target.Container}/{blobTarget}'");
					}
				}
				catch (Exception exception)
				{
					AddError($"Error when {processType} from '{sentence.Source.ToString()}' to '{sentence.Target.Container}/{blobTarget}'", exception);
				}
	}

	/// <summary>
	///		Obtiene la conexión
	/// </summary>
	private CloudConnection? GetConnection(string? storageKey)
	{
		Core.Models.Jobs.JobContextModel? context = GetContext(storageKey);

			if (context is CloudConnection connection)
				return connection;
			else
				return null;
	}

	/// <summary>
	///		Obtiene el nombre del contenedor. Si es necesario, lo recoge de los parámetros
	/// </summary>
	private string? GetContainerName(string? container)
	{
		// Quita los espacios
		container = container.TrimIgnoreNull();
		// Si el nombre del contenedor es del tipo {{parámetro}} lo sustituye por el valor del parámetro
		if (!string.IsNullOrWhiteSpace(container) && container.StartsWith("{{") && container.EndsWith("}}") && container.Length > 4)
		{
			object? value = ProgramInterpreter.GetVariableValue(container[2..^2]);

				// Sustituye por el valor del parámetro
				if (value is null)
					container = string.Empty;
				else
					container = value?.ToString();
		}
		// Devuelve el nombre del contenedor
		return container;
	}
}