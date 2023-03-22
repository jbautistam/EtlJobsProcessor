using System.Data;

using Bau.Libraries.DbAggregator.Models;
using Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Files;
using Bau.Libraries.LibJobProcessor.Database.Interpreter.FileControllers.Storage;
using Bau.Libraries.LibJsonFiles;

namespace Bau.Libraries.LibJobProcessor.Database.Interpreter.FileControllers.Implementation;

/// <summary>
///		Implementación de la importación / exportación de archivos json
/// </summary>
internal class JsonFileImplementation : BaseFileImplementation
{
    internal JsonFileImplementation(DbScriptInterpreter processor) : base(processor) { }

    /// <summary>
    ///		Importa un archivo
    /// </summary>
    internal async override Task ImportAsync(ProviderModel provider, SentenceFileImport sentence, Stream stream, CancellationToken cancellation)
    {
        long records = 0;

            // Evita las advertencias
            await Task.Delay(1);
            // Copia del origen al destino
            using (StreamReader streamReader = new StreamReader(stream))
            {
                using (JsonReader reader = new JsonReader(sentence.BatchSize))
                {
                    // Asigna el manejador de eventos
                    reader.Progress += (sender, args) => Processor.LogInfo($"Importing {args.Records:#,##0}");
                    // Abre el archivo de entrada
                    reader.Open(streamReader);
                    // Copia los datos del archivo sobre la tabla
                    records = provider.BulkCopy(reader, sentence.Table, sentence.Mappings, sentence.BatchSize, sentence.Timeout);
                }
            }
            // Log
            Processor.LogInfo($"Imported {records:#,##0} records");
    }

    /// <summary>
    ///		Exporta un archivo
    /// </summary>
    internal async override Task ExportAsync(Stream stream, ProviderModel provider, CommandModel command, SentenceFileExport sentence,
                                                CancellationToken cancellation)
    {
        using (IDataReader reader = provider.OpenReader(command, sentence.Timeout))
        {
            using (StreamWriter streamWriter = new StreamWriter(stream))
            {
                using (JsonWriter writer = new JsonWriter())
                {
                    // Evita las advertencias
                    await Task.Delay(1);
                    // Abre el archivo
                    writer.Open(streamWriter);
                    // Graba las líneas
                    while (reader.Read())
                    {
                        // Graba los valores
                        writer.WriteRow(GetValues(reader));
                        // Lanza el evento de progreso
                        if (writer.Rows % sentence.BatchSize == 0)
                            Processor.LogInfo($"Copying {writer.Rows:#,##0}");
                    }
                    // Log
                    Processor.LogInfo($"Exported {writer.Rows:#,##0} records");
                }
            }
        }
    }

    /// <summary>
    ///		Obtiene el diccionario de valores a partir de un registro
    /// </summary>
    private Dictionary<string, object> GetValues(IDataReader reader)
    {
        Dictionary<string, object> values = new();

            // Añade los nombres de columnas por las cabeceras
            for (int index = 0; index < reader.FieldCount; index++)
                values.Add(reader.GetName(index), reader.GetValue(index));
            // Devuelve la lista de valores
            return values;
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
}