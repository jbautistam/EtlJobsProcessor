using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Parameters;

namespace Bau.Libraries.LibJobProcessor.Database.Models.Sentences;

/// <summary>
///		Sentencia de ejecución de una comprobación de datos
/// </summary>
internal class SentenceIfExists : SentenceBase
{
    internal SentenceIfExists(string source, SqlSentenceModel command)
    {
        Source = source;
        Command = command;
    }

    /// <summary>
    ///		Proveedor del que se leen los datos
    /// </summary>
    internal string Source { get; }

    /// <summary>
    ///		Comando a ejecutar
    /// </summary>
    internal SqlSentenceModel Command { get; }

    /// <summary>
    ///		Sentencias a ejecutar si existe el registro
    /// </summary>
    internal SentenceCollection SentencesThen { get; } = new();

    /// <summary>
    ///		Sentencias a ejecutar si no existe el registro
    /// </summary>
    internal SentenceCollection SentencesElse { get; } = new();
}
