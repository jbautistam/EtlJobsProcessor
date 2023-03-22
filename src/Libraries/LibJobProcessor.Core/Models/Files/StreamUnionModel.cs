using Bau.Libraries.LibJobProcessor.Core.Interfaces.FilesManager;

namespace Bau.Libraries.LibJobProcessor.Core.Models.Files;

/// <summary>
///		Clase para con el modelo de stream abierto sobre un archivo
/// </summary>
public class StreamUnionModel : IDisposable
{
	/// <summary>
	///		Modo de apertura
	/// </summary>
	public enum OpenMode
	{
		/// <summary>Abre para lectura</summary>
		Read,
		/// <summary>Abre para escritura</summary>
		Write
	}

    public StreamUnionModel(IFileManager fileManager, OpenMode mode, string fileName, Stream stream)
    {
		FileManager = fileManager;
		Mode = mode;
		FileName = fileName;
		Stream = stream;
    }

	/// <summary>
	///		Cierra los streams abiertos
	/// </summary>
	public void Close()
	{
		Stream?.Close();
	}

	/// <summary>
	///		Libera la memoria
	/// </summary>
	protected virtual void Dispose(bool disposing)
	{
		if (!IsDisposed)
		{
			// Si tiene que liberar la memoria, cierra los streams
			if (disposing)
				Close();
			// Indica que se ha liberado la memoria
			IsDisposed = true;
		}
	}

	/// <summary>
	///		Libera la memoria
	/// </summary>
	void IDisposable.Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	///		Manager de archivos
	/// </summary>
	public IFileManager FileManager { get; }

	/// <summary>
	///		Modo de apertura
	/// </summary>
	public OpenMode Mode { get; }

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	public string FileName { get; }

	/// <summary>
	///		Stream sobre el archivo
	/// </summary>
	public Stream Stream { get; }

	/// <summary>
	///		Indica si se ha liberado la memoria
	/// </summary>
	public bool IsDisposed { get; private set; }
}
