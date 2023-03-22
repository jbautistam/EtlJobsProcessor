namespace Bau.Libraries.LibJobProcessor.Core.Models.Jobs;

/// <summary>
///		Datos necesarios para la ejecución del proyecto
/// </summary>
public class ProjectExecutionModel
{
	public ProjectExecutionModel(string projectWorkPath)
	{
		ProjectWorkPath = projectWorkPath;
	}

	/// <summary>
	///		Obtiene un contexto
	/// </summary>
	public JobContextModel? GetContext(string key)
	{
		return Contexts.FirstOrDefault(item => item.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase));
	}

	/// <summary>
	///		Directorio de proyecto
	/// </summary>
	public string ProjectWorkPath { get;  }

	/// <summary>
	///		Fecha de inicio de la ejecución
	/// </summary>
	public DateTime StartExecution { get; } = DateTime.UtcNow;

	/// <summary>
	///		Contexto de los diferentes tipos de procesadores
	/// </summary>
	public List<JobContextModel> Contexts { get; } = new();

	/// <summary>
	///		Parámetros de ejecución
	/// </summary>
	public List<JobParameterModel> Parameters { get; } = new();
}