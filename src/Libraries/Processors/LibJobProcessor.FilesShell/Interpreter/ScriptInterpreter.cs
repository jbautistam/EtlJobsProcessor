using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibJobProcessor.FilesShell.Models.Sentences;
using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Interpreter;

namespace Bau.Libraries.LibJobProcessor.FilesShell.Interpreter;

/// <summary>
///		Intérprete del script
/// </summary>
internal class ScriptInterpreter : BaseJobSentenceIntepreter
{
	public ScriptInterpreter(IProgramInterpreter programInterpreter) : base(programInterpreter)
	{
	}

	/// <summary>
	///		Inicializa el intérprete
	/// </summary>
	public override async Task InitializeAsync(CancellationToken cancellationToken)
	{
		await Task.Delay(1, cancellationToken);
	}

	/// <summary>
	///		Procesa una sentencia
	/// </summary>
	public async override Task<bool> ProcessAsync(SentenceBase sentenceBase, CancellationToken cancellationToken)
	{
		bool executed = true;

			// Ejecuta la sentencia
			switch (sentenceBase)
			{
				case CopySentence sentence:
						await ProcessCopyAsync(sentence, cancellationToken);
					break;
				case DeleteSentence sentence:
						await ProcessDeleteAsync(sentence, cancellationToken);
					break;
				case ExecuteSentence sentence:
						await ProcessExecuteAsync(sentence, cancellationToken);
					break;
				case IfExistsSentence sentence:
						await ProcessExistsAsync(sentence, cancellationToken);
					break;
				default:
						executed = false;
					break;
			}
			// Devuelve el valor que indica si ha ejecutado la sentencia
			return executed;
	}

	/// <summary>
	///		Procesa la sentencia que comprueba si existe un archivo o directorio
	/// </summary>
	private async Task ProcessExistsAsync(IfExistsSentence sentence, CancellationToken cancellationToken)
	{
		string? path = GetFullFileName(sentence.ProviderFile, sentence.ProviderFileKey, sentence.Path);

			if (string.IsNullOrWhiteSpace(path))
				AddError("Path is empty");
			else if (Directory.Exists(path) || File.Exists(path))
				await ProcessAsync(sentence.ThenSentences, cancellationToken);
			else
				await ProcessAsync(sentence.ElseSentences, cancellationToken);
	}

	/// <summary>
	///		Procesa una copia de archivos
	/// </summary>
	private async Task ProcessCopyAsync(CopySentence sentence, CancellationToken cancellationToken)
	{
		string? source = GetFullFileName(sentence.ProviderFile, sentence.ProviderFileKey, sentence.Source);
		string? target = GetFullFileName(sentence.ProviderFile, sentence.ProviderFileKey, sentence.Target);

			// Log
			LogInfo($"Start copy from '{sentence.Source}' to '{sentence.Target}'");
			// Copia los archivos
			if (string.IsNullOrWhiteSpace(source))
				AddError("Source is empty");
			else if (string.IsNullOrWhiteSpace(target))
				AddError("Target is empty");
			else
				try
				{
					if (Directory.Exists(source))
						await LibHelper.Files.HelperFiles.CopyPathAsync(source, target, sentence.Mask, sentence.Recursive, sentence.FlattenPaths, cancellationToken);
					else if (File.Exists(source))
					{
						if (!string.IsNullOrWhiteSpace(Path.GetExtension(target)))
							await LibHelper.Files.HelperFiles.CopyFileAsync(source, target);
						else
							await LibHelper.Files.HelperFiles.CopyFileAsync(source, Path.Combine(target, Path.GetFileName(source)));
					}
					else
						AddError($"Cant find source file or path '{source}'");
				}
				catch (Exception exception)
				{
					AddError($"Error when copy '{source}' to '{target}'. {exception.Message}");
				}
	}

	/// <summary>
	///		Procesa un borrado de archivos
	/// </summary>
	private async Task ProcessDeleteAsync(DeleteSentence sentence, CancellationToken cancellationToken)
	{
		string? path = GetFullFileName(sentence.ProviderFile, sentence.ProviderFileKey, sentence.Path);

			// Evita el warning
			await Task.Delay(1, cancellationToken);
			// Log
			LogInfo($"Start delete '{sentence.Path}'");
			// Borra los archivos / directorios
			if (string.IsNullOrWhiteSpace(path))
				AddError("Path undefined");
			else
				try
				{
					if (Directory.Exists(path))
					{
						if (!string.IsNullOrWhiteSpace(sentence.Mask))
							LibHelper.Files.HelperFiles.KillFiles(path, sentence.Mask);
						else
							LibHelper.Files.HelperFiles.KillPath(path);
					}
					else if (File.Exists(path))
						LibHelper.Files.HelperFiles.KillFile(path);
					else
						LogInfo($"Cant find file or path '{path}' for delete");
				}
				catch (Exception exception)
				{
					AddError($"Error when delete '{path}'. {exception.Message}");
				}
	}

	/// <summary>
	///		Ejecuta una sentencia de proceso
	/// </summary>
	private async Task ProcessExecuteAsync(ExecuteSentence sentence, CancellationToken cancellationToken)
	{
		// Evita el warning
		await Task.Delay(1, cancellationToken);
		// Log
		LogInfo($"Start execution '{sentence.Process}'");
		// Ejecuta la sentencia
		try
		{
			LibHelper.Processes.SystemProcessHelper processor = new();

				// Ejecuta el proceso
				processor.ExecuteApplication(sentence.Process, ConvertArguments(sentence), true, sentence.Timeout);
				// Log
				LogInfo($"End execution '{sentence.Process}'");
		}
		catch (Exception exception)
		{
			AddError($"Error when execute '{sentence.Process}'. {exception.Message}");
		}
	}

	/// <summary>
	///		Convierte la lista de argumentos
	/// </summary>
	private string ConvertArguments(ExecuteSentence sentence)
	{
		string parameters = string.Empty;

			// Crea la lista de argumentos
			foreach (ExecuteSentence.ExecuteSentenceArgument argument in sentence.Arguments)
			{
				string result = string.Empty;

					// Añade la clave
					if (!string.IsNullOrWhiteSpace(argument.Key))
						result += argument.Key.TrimIgnoreNull();
					// Añade el valor
					if (!string.IsNullOrWhiteSpace(argument.Value))
					{
						if (argument.TransformFileName)
							result = result.AddWithSeparator(GetFullFileName(sentence.ProviderFile, sentence.ProviderFileKey, argument.Value), " ");
						else
							result = result.AddWithSeparator(argument.Value, " ");
					}
					// Añade la cadena a la lista de parámetros
					if (!string.IsNullOrWhiteSpace(result))
						parameters = parameters.AddWithSeparator(result, " ");
			}
			// Devuelve la lista de argumentos
			return parameters;
	}
}