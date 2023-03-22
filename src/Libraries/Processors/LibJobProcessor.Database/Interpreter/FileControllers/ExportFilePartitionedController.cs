using Bau.Libraries.DbAggregator.Models;
using Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Files;
using Bau.Libraries.LibJobProcessor.Database.Interpreter.FileControllers.Storage;

namespace Bau.Libraries.LibJobProcessor.Database.Interpreter.FileControllers;

/// <summary>
///		Controlador para exportar datos de una consulta SQL a una serie de archivos CSV particionados
/// </summary>
internal class ExportFilePartitionedController : BaseFileController
{
    internal ExportFilePartitionedController(DbScriptInterpreter processor) : base(processor) { }

    /// <summary>
    ///		Procesa una exportación de una consulta a CSV
    /// </summary>
    internal async Task<bool> ExecuteAsync(SentenceFileExportPartitioned sentence, CancellationToken cancellationToken)
    {
        bool exported = false;

        // Log
        Processor.LogDebug($"Start exporting partitioned to {Path.GetFileName(sentence.FileName)}");
        // Exporta los datos
        if (string.IsNullOrWhiteSpace(sentence.Command.Sql))
            Processor.AddError("There is not command at export sentence");
        else
        {
            ProviderModel? provider = Processor.GetProvider(sentence.Source);
            string baseFileName = Processor.GetFullFileName(sentence.FileName);

            if (provider == null)
                Processor.AddError($"Can't find the provider. Key: '{sentence.Source}'");
            else
            {
                string container = Processor.ApplyVariables(sentence.Container);
                CommandModel command = Processor.ConvertProviderCommand(sentence.Command, out string error);

                if (!string.IsNullOrWhiteSpace(error))
                    Processor.AddError($"Error when convert export command. {error}");
                else
                    try
                    {
                        // Exporta los datos
                        using (BaseFileStorage fileManager = GetStorageManager(Processor, sentence, container))
                        {
                            // Abre la conexión
                            fileManager.Open();
                            // Exporta los datos
                            await GetFileImplementation(sentence).ExportPartitionedAsync(fileManager, provider, command, sentence, baseFileName,
                                                                                            cancellationToken);
                        }
                        // Indica que se ha exportado correctamente
                        exported = true;
                    }
                    catch (Exception exception)
                    {
                        Processor.AddError($"Error when export '{sentence.FileName}'", exception);
                    }
            }
        }
        // Devuelve el valor que indica si se ha exportado correctamente
        return exported;
    }
}