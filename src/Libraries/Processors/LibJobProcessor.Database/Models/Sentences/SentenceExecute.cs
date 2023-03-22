using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Parameters;

namespace Bau.Libraries.LibJobProcessor.Database.Models.Sentences;

/// <summary>
///		Sentencia de ejecución de un comando
/// </summary>
internal class SentenceExecute : SentenceBase
{
    internal SentenceExecute(string target, SqlSentenceModel command)
    {
        Target = target;
        Command = command;
    }

    /// <summary>
    ///		Proveedor sobre el que se ejecuta la sentencia
    /// </summary>
    internal string Target { get; }

    /// <summary>
    ///		Comando a ejecutar
    /// </summary>
    internal SqlSentenceModel Command { get; }
}
