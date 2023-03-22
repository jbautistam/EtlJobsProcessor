using Bau.Libraries.DbAggregator.Models;
using Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Files;
using Bau.Libraries.LibJobProcessor.Database.Interpreter.FileControllers.Storage;
using Bau.Libraries.LibParquetFiles.Readers;
using Bau.Libraries.LibParquetFiles.Writers;

namespace Bau.Libraries.LibJobProcessor.Database.Interpreter.FileControllers.Implementation;

/// <summary>
///		Implementación de la importación / exportación de archivos parquet
/// </summary>
internal class ParquetFileImplementation : BaseFileImplementation
{
    internal ParquetFileImplementation(DbScriptInterpreter processor) : base(processor) { }

    /// <summary>
    ///		Importa un archivo
    /// </summary>
    internal async override Task ImportAsync(ProviderModel provider, SentenceFileImport sentence, Stream stream, CancellationToken cancellationToken)
    {
        long records = 0;

            // Copia del origen al destino
            using (ParquetDataReader reader = new ParquetDataReader(sentence.BatchSize))
            {
                // Asigna el manejador de eventos
                reader.Progress += (sender, args) => Processor.LogInfo($"Importing {args.Records:#,##0}");
                // Abre el archivo de entrada
                await reader.OpenAsync(stream, cancellationToken);
                // Copia los datos del archivo sobre la tabla
                records = provider.BulkCopy(reader, sentence.Table, sentence.Mappings, sentence.BatchSize, sentence.Timeout);
            }
            // Log
            Processor.LogInfo($"Imported {records:#,##0} records");
    }

    /// <summary>
    ///		Exporta un archivo
    /// </summary>
    internal async override Task ExportAsync(Stream stream, ProviderModel provider, CommandModel command, SentenceFileExport sentence,
                                             CancellationToken cancellationToken)
    {
        using (System.Data.IDataReader reader = provider.OpenReader(command, sentence.Timeout))
        {
            long records = 0;

                // Escribe en el archivo
                records = await GetDataWriter(sentence.BatchSize).WriteAsync(stream, reader, cancellationToken);
                // Log
                Processor.LogInfo($"Exported {records:#,##0} records");
        }
    }

    /// <summary>
    ///		Exporta un archivo particionado
    /// </summary>
    internal override async Task ExportPartitionedAsync(BaseFileStorage fileManager, ProviderModel provider, CommandModel command,
                                                        SentenceFileExportPartitioned sentence, string baseFileName, CancellationToken cancellationToken)
    {
        // Evita los warnings
        await Task.Delay(1);
        // Lanza una excepción
        throw new NotImplementedException();
    }

    /// <summary>
    ///		Obtiene el generador de archivos parquet
    /// </summary>
    private ParquetDataWriter GetDataWriter(int recordsPerBlock)
    {
        ParquetDataWriter writer = new ParquetDataWriter(recordsPerBlock);

            // Asigna el manejador de eventos
            writer.Progress += (sender, args) => Processor.LogInfo($"Writing to file {args.Records:#,##0}");
            // Devuelve el generador
            return writer;
    }
}
