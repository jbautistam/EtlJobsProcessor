namespace Bau.Libraries.LibJobProcessor.FilesStructured.Models.Sentences;

/// <summary>
///		Sentencia para escribir datos sobre un archivo Parquet
/// </summary>
internal class WriteToParquetSentence: BaseWriteToSentence
{
	internal WriteToParquetSentence(string source, string target) : base(source, target, BaseOpenFileReaderSentence.FileType.Parquet)
	{
	}
}
