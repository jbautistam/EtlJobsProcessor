namespace Bau.Libraries.LibJobProcessor.Cloud.Models.Sentences;

/// <summary>
///		Sentencia para copiar o mover blobs
/// </summary>
internal class CopyBlobSentence : BaseBlobSentence
{
	internal CopyBlobSentence(string providerFile, string providerFileKey) : base(providerFile, providerFileKey) {}

	/// <summary>
	///		Blob origen
	/// </summary>
	internal BlobModel Source { get; } = new BlobModel();

	/// <summary>
	///		Blob destino
	/// </summary>
	internal BlobModel Target { get; } = new BlobModel();

	/// <summary>
	///		Indica si se debe mover el archivo
	/// </summary>
	internal bool Move { get; set; }

	/// <summary>
	///		Indica si se debe cambiar el nombre de archivo destino por un nombre de archivo único
	/// </summary>
	internal bool TransformFileName { get; set; }
}
