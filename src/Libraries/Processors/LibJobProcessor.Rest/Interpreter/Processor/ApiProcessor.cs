using Bau.Libraries.LibJobProcessor.Rest.Models.Sentences;
using Bau.Libraries.LibRestClient;
using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibRestClient.Messages;

namespace Bau.Libraries.LibJobProcessor.Rest.Interpreter.Processor;

/// <summary>
///		Procesador de llamadas a la API
/// </summary>
internal class ApiProcessor
{
	internal ApiProcessor(ScriptInterpreter interpreter, RestConnection connection)
	{
		Interpreter = interpreter;
		Connection = connection;
	}

	/// <summary>
	///		Procesa una sentencia de llamada
	/// </summary>
	internal async Task<List<SentenceBase>> ProcessAsync(CallApiSentence sentence, CancellationToken cancellationToken)
	{
		List<SentenceBase> sentences = new();

			try
			{
				ResponseMessage response = await Connection.SendAsync(CreateRequest(sentence), cancellationToken);

					// Trata las asignaciones
					AssignVariables(sentence, response);
					// Trata la respuesta
					sentences.AddRange(GetContinuation(sentence, response));
			}
			catch (Exception exception)
			{
				Interpreter.AddError($"Error when call api {sentence.EndPoint} {sentence.Method.ToString()}", exception);
			}
			// Devuelve las sentencias
			return sentences;
	}

	/// <summary>
	///		Crea el mensaje de la solicitud
	/// </summary>
	private RequestMessage CreateRequest(CallApiSentence sentence)
	{
		RequestMessage request = new RequestMessage(Convert(sentence.Method), Interpreter.ApplyVariables(sentence.EndPoint), sentence.Timeout);

			// Asigna las cabeceras
			foreach (KeyValuePair<string, string> header in sentence.Headers)
				request.Headers.Add(Interpreter.ApplyVariables(header.Key), Interpreter.ApplyVariables(header.Value));
			// Asigna el contenido
			request.Content = Interpreter.ApplyVariables(sentence.Body);
			// Log
			Interpreter.LogDebug($"Call to: {request.EndPoint}");
			// Devuelve la solicitud
			return request;

			// Convierte el método
			RequestMessage.MethodType Convert(CallApiSentence.MethodType method)
			{
				return method switch
								{
									CallApiSentence.MethodType.Get => RequestMessage.MethodType.Get,
									CallApiSentence.MethodType.Post => RequestMessage.MethodType.Post,
									CallApiSentence.MethodType.Put => RequestMessage.MethodType.Put,
									CallApiSentence.MethodType.Delete => RequestMessage.MethodType.Delete,
									CallApiSentence.MethodType.Head => RequestMessage.MethodType.Head,
									CallApiSentence.MethodType.Options => RequestMessage.MethodType.Options,
									_ => throw new ArgumentException($"Rest method unknown: {method.ToString()}")
								};
			}
	}

	/// <summary>
	///		Asigna las variables con los datos de las respuestas
	/// </summary>
	private void AssignVariables(CallApiSentence sentence, ResponseMessage response)
	{
		foreach (CallApiAssignmentSentence assignment in sentence.Assignments)
			switch (assignment.Type)
			{
				case CallApiAssignmentSentence.ContentType.Body:
						Interpreter.ProgramInterpreter.AddVariable(assignment.Variable, LibInterpreter.Models.Symbols.SymbolModel.SymbolType.String, response.Content);
					break;
				case CallApiAssignmentSentence.ContentType.Header:
						if (response.Headers.TryGetValue(assignment.Key, out string? value))
							Interpreter.ProgramInterpreter.AddVariable(assignment.Variable, LibInterpreter.Models.Symbols.SymbolModel.SymbolType.String, value);
					break;
				case CallApiAssignmentSentence.ContentType.StatusCode:
						Interpreter.ProgramInterpreter.AddVariable(assignment.Variable, LibInterpreter.Models.Symbols.SymbolModel.SymbolType.Boolean, response.StatusCode);
					break;
			}
	}

	/// <summary>
	///		Obtiene las sentencias que se deben ejecutar al encontrar un resultado
	/// </summary>
	private List<SentenceBase> GetContinuation(CallApiSentence sentence, ResponseMessage response)
	{
		// Obtiene el grupo de sentencias que tiene que tratar el resultado
		foreach (CallApiResultSentence apiResult in sentence.WhenResults)
			if (apiResult.From >= response.StatusCode && apiResult.To <= response.StatusCode)
				return apiResult.Sentences;
		// Si no ha encontrado nada, devuelve las sentencias de Else
		return sentence.Else;
	}

	/// <summary>
	///		Intérprete
	/// </summary>
	internal ScriptInterpreter Interpreter { get; }

	/// <summary>
	///		Conexión a la API
	/// </summary>
	internal RestConnection Connection { get; }
}
