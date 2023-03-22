using Bau.Libraries.LibJobProcessor.FilesStructured.Models.Sentences;

namespace Bau.Libraries.LibJobProcessor.FilesStructured.Interpreter.Controllers;

/// <summary>
///		Controlador para archivos Excel
/// </summary>
internal class ParquetController : BaseFileController
{
	internal ParquetController(ScriptInterpreter interpreter, OpenParquetReaderSentence? sentenceRead, WriteToParquetSentence? sentenceWrite) : base(interpreter) 
	{
		SentenceRead = sentenceRead ?? default!;
		SentenceWrite = sentenceWrite ?? default!;
	}
 
	/// <summary>
	///		Abre un IDataReader
	/// </summary>
	internal override async Task<System.Data.IDataReader> OpenReaderAsync(Core.Models.Files.StreamUnionModel streamUnion, CancellationToken cancellationToken)
	{
		LibParquetFiles.Readers.ParquetDataReader reader = new();

			// Abre el archivo
			if (streamUnion.Stream is null)
				throw new ArgumentNullException("The stream reader is null");
			else
				await reader.OpenAsync(streamUnion.Stream, cancellationToken);
			// Devuelve el lector
			return reader;
	}

	/// <summary>
	///		Escribe a un archivo
	/// </summary>
	internal override async Task WriteToAsync(System.Data.IDataReader reader, Core.Models.Files.StreamUnionModel streamTarget, CancellationToken cancellationToken)
	{
		if (streamTarget.Stream is null)
			Interpreter.AddError($"Error when write '{SentenceWrite.Source}' to '{SentenceWrite.Target}'. Stream writer is not open");
		else
			try
			{
				LibParquetFiles.Writers.ParquetDataWriter writer = new LibParquetFiles.Writers.ParquetDataWriter(200_000);

					// Escribe sobre el archivo de salida
					await writer.WriteAsync(streamTarget.Stream, reader, cancellationToken);
			}
			catch (Exception exception)
			{
				Interpreter.AddError($"Error when write '{SentenceWrite.Source}' to '{SentenceWrite.Target}'", exception);
			}
	}

	/// <summary>
	///		Datos de la sentencia
	/// </summary>
	internal OpenParquetReaderSentence SentenceRead { get; }

	/// <summary>
	///		Datos de la sentencia de escritura
	/// </summary>
	internal WriteToParquetSentence SentenceWrite { get; }
}
