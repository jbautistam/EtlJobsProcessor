using Bau.Libraries.DbAggregator.Models;
using Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Files;
using Bau.Libraries.LibJobProcessor.Database.Interpreter.FileControllers.Storage;

namespace Bau.Libraries.LibJobProcessor.Database.Interpreter.FileControllers.Implementation;

/// <summary>
///		Implementación base para la importación / exportación de archivos a base de datos
/// </summary>
internal abstract class BaseFileImplementation
{
	internal BaseFileImplementation(DbScriptInterpreter processor)
	{
		Processor = processor;
	}

	/// <summary>
	///		Importa un archivo en base de datos
	/// </summary>
	internal abstract Task ImportAsync(ProviderModel provider, SentenceFileImport sentence, Stream stream, CancellationToken cancellationToken);

	/// <summary>
	///		Exporta un archivo a una tabla base de datos
	/// </summary>
	internal abstract Task ExportAsync(Stream stream, ProviderModel provider, CommandModel command,
									   SentenceFileExport sentence, CancellationToken cancellationToken);

	/// <summary>
	///		Exporta datos de un comando a un archivo particionado
	/// </summary>
	internal abstract Task ExportPartitionedAsync(BaseFileStorage fileManager, ProviderModel provider, CommandModel command,
												  SentenceFileExportPartitioned sentence, string baseFileName, CancellationToken cancellationToken);

	/// <summary>
	///		Procesador
	/// </summary>
	internal DbScriptInterpreter Processor { get; }
}
