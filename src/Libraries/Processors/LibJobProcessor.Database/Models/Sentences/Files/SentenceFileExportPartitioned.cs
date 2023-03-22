namespace Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Files;

internal class SentenceFileExportPartitioned : SentenceFileBase
{
    /// <summary>
    ///		Comando de carga de datos para exportar
    /// </summary>
    internal Parameters.SqlSentenceModel? Command { get; set; }

    /// <summary>
    ///		Separador de partición
    /// </summary>
    internal string PartitionSeparator { get; set; } = "_#_";

    /// <summary>
    ///		Columnas de la partición
    /// </summary>
    internal List<string> PartitionColumns { get; } = new();
}
