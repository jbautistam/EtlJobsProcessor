namespace Bau.Libraries.LibJobProcessor.FilesStructured.Models.Sentences;

/// <summary>
///		Sentencia para escribir datos sobre un Excel
/// </summary>
internal class WriteToExcelSentence : BaseWriteToSentence
{
	internal WriteToExcelSentence(string source, string target, ExcelProperties properties) : base(source, target, BaseOpenFileReaderSentence.FileType.Excel)
	{
		Properties = properties;
	}

	/// <summary>
	///		Propiedades del Excel
	/// </summary>
	internal ExcelProperties Properties { get; }
}
