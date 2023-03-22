using Bau.Libraries.DbAggregator.Models;
using Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Files;
using Bau.Libraries.LibJobProcessor.Database.Interpreter.FileControllers.Storage;

namespace Bau.Libraries.LibJobProcessor.Database.Interpreter.FileControllers;

/// <summary>
///		Controlador para exportar datos de una consulta SQL a un archivo
/// </summary>
internal class ExportFileController : BaseFileController
{
    internal ExportFileController(DbScriptInterpreter processor) : base(processor) { }

    /// <summary>
    ///		Procesa una exportación de una consulta a un archivo
    /// </summary>
    internal async Task<bool> ExecuteAsync(SentenceFileExport sentence, CancellationToken cancellationToken)
    {
        bool exported = false;
        string? fileName = Processor.GetFullFileName(sentence.FileName);

            // Log
            Processor.LogInfo($"Start exporting to '{fileName}'");
            // Exporta los datos
            if (string.IsNullOrWhiteSpace(sentence.Command.Sql))
                Processor.AddError("There is not command at export sentence");
            else if (string.IsNullOrWhiteSpace(fileName))
                Processor.AddError("The file name is undefined");
            else if (!string.IsNullOrWhiteSpace(sentence.Target) && string.IsNullOrWhiteSpace(sentence.Container))
                Processor.AddError("It's defined target but is not defined a container");
            else if (string.IsNullOrWhiteSpace(sentence.Target) && !string.IsNullOrWhiteSpace(sentence.Container))
                Processor.AddError("It's defined a container but is not defined target");
            else
            {
                string? container = Processor.ApplyVariables(sentence.Container);
                ProviderModel? provider = Processor.GetProvider(sentence.Source);

                    if (provider is null)
                        Processor.AddError($"Can't find the provider. Key: '{sentence.Source}'");
                    else
                    {
                        CommandModel command = Processor.ConvertProviderCommand(sentence.Command, out string error);

                        if (!string.IsNullOrWhiteSpace(error))
                            Processor.AddError($"Error when convert export command. {error}");
                        else
                            try
                            {
                                // Exporta al archivo
                                using (BaseFileStorage fileManager = GetStorageManager(Processor, sentence, container))
                                {
                                    // Abre la conexión
                                    fileManager.Open();
                                    // Crea el directorio
                                    await fileManager.CreatePathAsync(Path.GetDirectoryName(fileName));
                                    // Exporta los datos
                                    using (Stream stream = await fileManager.GetStreamAsync(fileName, BaseFileStorage.OpenFileMode.Write))
                                    {
                                        await GetFileImplementation(sentence).ExportAsync(stream, provider, command, sentence, cancellationToken);
                                    }
                                    // Indica que se ha exportado correctamente
                                    exported = true;
                                }
                            }
                            catch (Exception exception)
                            {
                                Processor.AddError($"Error when export to '{fileName}'", exception);
                            }
                    }
            }
            // Devuelve el valor que indica si se ha exportado correctamente
            return exported;
    }
}
