using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Parameters;

namespace Bau.Libraries.LibJobProcessor.Database.Models.Sentences;

/// <summary>
///		Sentencia de lectura de IDataReader
/// </summary>
internal class SentenceOpenReader : SentenceBase
{
    internal SentenceOpenReader(string name, string source, SqlSentenceModel command)
    {
        Name = name;
        Source = source;
        Command = command;
    }

    /// <summary>
    ///     Nombre de la variable donde se va a almacenar el lector
    /// </summary>
    internal string Name { get; }

    /// <summary>
    ///		Proveedor del que se leen los datos
    /// </summary>
    internal string Source { get; }

    /// <summary>
    ///		Comando a ejecutar
    /// </summary>
    internal SqlSentenceModel Command { get; }

    /// <summary>
    ///		Instrucciones a ejecutar dentro del lector
    /// </summary>
    internal SentenceCollection Sentences { get; } = new();
}
