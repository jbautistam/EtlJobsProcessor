using Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Files;
using Bau.Libraries.LibJobProcessor.Database.Interpreter.FileControllers.Implementation;
using Bau.Libraries.LibJobProcessor.Database.Interpreter.FileControllers.Storage;

namespace Bau.Libraries.LibJobProcessor.Database.Interpreter.FileControllers;

/// <summary>
///		Controlador base para los archivos
/// </summary>
internal abstract class BaseFileController
{
    protected BaseFileController(DbScriptInterpreter processor)
    {
        Processor = processor;
    }

    /// <summary>
    ///		Obtiene la implementación de lectura / escritura de archivos
    /// </summary>
    protected BaseFileImplementation GetFileImplementation(SentenceFileBase sentence)
    {
        switch (sentence.Type)
        {
            case SentenceFileBase.FileType.Csv:
                return new CsvFileImplementation(Processor);
            case SentenceFileBase.FileType.Parquet:
                return new ParquetFileImplementation(Processor);
            case SentenceFileBase.FileType.Json:
                return new JsonFileImplementation(Processor);
            default:
                throw new NotImplementedException($"Unknown file type {sentence.Type.ToString()}");
        }
    }

    /// <summary>
    ///		Obtiene el manager adecuado para el storage
    /// </summary>
    protected BaseFileStorage GetStorageManager(DbScriptInterpreter processor, SentenceFileBase sentence, string container)
    {
        bool IsLocalSentence(string source, string container)
        {
            return string.IsNullOrWhiteSpace(source) && string.IsNullOrWhiteSpace(container);
        }

        switch (sentence)
        {
            case SentenceFileExport sentenceFile:
                if (IsLocalSentence(sentenceFile.Target, container))
                    return new LocalFileStorage(processor);
                else
                    return new CloudFileStorage(processor, sentenceFile.Target, container);
            case SentenceFileImport sentenceFile:
                if (IsLocalSentence(sentenceFile.Source, container))
                    return new LocalFileStorage(processor);
                else
                    return new CloudFileStorage(processor, sentenceFile.Source, container);
            default:
                throw new NotImplementedException($"Unknown file sentence: {sentence.GetType().ToString()}");
        }
    }

    /// <summary>
    ///		Manager
    /// </summary>
    protected DbScriptInterpreter Processor { get; }
}
