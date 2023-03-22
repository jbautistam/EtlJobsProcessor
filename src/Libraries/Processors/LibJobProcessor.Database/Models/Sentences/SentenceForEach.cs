using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Parameters;

namespace Bau.Libraries.LibJobProcessor.Database.Models.Sentences;

/// <summary>
///		Sentencia de ejecución de un bucle por cada registro leido del proveedor
/// </summary>
internal class SentenceForEach : SentenceBase
{
    internal SentenceForEach(string source, SqlSentenceModel command)
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
    ///		Instrucciones que se deben ejecutar cuando se encuentra algún dato
    /// </summary>
    internal SentenceCollection SentencesWithData { get; } = new();

    /// <summary>
    ///		Instrucciones que se deben ejecutar cuando no se encuentra ningún dato
    /// </summary>
    internal SentenceCollection SentencesEmptyData { get; } = new();
}