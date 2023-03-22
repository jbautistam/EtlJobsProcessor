using Bau.Libraries.LibInterpreter.Models.Sentences;

namespace Bau.Libraries.LibJobProcessor.Core.Interfaces.Interpreter;

/// <summary>
///		Base para los intérpretes de sentencias
/// </summary>
public abstract class BaseJobSentenceIntepreter : IJobSentenceIntepreter
{
    protected BaseJobSentenceIntepreter(IProgramInterpreter programInterpreter)
    {
        ProgramInterpreter = programInterpreter;
    }

    /// <summary>
    ///		Inicializa el intérprete
    /// </summary>
    public abstract Task InitializeAsync(CancellationToken cancellationToken);

    /// <summary>
    ///		Procesa la sentencia
    /// </summary>
    public abstract Task<bool> ProcessAsync(SentenceBase sentenceBase, CancellationToken cancellationToken);

    /// <summary>
    ///		Procesa una serie de sentencias
    /// </summary>
    protected async Task ProcessAsync(List<SentenceBase> sentences, CancellationToken cancellationToken)
    {
        await ProgramInterpreter.ProcessAsync(sentences, cancellationToken);
    }

    /// <summary>
    ///		Procesa una serie de sentencias añadiendo un contexto de variables nuevo
    /// </summary>
    protected async Task ProcessWithContextAsync(List<SentenceBase> sentences, CancellationToken cancellationToken)
    {
        await ProgramInterpreter.ProcessWithContextAsync(sentences, cancellationToken);
    }

    /// <summary>
    ///		Obtiene el nombre completo de un archivo
    /// </summary>
    public string GetFullFileName(string provider, string source, string fileName) => ProgramInterpreter.GetFullFileName(provider, source, fileName);

    /// <summary>
    ///		Obtiene un contexto
    /// </summary>
    public Models.Jobs.JobContextModel? GetContext(string key) => ProgramInterpreter.GetContext(key);

    /// <summary>
    ///		Aplica los parámetros a un valor
    /// </summary>
    public string ApplyVariables(string? value) => ProgramInterpreter.ApplyVariables(value);

    /// <summary>
    ///		Añade información
    /// </summary>
    public void LogInfo(string message)
    {
        ProgramInterpreter.LogInfo(message);
    }

    /// <summary>
    ///		Añade información de depuración
    /// </summary>
    public void LogDebug(string message)
    {
        ProgramInterpreter.LogDebug(message);
    }

    /// <summary>
    ///		Añade advertencia
    /// </summary>
    public void LogWarning(string message)
    {
        ProgramInterpreter.LogWarning(message);
    }

    /// <summary>
    ///		Añade un error
    /// </summary>
    public void AddError(string error, Exception? exception = null)
    {
        ProgramInterpreter.AddError(error, exception);
    }

    /// <summary>
    ///		Comprueba si hay algún error en la aplicación
    /// </summary>
    public bool CheckHasError() => ProgramInterpreter.CheckHasError();

    /// <summary>
    ///		Intérprete principal
    /// </summary>
    public IProgramInterpreter ProgramInterpreter { get; }
}