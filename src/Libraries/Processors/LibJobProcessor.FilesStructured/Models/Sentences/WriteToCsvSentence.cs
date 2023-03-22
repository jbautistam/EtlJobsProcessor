namespace Bau.Libraries.LibJobProcessor.FilesStructured.Models.Sentences;

/// <summary>
///		Sentencia para escribir datos sobre un CSV
/// </summary>
internal class WriteToCsvSentence : BaseWriteToSentence
{
	internal WriteToCsvSentence(string source, string target, CsvProperties properties) : base(source, target, BaseOpenFileReaderSentence.FileType.Csv)
	{
		Properties = properties;
	}

	/// <summary>
	///		Propiedades del CSV
	/// </summary>
	internal CsvProperties Properties { get; }
}
