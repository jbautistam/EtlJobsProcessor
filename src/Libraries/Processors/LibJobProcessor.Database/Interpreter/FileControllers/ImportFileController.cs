using Bau.Libraries.DbAggregator.Models;
using Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Files;
using Bau.Libraries.LibJobProcessor.Database.Interpreter.FileControllers.Storage;

namespace Bau.Libraries.LibJobProcessor.Database.Interpreter.FileControllers;

/// <summary>
///		Controlador para importar los datos de archivos a las bases de datos
/// </summary>
internal class ImportFileController : BaseFileController
{
    internal ImportFileController(DbScriptInterpreter processor) : base(processor)
    {
    }

    /// <summary>
    ///		Procesa una importación de un archivo
    /// </summary>
    internal async Task<bool> ExecuteAsync(SentenceFileImport sentence, CancellationToken cancellationToken)
    {
        ProviderModel? provider = Processor.GetProvider(sentence.Target);
        string? fileName = Processor.GetFullFileName(sentence.FileName);
        string? container = Processor.ApplyVariables(sentence.Container);
        bool imported = false;

            // Log
            Processor.LogInfo($"Start import {sentence.FileName}");
            // Compruba los datos e importa
            if (string.IsNullOrWhiteSpace(fileName))
                Processor.AddError("The file name / folder is undefined");
            else if (string.IsNullOrWhiteSpace(sentence.Source))
                Processor.AddError("The source is undefined");
            else if (string.IsNullOrWhiteSpace(container))
                Processor.AddError("The container is undefined");
            else if (provider is null)
                Processor.AddError($"Can't find the provider. Key: '{sentence.Target}'");
            else
                try
                {
                    // Importa el archivo
                    await ImportAsync(provider, sentence, fileName, container, cancellationToken);
                    // Indica que se ha ejecutado correctamente
                    imported = true;
                }
                catch (Exception exception)
                {
                    Processor.AddError($"Error when import '{fileName}'", exception);
                }
            // Devuelve el valor que indica si se ha importado
            return imported;
    }

    /// <summary>
    ///		Importa los datos del archivo o directorio sobre el proveedor
    /// </summary>
    private async Task ImportAsync(ProviderModel provider, SentenceFileImport sentence, string fileName, string container, CancellationToken cancellationToken)
    {
        using (BaseFileStorage fileManager = GetStorageManager(Processor, sentence, container))
        {
            // Abre la conexión
            fileManager.Open();
            // Importa los archivos
            await foreach (string file in fileManager.GetFilesAsync(fileName, sentence.ImportFolder, sentence.GetExtension()))
                if (!cancellationToken.IsCancellationRequested)
                {
                    // Log
                    Processor.LogInfo($"Importing {Path.GetFileName(file)}");
                    // Importa el archivo
                    await GetFileImplementation(sentence).ImportAsync(provider, sentence,
                                                                      await fileManager.GetStreamAsync(file, BaseFileStorage.OpenFileMode.Read),
                                                                      cancellationToken);
                }
        }
    }
}
