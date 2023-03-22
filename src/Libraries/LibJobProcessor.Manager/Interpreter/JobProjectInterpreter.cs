using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

using Bau.Libraries.LibInterpreter.Interpreter;
using Bau.Libraries.LibInterpreter.Interpreter.Context.Functions;
using Bau.Libraries.LibInterpreter.Interpreter.Context.Variables;
using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibJobProcessor.Core.Interfaces;
using Bau.Libraries.LibJobProcessor.Core.Models;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Interpreter;
using Bau.Libraries.LibJobProcessor.Manager.Models.Sentences;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.FilesManager;
using Bau.Libraries.LibInterpreter.Models.Symbols;
using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;

namespace Bau.Libraries.LibJobProcessor.Manager.Interpreter;

/// <summary>
///		Intérprete del proyecto
/// </summary>
internal class JobProjectInterpreter : ProgramProcessor, IProgramInterpreter
{
	internal JobProjectInterpreter(JobProjectManager manager, JobProjectModel project) : base(new ProcessorOptions())
	{
		Manager = manager;
		Project = project;
		ExpressionConversor = new(this);
	}

	/// <summary>
	///		Procesa el programa
	/// </summary>
	internal async Task ProcessAsync(CancellationToken cancellationToken)
	{
		// Inicializa el contexto
		Initialize(null);
		// Carga los intérpretes
		LoadInterpreters();
		// Añade los parámetros y las constantes a la tabla de variables del intérprete
		foreach (JobParameterModel parameter in Project.ProjectExecutionContext.Parameters)
			AddVariable(parameter.Key, Convert(parameter.Type), parameter.Value);
		// Añade las variables fijas
		AddVariable("ProjectWorkPath", SymbolModel.SymbolType.String, Project.ProjectExecutionContext.ProjectWorkPath);
		AddVariable("ProjectStartExecution", SymbolModel.SymbolType.Date, Project.ProjectExecutionContext.StartExecution);
		// Inicializa los intérpretes (después de añadir las variables al contexto)
		await IntializaInterpretersAsync(cancellationToken);
		// Interpreta las instrucciones del programa
		try
		{
			await ProcessAsync(Project.Program, cancellationToken);
		}
		catch (Exception exception)
		{
			AddError($"Error when process program. {exception.Message}", exception);
		}

		// Convierte el tipo
		SymbolModel.SymbolType Convert(JobParameterModel.ParameterType type)
		{
			return type switch
							{
								JobParameterModel.ParameterType.Boolean => SymbolModel.SymbolType.Boolean,
								JobParameterModel.ParameterType.String => SymbolModel.SymbolType.String,
								JobParameterModel.ParameterType.DateTime => SymbolModel.SymbolType.Date,
								JobParameterModel.ParameterType.Numeric => SymbolModel.SymbolType.Numeric,
								_ => SymbolModel.SymbolType.Object
							};
		}
	}

	/// <summary>
	///		Carga los intérpretes
	/// </summary>
	private void LoadInterpreters()
	{
		foreach (IJobProcessor processor in Manager.Processors)
			Interpreters.Add(processor.GetInterpreter(this));
	}

	/// <summary>
	///		Inicializa los intérpretes
	/// </summary>
	private async Task IntializaInterpretersAsync(CancellationToken cancellationToken)
	{
		foreach (IJobSentenceIntepreter interpreter in Interpreters)
			await interpreter.InitializeAsync(cancellationToken);
	}

	/// <summary>
	///		Ejecuta una sentencia
	/// </summary>
	protected async override Task ExecuteAsync(SentenceBase abstractSentence, CancellationToken cancellationToken)
	{
		switch (abstractSentence)
		{
			case SentenceBlock sentence:
					await ProcessBlockAsync(sentence, cancellationToken);
				break;
			case SentenceUsingFile sentence:
					await ProcessUsingFileAsync(sentence, cancellationToken);
				break;
			default:
					await ProcessExternalAsync(abstractSentence, cancellationToken);
				break;
		}
	}

	/// <summary>
	///		Ejecuta una función implícita
	/// </summary>
	protected async override Task<VariableModel?> ExecuteAsync(ImplicitFunctionModel function, CancellationToken cancellationToken)
	{
		await Task.Delay(1, cancellationToken);
		return null;
	}

	/// <summary>
	///		Procesa una serie de sentencias
	/// </summary>
	public async Task ProcessAsync(List<SentenceBase> sentences, CancellationToken cancellationToken)
	{
		await ExecuteAsync(sentences, cancellationToken);
	}

	/// <summary>
	///		Ejecuta una serie de sentencias creando un contexto nuevo
	/// </summary>
	public async Task ProcessWithContextAsync(List<SentenceBase> sentences, CancellationToken cancellationToken)
	{
		await ExecuteWithContextAsync(sentences, cancellationToken);
	}

	/// <summary>
	///		Procesa una sentencia en un intérprete externo
	/// </summary>
	private async Task ProcessExternalAsync(SentenceBase sentenceBase, CancellationToken cancellationToken)
	{
		bool executed = false;

			// Ejecuta el proceso en uno de los intérpretes externos
			foreach (IJobSentenceIntepreter interpreter in Interpreters)
				if (!Stopped && !executed)
					executed = await interpreter.ProcessAsync(sentenceBase, cancellationToken);
			// Si no se ha ejecutado, lanza el error
			if (!executed)
				AddError($"Sentence undefined: {sentenceBase.ToString()}");
	}

	/// <summary>
	///		Procesa un bloque de sentencias
	/// </summary>
	private async Task ProcessBlockAsync(SentenceBlock sentence, CancellationToken cancellationToken)
	{
		// Log
		LogInfo(sentence.GetMessage("Start block"));
		// Ejecuta las sentencias
		await ProcessAsync(sentence.Sentences, cancellationToken);
	}

	/// <summary>
	///		Procesa la sentencia de utilización de un archivo
	/// </summary>
	private async Task ProcessUsingFileAsync(SentenceUsingFile sentence, CancellationToken cancellationToken)
	{
		IFileManager? fileManager = GetFileManager(sentence.ProviderFile);

			// Log
			LogInfo($"Start using file {sentence.FileName} {sentence.Mode.ToString()}");
			// Abre el archivo
			if (fileManager is null)
				AddError($"Can't find the file provider for {sentence.ProviderFile}");
			else
			{
				string fileName = fileManager.GetFullFileName(this, sentence.ProviderFileKey, sentence.FileName);

					if (string.IsNullOrWhiteSpace(fileName))
						AddError($"There is no file name at sentence");
					else
						using (Core.Models.Files.StreamUnionModel stream = await fileManager.OpenAsync(sentence.ProviderFileKey, fileName, 
																									   sentence.Mode, cancellationToken))
						{
							VariableModel variable = AddVariable(sentence.Key, SymbolModel.SymbolType.Object, stream);

								// Ejecuta las sentencias hijo
								await ProcessAsync(sentence.Sentences, cancellationToken);
								// Quita la variable de memoria
								RemoveVariable(variable.Name);
								// Log
								LogInfo($"End using file {sentence.FileName}");
						}
			}
	}

	/// <summary>
	///		Obtiene el manager de archivos
	/// </summary>
	private IFileManager? GetFileManager(string provider)
	{
		// Obtiene el manager de archivos
		foreach (IFileManager fileManager in Manager.FilesManagers)
			if (fileManager.Key.Equals(provider, StringComparison.CurrentCultureIgnoreCase))
				return fileManager;
		// Si ha llagedo hasta aquí es porque no ha encontrado nada
		return null;
	}

	/// <summary>
	///		Obtiene los contextos del proyecto
	/// </summary>
	public IEnumerable<JobContextModel> GetContexts()
	{
		return Project.ProjectExecutionContext.Contexts;
	}

	/// <summary>
	///		Obtiene un contexto
	/// </summary>
	public JobContextModel? GetContext(string key)
	{
		return Project.ProjectExecutionContext.GetContext(key);
	}

    /// <summary>
    ///		Obtiene el nombre de archivo completo
    /// </summary>
    public string GetFullFileName(string provider, string source, string fileName)
	{
		IFileManager? fileManager = GetFileManager(provider);

			if (fileManager is not null)
				return fileManager.GetFullFileName(this, source, fileName);
			else
				throw new Exceptions.JobProcessorException($"Can't find the file provider {provider} for {fileName}");
	}

	/// <summary>
	///		Aplica los parámetros
	/// </summary>
	public string ApplyVariables(string? value)
	{
		int applied = 0;

			// Convierte las variables
			while (!string.IsNullOrWhiteSpace(value) && value.Contains("{{") && value.Contains("}}") && applied < 50)
			{
				// Aplica las variables
				foreach ((string key, VariableModel variable) in Context.Actual.GetVariablesRecursive().Enumerate())
					value = value.Replace("{{" + key + "}}", variable.GetStringValue(), StringComparison.CurrentCultureIgnoreCase);
				// Indica que se ha aplicado una vez más
				applied++;
			}
			// Devuelve el valor resultante
			return value ?? string.Empty;
	}

	/// <summary>
	///		Interpreta una cadena de expresión
	/// </summary>
	protected override LibInterpreter.Models.Expressions.ExpressionsCollection ParseExpression(string expression)
	{
		return ExpressionConversor.ParseExpression(expression);
	}

	/// <summary>
	///		Añade un nuevo contexto de ejecución (un ámbito superior)
	/// </summary>
	public void AddScope()
	{ 
		Context.Add();
	}
	
	/// <summary>
	///		Quita el último contexto de ejecución (el ámbito superior)
	/// </summary>
	public void RemoveScope()
	{ 
		Context.Pop();
	}

	/// <summary>
	///		Añade una variable al contexto
	/// </summary>
	public VariableModel AddVariable(string key, SymbolModel.SymbolType type, object? value)
	{
		return Context.Actual.VariablesTable.Add(key, type, value);
	}

	/// <summary>
	///		Elimina una variable de la tabla
	/// </summary>
	public void RemoveVariable(string key)
	{
		Context.Actual.VariablesTable.Remove(key);
	}

	/// <summary>
	///		Obtiene el valor de una variable
	/// </summary>
	public object? GetVariableValue(string key)
	{
		return Context.Actual.VariablesTable.Get(key)?.Value;
	}

	/// <summary>
	///		Obtiene una variable
	/// </summary>
	public VariableModel? GetVariable(string key)
	{
		return Context.Actual.VariablesTable.Get(key);
	}

	/// <summary>
	///		Obtiene un diccionario con las variables obtenidas recursivamente por todos los ámbitos
	/// </summary>
	public Dictionary<string, VariableModel> GetVariablesRecursive()
	{
		return Context.Actual.GetVariablesRecursive().GetAll();
	}

	/// <summary>
	///		Formatea una cadena (aplica las variables)
	/// </summary>
	protected override string FormatString(string value)
	{
		return ApplyVariables(value);
	}

	/// <summary>
	///		Añade un mensaje de depuración
	/// </summary>
	protected override void AddDebug(string message, [CallerFilePath] string? fileName = null, 
									 [CallerMemberName] string? methodName = null, [CallerLineNumber] int lineNumber = 0)
	{
		LogDebug(message);
	}

	/// <summary>
	///		Añade un mensaje informativo
	/// </summary>
	protected override void AddInfo(string message, [CallerFilePath] string? fileName = null, 
									[CallerMemberName] string? methodName = null, [CallerLineNumber] int lineNumber = 0)
	{
		LogInfo(message);
	}

	/// <summary>
	///		Añade un mensaje a la consola de salida
	/// </summary>
	protected override void AddConsoleOutput(string message, [CallerFilePath] string? fileName = null, 
											 [CallerMemberName] string? methodName = null, [CallerLineNumber] int lineNumber = 0)
	{
		LogInfo(message);
	}

	/// <summary>
	///		Añade un error y detiene la compilación si es necesario
	/// </summary>
	protected override void AddError(string error, Exception? exception = null, [CallerFilePath] string? fileName = null, 
									 [CallerMemberName] string? methodName = null, [CallerLineNumber] int lineNumber = 0)
	{
		AddError(error, exception);
	}

	/// <summary>
	///		Añade información de depuración
	/// </summary>
	public void LogDebug(string message)
	{
		Manager.Logger.LogDebug(message);
	}

	/// <summary>
	///		Añade información
	/// </summary>
	public void LogInfo(string message)
	{
		Manager.Logger.LogInformation(message);
	}

	/// <summary>
	///		Añade advertencia
	/// </summary>
	public void LogWarning(string message)
	{
		Manager.Logger.LogWarning(message);
	}

	/// <summary>
	///		Añade un error
	/// </summary>
	public void AddError(string error, Exception? exception = null)
	{
		// Añade el error a la lista y al log
		Manager.Errors.Add(error);
		Manager.Logger.LogError(exception, error);
		// Indica que no puede continuar
		Stopped = true;
	}

	/// <summary>
	///		Comprueba si hay algún error
	/// </summary>
	public bool CheckHasError()
	{
		return Manager.HasError;
	}

	/// <summary>
	///		Manager principal
	/// </summary>
	internal JobProjectManager Manager { get; }

	/// <summary>
	///		Datos del proyecto
	/// </summary>
	internal JobProjectModel Project { get; }

	/// <summary>
	///		Colección de intérpretes
	/// </summary>
	internal List<IJobSentenceIntepreter> Interpreters { get; } = new();

	/// <summary>
	///		Conversor de expresiones
	/// </summary>
	private ExpressionConversor ExpressionConversor { get; }
}
