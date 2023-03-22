using Bau.Libraries.LibDbProviders.Base.Schema;
using Bau.Libraries.DbAggregator.Models;
using Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Files;

namespace Bau.Libraries.LibJobProcessor.Database.Interpreter.FileControllers;

/// <summary>
///		Procesador de las sentencias de importación de archivos de todo el esquema de base de datos
/// </summary>
internal class ImportFileSchemaController : BaseFileController
{
    internal ImportFileSchemaController(DbScriptInterpreter processor) : base(processor) { }

    /// <summary>
    ///		Procesa una importación de archivos CSV para las tablas de esquema de base de datos
    /// </summary>
    internal async Task<bool> ExecuteAsync(SentenceFileImportSchema sentence, CancellationToken cancellationToken)
    {
        ProviderModel? provider = Processor.GetProvider(sentence.Target);
        bool imported = false;

            // Log
            Processor.LogInfo($"Start importing tables from {sentence.Target}");
            // Importa los archivos
            if (provider is null)
                Processor.AddError($"Can't find the provider '{sentence.Target}'");
            else
            {
                // Supone que todo se importa correctamente
                imported = true;
                // Ejecuta las sentencias de importación obtenidas a partir del esquema y los archivos
                foreach (SentenceFileImport importSentence in GetImportFileSentences(provider, sentence))
                    if (imported && !cancellationToken.IsCancellationRequested)
                    {
                        // Log
                        Processor.LogInfo($"Importing '{importSentence.FileName}' in '{importSentence.Table}'");
                        // Importa el archivo
                        imported = await new ImportFileController(Processor).ExecuteAsync(importSentence, cancellationToken);
                    }
            }
            // Devuelve el valor que indica si se ha importado
            return imported;
    }

    /// <summary>
    ///		Obtiene los comandos para importación de los archivos asociados a las tablas
    /// </summary>
    private List<SentenceFileImport> GetImportFileSentences(ProviderModel provider, SentenceFileImportSchema sentence)
    {
        List<SentenceFileImport> sentences = new List<SentenceFileImport>();

            // Obtiene las sentencias
            foreach (TableDbModel table in provider.LoadSchema().Tables)
                if (sentence.ExcludeRules.CheckMustExclude(table.Name))
                    Processor.LogInfo($"Skip table {table.Name} because is excluded");
                else
                    sentences.Add(CreateSentence(sentence, table));
            // Devuelve la colección de instrucciones
            return sentences;
    }

    /// <summary>
    ///		Crea una sentencia de importación de una tabla con los datos de un archivo
    /// </summary>
    private SentenceFileImport CreateSentence(SentenceFileImportSchema sentence, TableDbModel table)
    {
        SentenceFileImport importSentence = new SentenceFileImport(table.Name);

            // Asigna las propiedades
            importSentence.Type = importSentence.Type;
            importSentence.Source = sentence.Source;
            importSentence.Target = sentence.Target;
            importSentence.Container = importSentence.Container;
            importSentence.FileName = Path.Combine(Processor.ProgramInterpreter.GetFullFileName(sentence.FileName) ?? string.Empty, table.Name + sentence.GetExtension());
            importSentence.BatchSize = sentence.BatchSize;
            importSentence.Timeout = sentence.Timeout;
            // Asigna los parámetros de archivo
            importSentence.Definition = sentence.Definition;
            // Devuelve la sentencia de importación
            return importSentence;
    }
}
