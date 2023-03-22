namespace Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Files;

/// <summary>
///		Sentencia de exportación de todas las tablas de un esquema a archivos CSV
/// </summary>
internal class SentenceFileImportSchema : SentenceFileBase
{
    /// <summary>
    ///		Reglas de exclusión de tablas 
    /// </summary>
    internal SchemaExcludeRuleCollection ExcludeRules { get; } = new();
}
