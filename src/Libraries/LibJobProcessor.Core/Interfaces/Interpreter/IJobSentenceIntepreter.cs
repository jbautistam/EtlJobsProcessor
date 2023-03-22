using Bau.Libraries.LibInterpreter.Models.Sentences;

namespace Bau.Libraries.LibJobProcessor.Core.Interfaces.Interpreter;

/// <summary>
///		Interface para los intérpretes de sentencias
/// </summary>
public interface IJobSentenceIntepreter
{
    /// <summary>
    ///		Inicializa el intérprete
    /// </summary>
    Task InitializeAsync(CancellationToken cancellationToken);

    /// <summary>
    ///		Procesa una sentencia. Devuelve un valor que indica si se ha procesado
    /// </summary>
    Task<bool> ProcessAsync(SentenceBase sentence, CancellationToken cancellationToken);

    /// <summary>
    ///		Intérprete principal
    /// </summary>
    IProgramInterpreter ProgramInterpreter { get; }
}
