namespace Bau.Libraries.LibJobProcessor.FilesStructured.Models.Sentences;

/// <summary>
///		Sentencia para abrir un reader sobre un archivo Excel
/// </summary>
internal class OpenExcelReaderSentence : BaseOpenFileReaderSentence
{
	internal OpenExcelReaderSentence(string name, string source, ExcelProperties properties) : base(name, source, FileType.Excel)
	{
		Properties = properties;
	}

	/// <summary>
	///		Propiedades del archivo Excel
	/// </summary>
	internal ExcelProperties Properties { get; }
}
