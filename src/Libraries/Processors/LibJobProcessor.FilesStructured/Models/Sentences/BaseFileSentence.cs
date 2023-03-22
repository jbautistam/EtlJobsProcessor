namespace Bau.Libraries.LibJobProcessor.FilesStructured.Models.Sentences;

/// <summary>
///		Sentencia base para los archivos
/// </summary>
internal abstract class BaseFileSentence : LibInterpreter.Models.Sentences.SentenceBase
{
	/// <summary>
	///		Tipo de archivo
	/// </summary>
	internal enum FileType
	{
		/// <summary>Csv</summary>
		Csv,
		/// <summary>Parquet</summary>
		Parquet,
		/// <summary>Excel</summary>
		Excel
	}

	internal BaseFileSentence(FileType type)
	{
		Type = type;
	}

	/// <summary>
	///		Tipo de archivo
	/// </summary>
	internal FileType Type { get; }

	/// <summary>
	///		Columnas del archivo
	/// </summary>
	internal List<FileColumnModel> Columns { get; } = new();
}
