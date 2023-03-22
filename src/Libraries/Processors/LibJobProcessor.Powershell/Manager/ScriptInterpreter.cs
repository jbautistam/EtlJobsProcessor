using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;
using Bau.Libraries.LibJobProcessor.Powershell.Models;

namespace Bau.Libraries.LibJobProcessor.Powershell.Manager
{
	/// <summary>
	///		Intérprete del script
	/// </summary>
	internal class ScriptInterpreter
	{
		internal ScriptInterpreter(JobPowershellManager manager, JobStepModel step)
		{
			Manager = manager;
			Step = step;
		}

		/// <summary>
		///		Procesa las sentencias del script
		/// </summary>
		internal async Task<bool> ProcessAsync(List<BaseSentence> program, NormalizedDictionary<object> parameters, CancellationToken cancellationToken)
		{
			// Ejecuta el programa
			await ExecuteAsync(program, parameters, cancellationToken);
			// Devuelve el valor que indica si todo es correcto
			return !HasError;
		}

		/// <summary>
		///		Procesa los datos
		/// </summary>
		private async Task ExecuteAsync(List<BaseSentence> sentences, NormalizedDictionary<object> parameters, CancellationToken cancellationToken)
		{
			foreach (BaseSentence sentenceBase in sentences)
				if (!HasError && sentenceBase.Enabled && !cancellationToken.IsCancellationRequested)
					switch (sentenceBase)
					{
						case ExecuteScriptSentence sentence:
								await ProcessScriptAsync(sentence, parameters, cancellationToken);
							break;
						default:
								AddError($"Unknown sentence {sentenceBase.GetType().ToString()}");
							break;
					}
		}

		/// <summary>
		///		Procesa un script
		/// </summary>
		private async Task ProcessScriptAsync(ExecuteScriptSentence sentence, NormalizedDictionary<object> parameters, CancellationToken cancellationToken)
		{
			// Log
			Manager.LogInformation($"Start execute script");
			// Ejecuta la sentencia
			try
			{
				PowerShellController controller = new PowerShellController(this, Step);

					// Ejecuta el script
					await controller.ExecuteAsync(sentence, parameters);
					// Muestra los errores
					if (controller.Errors.Count > 0)
						AddErrors(controller.Errors);
					else
						Manager.LogInformation("End script execution");
			}
			catch (Exception exception)
			{
				AddError("Error when execute script Powershell. {exception.Message}", exception);
			}
		}

		/// <summary>
		///		Añade un error a la colección
		/// </summary>
		private void AddError(string message, Exception exception = null)
		{
			Manager.LogError(message, exception);
			Errors.Add(message + Environment.NewLine + exception?.Message);
		}

		/// <summary>
		///		Añade una serie de errores a la colección
		/// </summary>
		private void AddErrors(List<string> errors)
		{
			foreach (string error in errors)
			{
				Manager.LogError(error);
				Errors.Add(error);
			}
		}

		/// <summary>
		///		Manager
		/// </summary>
		internal JobPowershellManager Manager { get; }

		/// <summary>
		///		Paso de ejecución
		/// </summary>
		private JobStepModel Step { get; }

		/// <summary>
		///		Errores de proceso
		/// </summary>
		internal List<string> Errors { get; } = new List<string>();

		/// <summary>
		///		Indica si ha habido algún error
		/// </summary>
		public bool HasError 
		{ 
			get { return Errors.Count > 0; }
		}
	}
}