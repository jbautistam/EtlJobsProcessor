namespace Bau.Libraries.LibJobProcessor.Cloud.Models.Sentences;

/// <summary>
///		Sentencia de descarga de un blob
/// </summary>
internal class DownloadBlobSentence : BaseBlobSentence
{
	internal DownloadBlobSentence(string providerFile, string providerFileKey, string fileName) : base(providerFile, providerFileKey)
	{
		FileName = fileName;
	}

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	internal string FileName { get; }

	/// <summary>
	///		Blob origen
	/// </summary>
	internal BlobModel Source { get; } = new();
}
