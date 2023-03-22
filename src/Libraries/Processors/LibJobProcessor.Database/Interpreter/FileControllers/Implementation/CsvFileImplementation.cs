using System.Data;

using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.DbAggregator.Models;
using Bau.Libraries.LibCsvFiles;
using Bau.Libraries.LibCsvFiles.Models;
using Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Files;
using Bau.Libraries.LibJobProcessor.Database.Interpreter.FileControllers.Storage;

namespace Bau.Libraries.LibJobProcessor.Database.Interpreter.FileControllers.Implementation;

/// <summary>
///		Implementación de la importación / exportación de archivos CSV
/// </summary>
internal class CsvFileImplementation : BaseFileImplementation
{
    internal CsvFileImplementation(DbScriptInterpreter processor) : base(processor) { }

    /// <summary>
    ///		Importa un archivo
    /// </summary>
    internal async override Task ImportAsync(ProviderModel provider, SentenceFileImport sentence, Stream stream, CancellationToken cancellationToken)
    {
        long records = 0;

            // Evita las advertencias
            await Task.Delay(1);
            // Copia del origen al destino
            using (StreamReader streamReader = new StreamReader(stream))
            {
                using (CsvReader reader = new CsvReader(sentence.Definition, sentence.Columns, sentence.BatchSize))
                {
                    // Asigna el manejador de eventos
                    reader.ReadBlock += (sender, args) => Processor.LogInfo($"Importing {args.Records:#,##0}");
                    // Abre el archivo
                    reader.Open(streamReader);
                    // Copia los datos del archivo sobre la tabla
                    records = provider.BulkCopy(reader, sentence.Table, sentence.Mappings, sentence.BatchSize, sentence.Timeout);
                }
            }
            // Log
            Processor.LogInfo($"Imported {records:#,##0} records");
    }

    /// <summary>
    ///		Exporta un comando a un archivo
    /// </summary>
    internal async override Task ExportAsync(Stream stream, ProviderModel provider, CommandModel command,
                                             SentenceFileExport sentence, CancellationToken cancellationToken)
    {
        using (IDataReader reader = provider.OpenReader(command, sentence.Timeout))
        {
            long records = 0;

                // Evita las advertencias
                await Task.Delay(1);
                // Escribe en el archivo
                using (StreamWriter streamWriter = new StreamWriter(stream))
                {
                    using (CsvWriter writer = new CsvWriter(sentence.Definition))
                    {
                        List<ColumnModel> columns = GetColumns(reader);

                        // Abre el archivo
                        writer.Open(streamWriter);
                        // Añade las cabeceras
                        writer.WriteHeaders(GetHeaders(columns, sentence.Definition.TypedHeader));
                        // Graba las líneas
                        while (reader.Read())
                        {
                            // Graba los valores
                            writer.WriteRow(GetValues(columns, reader));
                            // Lanza el evento de progreso
                            if (++records % sentence.BatchSize == 0)
                                Processor.LogInfo($"Copying {records:#,##0}");
                        }
                        // Log
                        Processor.LogInfo($"Exported {records:#,##0} records");
                    }
                }
        }
    }

    /// <summary>
    ///		Exporta un archivo particionado
    /// </summary>
    internal override async Task ExportPartitionedAsync(BaseFileStorage fileManager, ProviderModel provider, CommandModel command,
                                                        SentenceFileExportPartitioned sentence, string baseFileName, CancellationToken cancellationToken)
    {
        using (IDataReader reader = provider.OpenReader(command, sentence.Timeout))
        {
            long records = 0;
            List<ColumnModel> headers = GetColumns(reader);
            string partitionKey = string.Empty;
            CsvWriter writer = default!;

                // Lee los registros y los va grabando en particiones
                while (reader.Read() && !cancellationToken.IsCancellationRequested)
                {
                    string actualPartition = GetPartitionKey(sentence.PartitionColumns, sentence.PartitionSeparator, reader);

                    // Cambia la partición
                    if (!actualPartition.EqualsIgnoreCase(partitionKey))
                    {
                        string fileName = GetFileName(baseFileName, sentence.PartitionSeparator, actualPartition);

                        // Log
                        Processor.LogInfo($"Opening the file: {fileName}");
                        // Cierra el archivo si ya existía
                        CloseFile(writer);
                        // Abre un nuevo archivo
                        writer = await OpenFileAsync(fileManager, sentence, headers, fileName);
                        // Cambia la clave de partición
                        partitionKey = actualPartition;
                    }
                    // Añade la línea
                    writer.WriteRow(GetValues(headers, reader));
                    // Lanza el evento de progreso
                    if (++records % sentence.BatchSize == 0)
                        Processor.LogInfo($"Copying {records:#,##0}");
                }
                // Cierra el archivo si estaba abierto
                CloseFile(writer);
                // Log
                Processor.LogInfo($"Exported {records:#,##0} records");
        }
    }

    /// <summary>
    ///		Abre un archivo
    /// </summary>
    private async Task<CsvWriter> OpenFileAsync(BaseFileStorage fileStorage, SentenceFileExportPartitioned sentence, List<ColumnModel> headers, string fileName)
    {
        CsvWriter writer = new CsvWriter(sentence.Definition);

            // Crea el directorio
            await fileStorage.CreatePathAsync(Path.GetDirectoryName(fileName));
            // Abre el archivo
            writer.Open(new StreamWriter(await fileStorage.GetStreamAsync(fileName, BaseFileStorage.OpenFileMode.Write)));
            // Escribe las cabeceras
            writer.WriteHeaders(GetHeaders(headers, sentence.Definition.TypedHeader));
            // Devuelve el archivo
            return writer;
    }

    /// <summary>
    ///		Obtiene el nombre de archivo
    /// </summary>
    private string GetFileName(string fileName, string separator, string partitionKey)
    {
        string? path = Path.GetDirectoryName(fileName);
        string result = Path.GetFileNameWithoutExtension(fileName) + separator + partitionKey + Path.GetExtension(fileName);

            // Agrega el directorio
            if (!string.IsNullOrWhiteSpace(path))
                result = Path.Combine(path, result);
            // Devuelve el nombre de archivo
            return result;
    }

    /// <summary>
    ///		Cierra un archivo si estaba abierto
    /// </summary>
    private void CloseFile(CsvWriter writer)
    {
        if (writer != null)
        {
            writer.Flush();
            writer.Close();
        }
    }

    /// <summary>
    ///		Obtiene la clave de partición
    /// </summary>
    private string GetPartitionKey(List<string> columns, string separator, IDataReader reader)
    {
        string partitionKey = string.Empty;

            // Obtiene la clave de partición
            foreach (string column in columns)
            {
                object result = reader[column];

                if (result is DBNull)
                    partitionKey = partitionKey.AddWithSeparator(string.Empty, separator, false);
                else
                    partitionKey = partitionKey.AddWithSeparator(result.ToString(), separator, false);
            }
            // Devuelve la clave de partición
            return partitionKey;
    }

    /// <summary>
    ///		Obtiene las columnas asociadas al <see cref="IDataReader"/>
    /// </summary>
    private List<ColumnModel> GetColumns(IDataReader reader)
    {
        List<ColumnModel> columns = new();
        DataTable? schema = reader.GetSchemaTable();

            // Obtiene las columnas del dataReader
            if (schema is not null)
                foreach (DataRow dataRow in schema.Rows)
                {
                    ColumnModel column = new();

                    // Busca las propiedades en las columnas
                    foreach (DataColumn readerColumn in schema.Columns)
                        if (readerColumn.ColumnName.Equals("ColumnName", StringComparison.CurrentCultureIgnoreCase))
                            column.Name = dataRow[readerColumn].ToString();
                        else if (readerColumn.ColumnName.Equals("DataType", StringComparison.CurrentCultureIgnoreCase))
                            column.Type = GetColumnType((Type)dataRow[readerColumn]);
                    // Añade la columna a la lista
                    columns.Add(column);
                }
            // Devuelve la colección de columnas
            return columns;
    }

    /// <summary>
    ///		Obtiene el tipo de columna
    /// </summary>
    private ColumnModel.ColumnType GetColumnType(Type dataType)
    {
        if (IsDataType(dataType, "byte[]")) // ... no vamos a convertir los arrays de bytes
            return ColumnModel.ColumnType.Unknown;
        else if (IsDataType(dataType, "int"))
            return ColumnModel.ColumnType.Integer;
        else if (IsDataType(dataType, "decimal") || IsDataType(dataType, "double") || IsDataType(dataType, "float"))
            return ColumnModel.ColumnType.Decimal;
        else if (IsDataType(dataType, "date"))
            return ColumnModel.ColumnType.DateTime;
        else if (IsDataType(dataType, "bool"))
            return ColumnModel.ColumnType.Boolean;
        else
            return ColumnModel.ColumnType.String;
    }

    /// <summary>
    ///		Comprueba si un nombre de tipo contiene un valor determinado
    /// </summary>
    private bool IsDataType(Type dataType, string search)
    {
        return dataType.FullName?.IndexOf("." + search, StringComparison.CurrentCultureIgnoreCase) >= 0;
    }

    /// <summary>
    ///		Obtiene las cabeceras a partir de las columnas
    /// </summary>
    private List<string> GetHeaders(List<ColumnModel> columns, bool typedHeader)
    {
        List<string> headers = new();

            // Añade los nombres de columnas por las cabeceras
            foreach (ColumnModel column in columns)
                if (typedHeader)
                    headers.Add($"{column.Name}|{column.Type.ToString()}");
                else
                    headers.Add(column.Name);
            // Devuelve la lista de cabeceras
            return headers;
    }

    /// <summary>
    ///		Obtiene los valores del <see cref="IDataReader"/>
    /// </summary>
    private List<(ColumnModel.ColumnType type, object value)> GetValues(List<ColumnModel> columns, IDataReader reader)
    {
        List<(ColumnModel.ColumnType type, object value)> values = new();

            // Añade los valores de la fila
            for (int index = 0; index < reader.FieldCount; index++)
                values.Add((columns[index].Type, reader.GetValue(index)));
            // Devuelve la colección de valores
            return values;
    }
}
