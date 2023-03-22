using Bau.Libraries.LibJobProcessor.Core.Interfaces;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Interpreter;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Repository;

namespace Bau.Libraries.LibJobProcessor.Cloud;

/// <summary>
///		Procesador de trabajos de base de datos
/// </summary>
public class JobCloudManager : IJobProcessor
{
	/// <summary>
	///		Obtiene el repositorio
	/// </summary>
	public IProcessorRepository GetRepository(IProgramRepository programRepository) => new Repository.JobCloudRepository(programRepository);

	/// <summary>
	///		Obtiene el repositorio de contexto
	/// </summary>
	public IContextRepository GetContextRepository() => new Repository.JobCloudContextRepository();

	/// <summary>
	///		Obtiene el intérprete
	/// </summary>
	public IJobSentenceIntepreter GetInterpreter(IProgramInterpreter programInterpreter) => new Manager.ScriptInterpreter(programInterpreter);
}