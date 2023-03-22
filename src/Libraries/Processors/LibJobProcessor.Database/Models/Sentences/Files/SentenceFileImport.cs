using Bau.Libraries.LibCsvFiles.Models;

namespace Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Files;

/// <summary>
///		Sentencia de importación a CSV
/// </summary>
internal class SentenceFileImport : SentenceFileBase
{
    internal SentenceFileImport(string table)
    {
        Table = table;
    }

    /// <summary>
    ///		Indica si se deben importar los archivos de la carpeta
    /// </summary>
    internal bool ImportFolder { get; set; }

    /// <summary>
    ///		Tabla sobre la que se van a importar los datos
    /// </summary>
    internal string Table { get; }

    /// <summary>
    ///		Indica si la importación de este archivo es obligatoria
    /// </summary>
    internal bool Required { get; set; } = true;

    /// <summary>
    ///		Mapeo de columnas. Clave: nombre de columna origen, Valor: nombre de columna destino
    /// </summary>
    internal Dictionary<string, string> Mappings { get; } = new();

    /// <summary>
    ///		Columnas del archivo
    /// </summary>
    internal List<ColumnModel> Columns { get; } = new();
}
