namespace Bau.Libraries.LibJobProcessor.Cloud.Models.Sentences;

/// <summary>
///		Sentencia de descarga de un directorio de blobs de un contenedor
/// </summary>
internal class DownloadBlobFolderSentence : BaseBlobSentence
{
	internal DownloadBlobFolderSentence (string providerFile, string providerFileKey, string path) : base(providerFile, providerFileKey)
	{
		Path = path;
	}

	/// <summary>
	///		Directorio de destino
	/// </summary>
	internal string Path { get; }

	/// <summary>
	///		Blob origen
	/// </summary>
	internal BlobModel Source { get; } = new();
}
