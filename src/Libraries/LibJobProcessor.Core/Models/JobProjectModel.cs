using Bau.Libraries.LibDataStructures.Base;
using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;

namespace Bau.Libraries.LibJobProcessor.Core.Models;

/// <summary>
///		Clase con los datos del proyecto
/// </summary>
public class JobProjectModel : BaseExtendedModel
{
	public JobProjectModel(string projectWorkPath)
	{
		ProjectExecutionContext = new ProjectExecutionModel(projectWorkPath);
	}

	/// <summary>
	///		Obtiene la cadena de depuración 
	/// </summary>
	public string Debug()
	{
		System.Text.StringBuilder builder = new System.Text.StringBuilder();
		string indent = new string(' ', 4);

			// Añade los datos
			builder.AppendLine($"Project {nameof(Name)}: {Name}");
			builder.AppendLine($"{indent}{nameof(Description)}: {Description}");
			builder.AppendLine($"{indent}{nameof(ProjectExecutionContext.ProjectWorkPath)}: {ProjectExecutionContext.ProjectWorkPath}");
			// Añade la depuración de los contextos
			if (ProjectExecutionContext.Contexts.Count > 0)
			{
				builder.AppendLine($"{indent}{nameof(ProjectExecutionContext.Contexts)}:");
				foreach (JobContextModel context in ProjectExecutionContext.Contexts)
					context.Debug(builder, indent + new string(' ', 4));
			}
			//? Aquí debería ir la depuración de las sentencias
			// Devuelve la cadena
			return builder.ToString();
	}

	/// <summary>
	///		Datos del programa
	/// </summary>
	public List<LibInterpreter.Models.Sentences.SentenceBase> Program { get; } = new();

	/// <summary>
	///		Contexto de ejecución del proyecto
	/// </summary>
	public ProjectExecutionModel ProjectExecutionContext { get; }
}
