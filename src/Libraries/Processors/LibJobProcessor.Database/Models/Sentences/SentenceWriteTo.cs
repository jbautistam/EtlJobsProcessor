using Bau.Libraries.LibInterpreter.Models.Sentences;

namespace Bau.Libraries.LibJobProcessor.Database.Models.Sentences;

/// <summary>
///		Sentencia de escritura de un IDataReader sobre una tabla
/// </summary>
internal class SentenceWriteTo : SentenceBase
{
    internal SentenceWriteTo(string source, string target, string table, int batchSize, TimeSpan timeout)
    {
        Source = source;
        Target = target;
        Table = table;
        BatchSize = batchSize;
        Timeout = timeout;
    }

    /// <summary>
    ///     Nombre de la variable donde está el lector origen de los datos
    /// </summary>
    internal string Source { get; }

    /// <summary>
    ///		Nombre del proveedor donde se van a grabar los datos
    /// </summary>
    internal string Target { get; }

    /// <summary>
    ///		Tabla en la que se van a copiar los datos
    /// </summary>
    internal string Table { get; }

    /// <summary>
    ///		Número de registros por bloque
    /// </summary>
    internal int BatchSize { get; }

    /// <summary>
    ///		Mapeo de columnas. Clave: nombre de columna origen, Valor: nombre de columna destino
    /// </summary>
    internal Dictionary<string, string> Mappings { get; } = new();

    /// <summary>
    ///     Tiempo máximo de la copia de datos
    /// </summary>
    internal TimeSpan Timeout { get; }
}
