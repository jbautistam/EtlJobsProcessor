using Bau.Libraries.LibJobProcessor.Core.Interfaces;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Interpreter;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Repository;

namespace Bau.Libraries.LibJobProcessor.Database;

/// <summary>
///		Procesador de trabajos de base de datos
/// </summary>
public class JobDatabaseManager : IJobProcessor
{
	/// <summary>
	///		Obtiene el repositorio de sentencias
	/// </summary>
	public IProcessorRepository GetRepository(IProgramRepository programRepository) => new Repository.DbScriptRepository(programRepository);

	/// <summary>
	///		Obtiene el repositorio del contexto
	/// </summary>
	public IContextRepository GetContextRepository() => new Repository.DbContextRepository();

	/// <summary>
	///		Obtiene el intérprete
	/// </summary>
	public IJobSentenceIntepreter GetInterpreter(IProgramInterpreter programInterpreter) => new Interpreter.DbScriptInterpreter(programInterpreter);
}
