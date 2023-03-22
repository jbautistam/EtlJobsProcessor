using Bau.Libraries.LibJobProcessor.Core.Interfaces.Interpreter;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Repository;

namespace Bau.Libraries.LibJobProcessor.Core.Interfaces;

/// <summary>
///		Interface para los procesadores de trabajo
/// </summary>
public interface IJobProcessor
{
	/// <summary>
	///		Obtiene el repositorio del procesador
	/// </summary>
	IProcessorRepository GetRepository(IProgramRepository programRepository);

	/// <summary>
	///		Obtiene el repositorio de contexto
	/// </summary>
	IContextRepository GetContextRepository();

	/// <summary>
	///		Obtiene el intérprete de sentencias
	/// </summary>
	IJobSentenceIntepreter GetInterpreter(IProgramInterpreter programInterpreter);
}