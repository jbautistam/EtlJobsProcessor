namespace Bau.Libraries.LibJobProcessor.FilesStructured.Models.Sentences;

/// <summary>
///		Sentencia para abrir un reader sobre un archivo Parquet
/// </summary>
internal class OpenParquetReaderSentence : BaseOpenFileReaderSentence
{
	internal OpenParquetReaderSentence(string name, string source) : base(name, source, BaseOpenFileReaderSentence.FileType.Parquet)
	{
	}
}
