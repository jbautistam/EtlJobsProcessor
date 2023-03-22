using System.Data;

namespace Bau.Libraries.LibJobProcessor.FilesStructured.Interpreter.Controllers;

/// <summary>
///		Clase base para los controladores de archivos
/// </summary>
internal abstract class BaseFileController
{
	protected BaseFileController(ScriptInterpreter interpreter)
	{
		Interpreter = interpreter;
	}

	/// <summary>
	///		Abre un IDataReader
	/// </summary>
	internal abstract Task<IDataReader> OpenReaderAsync(Core.Models.Files.StreamUnionModel streamSource, CancellationToken cancellationToken);

	/// <summary>
	///		Escribe a un archivo
	/// </summary>
	internal abstract Task WriteToAsync(IDataReader reader, Core.Models.Files.StreamUnionModel streamTarget, CancellationToken cancellationToken);

	/// <summary>
	///		Intérprete
	/// </summary>
	internal ScriptInterpreter Interpreter { get; }
}
