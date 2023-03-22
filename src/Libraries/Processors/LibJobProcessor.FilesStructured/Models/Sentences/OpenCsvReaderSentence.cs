namespace Bau.Libraries.LibJobProcessor.FilesStructured.Models.Sentences;

/// <summary>
///		Sentencia para abrir un reader sobre un archivo CSV
/// </summary>
internal class OpenCsvReaderSentence : BaseOpenFileReaderSentence
{
	internal OpenCsvReaderSentence(string name, string source, CsvProperties properties) : base(name, source, BaseOpenFileReaderSentence.FileType.Csv)
	{
		Properties = properties;
	}

	/// <summary>
	///		Propiedades del CSV
	/// </summary>
	internal CsvProperties Properties { get; }
}
