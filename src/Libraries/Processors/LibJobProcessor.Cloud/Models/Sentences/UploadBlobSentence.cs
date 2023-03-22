namespace Bau.Libraries.LibJobProcessor.Cloud.Models.Sentences;

/// <summary>
///		Sentencia para subir un archivo a un blob
/// </summary>
internal class UploadBlobSentence : BaseBlobSentence
{
	internal UploadBlobSentence(string providerFile, string providerFileKey, string fileName) : base(providerFile, providerFileKey)
	{
		FileName = fileName;
	}

	/// <summary>
	///		Nombre de archivo
	/// </summary>
	internal string FileName { get; }

	/// <summary>
	///		Blob destino
	/// </summary>
	internal BlobModel Target { get; } = new();
}
