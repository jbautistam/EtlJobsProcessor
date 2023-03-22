using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Parameters;

namespace Bau.Libraries.LibJobProcessor.Database.Models.Sentences;

/// <summary>
///		Sentencia de copia de datos
/// </summary>
internal class SentenceCopy : SentenceBase
{
    internal SentenceCopy(string source, string target, SqlSentenceModel loadCommand, SqlSentenceModel saveCommand)
    {
        Source = source;
        Target = target;
        LoadCommand = loadCommand;
        SaveCommand = saveCommand;
    }

    /// <summary>
    ///		Proveedor del que se leen los datos
    /// </summary>
    internal string Source { get; }

    /// <summary>
    ///		Proveedor sobre el que se ejecuta la sentencia
    /// </summary>
    internal string Target { get; }

    /// <summary>
    ///		Comando de carga
    /// </summary>
    internal SqlSentenceModel LoadCommand { get; }

    /// <summary>
    ///		Comando de grabación
    /// </summary>
    internal SqlSentenceModel SaveCommand { get; }
}
