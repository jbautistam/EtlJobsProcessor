using System;
using System.Threading.Tasks;

using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;
using Bau.Libraries.LibJobProcessor.Powershell.Models;
using Bau.Libraries.LibPowerShell;

namespace Bau.Libraries.LibJobProcessor.Powershell.Manager
{
	/// <summary>
	///		Controlador para ejecutar sentencias de PowerShell
	/// </summary>
	internal class PowerShellController
	{
		internal PowerShellController(ScriptInterpreter interpreter, JobStepModel step)
		{
			Interpreter = interpreter;
			Step = step;
		}

		/// <summary>
		///		Ejecuta una sentencia de un script de Powershell
		/// </summary>
		internal async Task ExecuteAsync(ExecuteScriptSentence sentence, NormalizedDictionary<object> parameters)
		{
			if (!string.IsNullOrWhiteSpace(sentence.Content))
				await ExecuteContentAsync(sentence.Content, parameters, sentence);
			else if (!string.IsNullOrWhiteSpace(sentence.FileName))
			{
				string fileName = Step.Project.GetFullFileName(sentence.FileName);

					if (!System.IO.File.Exists(fileName))
						Errors.Add($"Can't find the file '{fileName}'");
					else 
						await ExecuteFileAsync(fileName, parameters, sentence);
			}
			else
				Errors.Add("There is no content nor filename at sentence");
		}

		/// <summary>
		///		Ejecuta un archivo de Powershell
		/// </summary>
		private async Task ExecuteFileAsync(string fileName, NormalizedDictionary<object> parameters, ExecuteScriptSentence sentence)
		{
			await ExecuteContentAsync(LibHelper.Files.HelperFiles.LoadTextFile(fileName), parameters, sentence);
		}

		/// <summary>
		///		Ejecuta un script de powershell
		/// </summary>
		private async Task ExecuteContentAsync(string script, NormalizedDictionary<object> parameters, ExecuteScriptSentence sentence)
		{
			PowerShellManager manager = new(CreateContext(sentence, script, parameters));
			PowerShellResult result = await manager.ProcessAsync(System.Threading.CancellationToken.None);

					// Añade los datos de log
					foreach (PowerShellLog log in result.Log)
						switch (log.Type)
						{
							case PowerShellLog.LogType.Error:
									Errors.Add(log.Message);
								break;
							case PowerShellLog.LogType.Info:
									Interpreter.Manager.LogInformation(log.Message);
								break;
							case PowerShellLog.LogType.Warning:
									Interpreter.Manager.LogInformation("[WARNING] " + log.Message);
								break;
						}
					// Añade los datos de los parámetros de salida
					foreach (object output in result.OutputObjects)
						Interpreter.Manager.LogInformation($"{result.OutputObjects.IndexOf(output).ToString()}: {output?.ToString()}");
		}

		/// <summary>
		///		Añade los parámetros al powershell que se va a ejecutar
		/// </summary>
		private PowerShellContext CreateContext(ExecuteScriptSentence sentence, string script, NormalizedDictionary<object> parameters)
		{
			PowerShellContext context = new();

				// Asigna el script
				context.Script = script;
				// Añade los parámetros
				foreach ((string key, string value) in sentence.Mappings.Enumerate())
					if (parameters.ContainsKey(key))
						context.InputParameters.Add(value, parameters[key]);
				// Añade los directorios
				foreach ((string key, string value) in sentence.Paths.Enumerate())
					context.InputParameters.Add(key, Step.Project.GetFullFileName(value));
				// Devuelve el contexto para ejecutar el Powershell
				return context;
		}

		/// <summary>
		///		Intérprete
		/// </summary>
		internal ScriptInterpreter Interpreter { get; }

		/// <summary>
		///		Paso para el que se ejecuta el powersehll
		/// </summary>
		internal JobStepModel Step { get; }

		/// <summary>
		///		Errores del proceso
		/// </summary>
		internal System.Collections.Generic.List<string> Errors { get; } = new System.Collections.Generic.List<string>();
	}
}