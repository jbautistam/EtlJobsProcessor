using Bau.Libraries.LibJobProcessor.Core.Interfaces;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Interpreter;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Repository;

namespace Bau.Libraries.LibJobProcessor.FilesStructured;

/// <summary>
///		Procesador de trabajos con archivos estructurados: CSV, Parquet, Excel
/// </summary>
public class JobFileStructuredManager : IJobProcessor
{
	/// <summary>
	///		Obtiene el repositorio
	/// </summary>
	public IProcessorRepository GetRepository(IProgramRepository programRepository) => new Repository.JobFilesStructuredRepository(programRepository);

	/// <summary>
	///		Obtiene el repositorio de contextos
	/// </summary>
	public IContextRepository GetContextRepository() => new Repository.JobContextRepository();

	/// <summary>
	///		Obtiene el intérprete
	/// </summary>
	public IJobSentenceIntepreter GetInterpreter(IProgramInterpreter programInterpreter) => new Interpreter.ScriptInterpreter(programInterpreter);
}