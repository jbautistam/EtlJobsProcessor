using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibJobProcessor.Cloud.Models.Sentences;
using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Repository;

namespace Bau.Libraries.LibJobProcessor.Cloud.Repository;

/// <summary>
///		Clase para cargar datos de proceso
/// </summary>
internal class JobCloudRepository : BaseJobRepository
{
	// Constantes privadas
	private const string TagProviderFile = "ProviderFile";
	private const string TagProviderFileKey = "ProviderFileKey";
	//private const string TagSource = "Source";
	//private const string TagTarget = "Target";
	private const string TagEnabled = "Enabled";
	private const string TagUploadBlob = "Upload";
	private const string TagDownloadBlob = "Download";
	private const string TagBlobContainer = "Container";
	private const string TagBlobFile = "Blob";
	private const string TagFileName = "FileName";
	private const string TagCopy = "Copy";
	private const string TagTransformFileName = "TransformFileName";
	private const string TagMove = "Move";
	private const string TagFrom = "From";
	private const string TagTo = "To";
	private const string TagDownloadPath = "DownloadPath";
	private const string TagPath = "Path";
	private const string TagStoragePath = "StoragePath";

	public JobCloudRepository(IProgramRepository programRepository) : base(programRepository)
	{
	}

	/// <summary>
	///		Carga la sentencias de un nodo
	/// </summary>
	public override SentenceBase? Load(MLNode rootML)
	{
		return rootML.Name switch
		{
			TagUploadBlob => LoadUploadBlobSentence(rootML),
			TagDownloadBlob => LoadDownloadBlobSentence(rootML),
			TagDownloadPath => LoadDownloadPathSentence(rootML),
			TagCopy => LoadCopyBlobSentence(rootML, false),
			TagMove => LoadCopyBlobSentence(rootML, true),
			_ => null,
		};
	}

	/// <summary>
	///		Carga la sentencia para subir un archivo
	/// </summary>
	private BaseBlobSentence LoadUploadBlobSentence(MLNode rootML)
	{
		UploadBlobSentence sentence = new UploadBlobSentence(rootML.Attributes[TagProviderFile].Value.TrimIgnoreNull(),
															 rootML.Attributes[TagProviderFileKey].Value.TrimIgnoreNull(),
															 rootML.Attributes[TagFileName].Value.TrimIgnoreNull());

			// Carga los datos de la sentencia
			AssignBlobSentences(sentence, rootML);
			sentence.Target.Container = rootML.Attributes[TagBlobContainer].Value;
			sentence.Target.Blob = rootML.Attributes[TagBlobFile].Value;
			// Devuelve la sentencia
			return sentence;
	}

	/// <summary>
	///		Carga la sentencia para descargar un archivo
	/// </summary>
	private BaseBlobSentence LoadDownloadBlobSentence(MLNode rootML)
	{
		DownloadBlobSentence sentence = new DownloadBlobSentence(rootML.Attributes[TagProviderFile].Value.TrimIgnoreNull(),
																 rootML.Attributes[TagProviderFileKey].Value.TrimIgnoreNull(),
																 rootML.Attributes[TagFileName].Value.TrimIgnoreNull());

			// Carga los datos de la sentencia
			AssignBlobSentences(sentence, rootML);
			sentence.Source.Container = rootML.Attributes[TagBlobContainer].Value;
			sentence.Source.Blob = rootML.Attributes[TagBlobFile].Value;
			// Devuelve la sentencia
			return sentence;
	}

	/// <summary>
	///		Carga la sentencia para descargar un directorio del storage
	/// </summary>
	private BaseBlobSentence LoadDownloadPathSentence(MLNode rootML)
	{
		DownloadBlobFolderSentence sentence = new DownloadBlobFolderSentence(rootML.Attributes[TagProviderFile].Value.TrimIgnoreNull(),
																			 rootML.Attributes[TagProviderFileKey].Value.TrimIgnoreNull(),
																			 rootML.Attributes[TagPath].Value.TrimIgnoreNull());

			// Carga los datos de la sentencia
			AssignBlobSentences(sentence, rootML);
			sentence.Source.Container = rootML.Attributes[TagBlobContainer].Value;
			sentence.Source.Blob = rootML.Attributes[TagStoragePath].Value;
			// Devuelve la sentencia
			return sentence;
	}

	/// <summary>
	///		Carga una sentencia de copia entre blobs
	/// </summary>
	private BaseBlobSentence LoadCopyBlobSentence(MLNode rootML, bool move)
	{
		CopyBlobSentence sentence = new CopyBlobSentence(rootML.Attributes[TagProviderFile].Value.TrimIgnoreNull(),
														 rootML.Attributes[TagProviderFileKey].Value.TrimIgnoreNull());

			// Carga los datos básicos de la sentencia
			AssignBlobSentences(sentence, rootML);
			// Carga el resto de datos
			sentence.Move = move;
			sentence.TransformFileName = rootML.Attributes[TagTransformFileName].Value.GetBool();
			foreach (MLNode nodeML in rootML.Nodes)
				switch (nodeML.Name)
				{
					case TagFrom:
							sentence.Source.Container = nodeML.Attributes[TagBlobContainer].Value;
							sentence.Source.Blob = nodeML.Attributes[TagBlobFile].Value;
						break;
					case TagTo:
							sentence.Target.Container = nodeML.Attributes[TagBlobContainer].Value;
							sentence.Target.Blob = nodeML.Attributes[TagBlobFile].Value;
						break;
				}
			// Devuelve la sentencia
			return sentence;
	}

	/// <summary>
	///		Asigna los datos de una sentencia asociada con un blob
	/// </summary>
	private void AssignBlobSentences(BaseBlobSentence sentence, MLNode rootML)
	{
		sentence.Enabled = rootML.Attributes[TagEnabled].Value.GetBool(true);
		sentence.Timeout = ProgramRepository.GetTimeout(rootML, TimeSpan.FromMinutes(5));
	}
}
