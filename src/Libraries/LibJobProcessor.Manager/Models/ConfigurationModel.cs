namespace Bau.Libraries.LibJobProcessor.Manager.Models;

/// <summary>
///		Clase con los datos de configuración
/// </summary>
public class ConfigurationModel
{
	/// <summary>
	///		Interpreta los argumentos
	/// </summary>
	public void Parse(string[] args)
	{
		for (int index = 0; index < args.Length; index++)
			if (IsParameter("Context", args[index]) && index + 1 < args.Length)
				ContextFile = args[index + 1];
			else if (IsParameter("Project", args[index]) && index + 1 < args.Length)
				ProjectFile = args[index + 1];
			else if (IsParameter("Debug", args[index]))
				IsDebug = true;
	}

	/// <summary>
	///		Comprueba si la clave coincide con el parámetro
	/// </summary>
	private bool IsParameter(string key, string value)
	{
		return !string.IsNullOrWhiteSpace(key) && (key.Equals(value, StringComparison.CurrentCultureIgnoreCase) ||
												   $"-{key}".Equals(value, StringComparison.CurrentCultureIgnoreCase) ||
												   $"--{key}".Equals(value, StringComparison.CurrentCultureIgnoreCase)
												  );
	}

	/// <summary>
	///		Valida la configuración
	/// </summary>
	public bool Validate(out string error)
	{
		// Inicializa los argumentos de salida
		error = string.Empty;
		// Comprueba los datos
		if (string.IsNullOrWhiteSpace(ProjectFile))
			error = "Script file name undefined";
		else if (!File.Exists(ProjectFile))
			error = $"Can't find the script '{ProjectFile}'";
		else if (!string.IsNullOrWhiteSpace(ContextFile) && !File.Exists(ContextFile))
			error = $"Can't find the context file '{ContextFile}'";
		// Devuelve el valor que indica si los datos son correctos
		return string.IsNullOrWhiteSpace(error);
	}

	/// <summary>
	///		Archivo de proyecto
	/// </summary>
	public string ProjectFile { get; set; } = default!;

	/// <summary>
	///		Archivo de contexto
	/// </summary>
	public string ContextFile { get; set; } = default!;

	/// <summary>
	///		Indica si está en modo de depuración
	/// </summary>
	public bool IsDebug { get; private set; }
}
