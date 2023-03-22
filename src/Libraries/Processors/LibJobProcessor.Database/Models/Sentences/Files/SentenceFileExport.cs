namespace Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Files;

/// <summary>
///		Sentencia de exportación a archivo
/// </summary>
internal class SentenceFileExport : SentenceFileBase
{
    /// <summary>
    ///		Número de registros por grupo de filas
    /// </summary>
    internal int RowGroupSize { get; set; } = 45_000;

    /// <summary>
    ///		Comando de carga de datos para exportar
    /// </summary>
    internal Parameters.SqlSentenceModel? Command { get; set; }

    /// <summary>
    ///		Indica si los archivos están en el storage
    /// </summary>
    internal bool FilesAtStorage
    {
        get { return !string.IsNullOrWhiteSpace(Target) && !string.IsNullOrWhiteSpace(Container); }
    }
}
