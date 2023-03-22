using Bau.Libraries.LibJobProcessor.FilesStructured.Models.Sentences;

namespace Bau.Libraries.LibJobProcessor.FilesStructured.Interpreter.Controllers;

/// <summary>
///		Controlador para archivos Excel
/// </summary>
internal class ExcelController : BaseFileController
{
	internal ExcelController(ScriptInterpreter interpreter, OpenExcelReaderSentence? sentenceRead, WriteToExcelSentence? sentenceWrite) : base(interpreter) 
	{
		SentenceRead = sentenceRead ?? default!;
		SentenceWrite = sentenceWrite ?? default!;
	}
 
	/// <summary>
	///		Abre un IDataReader
	/// </summary>
	internal override async Task<System.Data.IDataReader> OpenReaderAsync(Core.Models.Files.StreamUnionModel streamUnion, CancellationToken cancellationToken)
	{
		LibExcelFiles.Data.ExcelReader reader = new(SentenceRead.Properties.WithHeader);

			// Evita las advertencias
			await Task.Delay(1, cancellationToken);
			// Abre el archivo
			reader.Open(streamUnion.Stream, SentenceRead.Properties.SheetIndex);
			// Devuelve el lector sobre el archivo
			return reader;
	}

	/// <summary>
	///		Escribe a un archivo
	/// </summary>
	internal override async Task WriteToAsync(System.Data.IDataReader reader, Core.Models.Files.StreamUnionModel streamTarget, CancellationToken cancellationToken)
	{
		// Evita las advertencias
		await Task.Delay(1, cancellationToken);
		// Lanza una excepción porque esta librería necesita una plantilla para poder escribir
		throw new NotImplementedException("Not implemented. Need a template stream");
/*
		LibExcelFiles.Data.ExcelWriter writer = new(SentenceWrite.Properties.WithHeader);

			// Evita las advertencias
			await Task.Delay(1, cancellationToken);
			// Escribe los datos
			writer.Write(streamTarget.Stream, SentenceWrite.Properties.SheetIndex, reader);
*/
	}

	/// <summary>
	///		Datos de la sentencia de lectura
	/// </summary>
	internal OpenExcelReaderSentence SentenceRead { get; }

	/// <summary>
	///		Datos de la sentencia de escritura
	/// </summary>
	internal WriteToExcelSentence SentenceWrite { get; }
}
