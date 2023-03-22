using Bau.Libraries.LibJobProcessor.Core.Interfaces;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Interpreter;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Repository;

namespace Bau.Libraries.LibJobProcessor.Rest;

/// <summary>
///		Procesador de trabajos sobre REST
/// </summary>
public class JobRestManager : IJobProcessor
{
	/// <summary>
	///		Obtiene el procesador
	/// </summary>
	public IProcessorRepository GetRepository(IProgramRepository programRepository)
	{
		return new Repository.JobRestRepository(programRepository);
	}

	/// <summary>
	///		Obtiene el repositorio para los contextos
	/// </summary>
	public IContextRepository GetContextRepository()
	{
		return new Repository.JobContextRepository();
	}

	/// <summary>
	///		Obtiene el intérprete
	/// </summary>
	public IJobSentenceIntepreter GetInterpreter(IProgramInterpreter programInterpreter)
	{
		return new Manager.ScriptInterpreter(programInterpreter);
	}
}