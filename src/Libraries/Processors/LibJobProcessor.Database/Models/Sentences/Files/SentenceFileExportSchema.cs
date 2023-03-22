namespace Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Files;

/// <summary>
///		Sentencia de exportación de todas las tablas de un esquema a archivos CSV
/// </summary>
internal class SentenceFileExportSchema : SentenceFileBase
{
    /// <summary>
    ///		Indica si se deben borrar los archivos antiguos
    /// </summary>
    internal bool DeleteOldFiles { get; set; }

    /// <summary>
    ///		Reglas de exclusión de tablas 
    /// </summary>
    internal SchemaExcludeRuleCollection ExcludeRules { get; } = new();
}
