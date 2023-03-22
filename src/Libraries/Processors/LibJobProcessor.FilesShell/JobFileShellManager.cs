using Bau.Libraries.LibJobProcessor.Core.Interfaces;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Interpreter;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Repository;

namespace Bau.Libraries.LibJobProcessor.FilesShell;

/// <summary>
///		Procesador de trabajos de sistema operativo: copia de archivos, borrado, ejecución de procesos...
/// </summary>
public class JobFileShellManager : IJobProcessor
{
	/// <summary>
	///		Obtiene el repositorio
	/// </summary>
	public IProcessorRepository GetRepository(IProgramRepository programRepository) => new Repository.JobFilesShellRepository(programRepository);

	/// <summary>
	///		Obtiene el repositorio de contextos
	/// </summary>
	public IContextRepository GetContextRepository() => new Repository.JobContextRepository();

	/// <summary>
	///		Obtiene el intérprete
	/// </summary>
	public IJobSentenceIntepreter GetInterpreter(IProgramInterpreter programInterpreter) => new Interpreter.ScriptInterpreter(programInterpreter);
}