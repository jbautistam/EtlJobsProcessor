using Microsoft.Extensions.Logging;

using Bau.Libraries.LibJobProcessor.Core.Interfaces;
using Bau.Libraries.LibJobProcessor.Core.Models;
using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;

namespace Bau.Libraries.LibJobProcessor.Manager;

/// <summary>
///		Manager de proyectos de trabajos
/// </summary>
public class JobProjectManager
{
	public JobProjectManager(ILogger logger, Models.ConfigurationModel configuration)
	{
		Logger = logger;
		Configuration = configuration;
	}

	/// <summary>
	///		Añade un procesador
	/// </summary>
	public void AddProcessor(IJobProcessor processor)
	{
		Processors.Add(processor);
	}

	/// <summary>
	///		Añade un administrador de archivos
	/// </summary>
	public void AddFileManager(Core.Interfaces.FilesManager.IFileManager fileManager)
	{
		FilesManagers.Add(fileManager);
	}

	/// <summary>
	///		Carga los datos de un proyecto y lo procesa
	/// </summary>
	public async Task ProcessAsync(CancellationToken cancellationToken)
	{
		await ProcessAsync(Load(), cancellationToken);
	}

	/// <summary>
	///		Carga el proyecto con los datos de configuración
	/// </summary>
	public JobProjectModel Load()
	{
		JobProjectModel project = new Repository.JobProjectRepository(this, Configuration.ProjectFile).Load();

			// Carga los datos de contexto
			if (!string.IsNullOrWhiteSpace(Configuration.ContextFile))
				new Repository.JobContextRepository().Load(this, project, Configuration.ContextFile);
			// Devuelve el proyecto
			return project;
	}

	/// <summary>
	///		Ejecuta un proyecto
	/// </summary>
	public async Task<bool> ProcessAsync(JobProjectModel project, CancellationToken cancellationToken)
	{
		// Log
		Logger.LogInformation($"Start project execution '{project.Name}'");
		// Indica que por ahora no ha habido errores
		Errors.Clear();
		// Ejecuta los pasos
		await new Interpreter.JobProjectInterpreter(this, project).ProcessAsync(cancellationToken);
		// Devuelve el valor que indica si se ha procesado correctamente
		return !HasError;
	}

	/// <summary>
	///		Obtiene la cadena de depuración
	/// </summary>
	public string GetDebug()
	{
		System.Text.StringBuilder builder = new();
		JobProjectModel project = Load();

			// Añade los datos del proyecto a la depuración
			builder.AppendLine(project.Debug());
			// Añade los contextos
			foreach (JobContextModel context in project.ProjectExecutionContext.Contexts)
				context.Debug(builder, string.Empty);
			// Devuelve la cadena de depuración
			return builder.ToString();
	}

	/// <summary>
	///		Manejador de log
	/// </summary>
	internal ILogger Logger { get; }

	/// <summary>
	///		Configuración
	/// </summary>
	internal Models.ConfigurationModel Configuration { get; }

	/// <summary>
	///		Procesadores
	/// </summary>
	internal List<IJobProcessor> Processors { get; } = new();

	/// <summary>
	///		Managers de archivos
	/// </summary>
	internal List<Core.Interfaces.FilesManager.IFileManager> FilesManagers { get; } = new();

	/// <summary>
	///		Colección de errores
	/// </summary>
	public List<string> Errors { get; } = new();

	/// <summary>
	///		Indica si ha habido algún error en el procesamiento del script
	/// </summary>
	public bool HasError => Errors.Count > 0;
}
