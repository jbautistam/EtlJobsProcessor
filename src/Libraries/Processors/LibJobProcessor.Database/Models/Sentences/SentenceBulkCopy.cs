using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Parameters;

namespace Bau.Libraries.LibJobProcessor.Database.Models.Sentences;

/// <summary>
///		Sentencia de copia masiva
/// </summary>
internal class SentenceBulkCopy : SentenceBase
{
    internal SentenceBulkCopy(string source, string target, string table, SqlSentenceModel command)
    {
        Source = source;
        Target = target;
        Table = table;
        Command = command;
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
    ///		Tabla sobre la que se realiza la inserción
    /// </summary>
    internal string Table { get; }

    /// <summary>
    ///		Mapeo de columnas. Clave: nombre de columna origen, Valor: nombre de columna destino
    /// </summary>
    internal Dictionary<string, string> Mappings { get; } = new();

    /// <summary>
    ///		Comando a ejecutar
    /// </summary>
    internal SqlSentenceModel Command { get; }

    /// <summary>
    ///		Tamaño del lote de escritura
    /// </summary>
    internal int BatchSize { get; set; } = 30_000;
}
