using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibJobProcessor.Rest.Models.Sentences;
using Bau.Libraries.LibHttpClient;
using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibInterpreter.Interpreter.Context.Variables;

namespace Bau.Libraries.LibJobProcessor.Rest.Manager.Processor;

/// <summary>
///		Procesador de llamadas a la API
/// </summary>
internal class ApiProcessor
{
	internal ApiProcessor(ScriptInterpreter interpreter)
	{
		Interpreter = interpreter;
	}

	/// <summary>
	///		Procesa una sentencia de llamada
	/// </summary>
	internal async Task<List<SentenceBase>> ProcessAsync(CallApiSentence sentence, CancellationToken cancellationToken)
	{
		HttpClientManager client = new HttpClientManager(new Uri(TransformVariables(sentence.Url)), TransformVariables(sentence.User), TransformVariables(sentence.Password));
		List<SentenceBase> sentences = new();

			// Ejecuta las sentencias
			foreach (CallApiMethodSentence method in sentence.Methods)
				if (!Interpreter.CheckHasError() && !cancellationToken.IsCancellationRequested)
					sentences.AddRange(await ExecuteMethodAsync(method, client, cancellationToken));
			// Devuelve las sentencias que hay que ejecutar
			return sentences;
	}

	/// <summary>
	///		Ejecuta el método
	/// </summary>
	private async Task<List<SentenceBase>> ExecuteMethodAsync(CallApiMethodSentence method, HttpClientManager client, CancellationToken cancellationToken)
	{
		List<SentenceBase> sentences = new();
		int result = -1;

			// Ejecuta el método
			switch (method.Method)
			{
				case CallApiMethodSentence.MethodType.Post:
						result = await client.PostAsync(method.EndPoint, TransformVariables(method.Body), cancellationToken);
					break;
			}
			// Trata el resultado
			if (result == -1)
				Interpreter.AddError($"Error when call {method.EndPoint}: {method.Method.ToString()}");
			else
				sentences = GetContinuation(method, result);
			// Devuelve la lista de sentencias
			return sentences;
	}

	/// <summary>
	///		Obtiene las sentencias que se deben ejecutar al encontrar un resultado
	/// </summary>
	private List<SentenceBase> GetContinuation(CallApiMethodSentence method, int result)
	{
		// Obtiene el grupo de sentencias que tiene que tratar el resultado
		foreach (CallApiResultSentence apiResult in method.Results)
			if (apiResult.ResultFrom >= result && apiResult.ResultTo <= result)
				return apiResult.Sentences;
		// Si no ha encontrado nada, busca un resultado que tenga tanto FROM como TO a 0
		foreach (CallApiResultSentence apiResult in method.Results)
			if (apiResult.ResultFrom == 0 && apiResult.ResultTo == 0)
				return apiResult.Sentences;
		// Si ha llegado hasta aquí, es porque no ha encontrado nada a ejecutar
		return new();
	}

	/// <summary>
	///		Transforma las variables
	/// </summary>
	private string? TransformVariables(string? value)
	{
		// Transforma la cadena teniendo en cuenta las variables
		if (!string.IsNullOrWhiteSpace(value))
			foreach ((string key, VariableModel variable) in Interpreter.ProgramInterpreter.GetVariablesRecursive())
			{
				string variableKey = "{{" + key + "}}";
				
					if (value.Contains(variableKey, StringComparison.CurrentCultureIgnoreCase))
						value = value.ReplaceWithStringComparison(variableKey, variable.GetStringValue());
			}
		// Devuelve el valor modificado
		return value;
	}

	/// <summary>
	///		Intérprete
	/// </summary>
	internal ScriptInterpreter Interpreter { get; }
}
