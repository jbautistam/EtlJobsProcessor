namespace Bau.Libraries.LibJobProcessor.FilesStructured.Models.Sentences;

/// <summary>
///		Propiedades de un archivo Excel
/// </summary>
internal class ExcelProperties
{
	internal ExcelProperties(int sheetIndex, bool withHeader)
	{
		SheetIndex = sheetIndex;
		WithHeader = withHeader;
	}

	/// <summary>
	///		Indice de la hoja de los archivos Excel
	/// </summary>
	internal int SheetIndex { get; }

	/// <summary>
	///		Indica si los archivos Excel tienen cabecera
	/// </summary>
	internal bool WithHeader { get; }
}
