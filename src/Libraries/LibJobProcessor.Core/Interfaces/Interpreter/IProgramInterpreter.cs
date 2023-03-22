using Bau.Libraries.LibInterpreter.Interpreter.Context.Variables;
using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibInterpreter.Models.Symbols;
using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;

namespace Bau.Libraries.LibJobProcessor.Core.Interfaces.Interpreter;

/// <summary>
///		Interface del intérprete principal
/// </summary>
public interface IProgramInterpreter
{
    /// <summary>
    ///		Procesa una serie de sentencias
    /// </summary>
    Task ProcessAsync(List<SentenceBase> sentences, CancellationToken cancellationToken);

    /// <summary>
    ///		Procesa una serie de sentencias creando un contexto nuevo
    /// </summary>
    Task ProcessWithContextAsync(List<SentenceBase> sentences, CancellationToken cancellationToken);

    /// <summary>
    ///		Obtiene los contextos del proyecto
    /// </summary>
    IEnumerable<JobContextModel> GetContexts();

    /// <summary>
    ///		Obtiene un contexto
    /// </summary>
    JobContextModel? GetContext(string key);

    /// <summary>
    ///		Añade un nuevo contexto de ejecución (un ámbito superior)
    /// </summary>
    void AddScope();

    /// <summary>
    ///		Quita el último contexto de ejecución (el ámbito superior)
    /// </summary>
    void RemoveScope();

    /// <summary>
    ///		Añade una variable al contexto
    /// </summary>
    VariableModel AddVariable(string key, SymbolModel.SymbolType type, object? value);

	/// <summary>
	///		Elimina una variable de la tabla
	/// </summary>
	void RemoveVariable(string key);

    /// <summary>
    ///		Obtiene el valor de una variable
    /// </summary>
    object? GetVariableValue(string key);

    /// <summary>
    ///		Obtiene el valor de una variable
    /// </summary>
    VariableModel? GetVariable(string key);

    /// <summary>
    ///		Obtiene un diccionario con las variables obtenidas recursivamente por todos los ámbitos
    /// </summary>
    Dictionary<string, VariableModel> GetVariablesRecursive();

    /// <summary>
    ///		Obtiene el nombre de archivo completo
    /// </summary>
    string GetFullFileName(string provider, string source, string fileName);

    /// <summary>
    ///		Aplica los parámetros a una cadena
    /// </summary>
    string ApplyVariables(string? value);

    /// <summary>
    ///		Añade información
    /// </summary>
    void LogInfo(string message);

    /// <summary>
    ///		Añade información de depuración
    /// </summary>
    void LogDebug(string message);

    /// <summary>
    ///		Añade advertencia
    /// </summary>
    void LogWarning(string message);

    /// <summary>
    ///		Añade un error
    /// </summary>
    void AddError(string error, Exception? exception = null);

    /// <summary>
    ///		Comprueba si hay algún error
    /// </summary>
    bool CheckHasError();
}
