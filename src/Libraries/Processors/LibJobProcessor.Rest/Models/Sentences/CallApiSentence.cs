namespace Bau.Libraries.LibJobProcessor.Rest.Models.Sentences;

/// <summary>
///		Sentencia de llamada a una API
/// </summary>
internal class CallApiSentence : BaseRestSentence
{
	/// <summary>
	///		Método de llamada
	/// </summary>
	internal enum MethodType
	{
		/// <summary>Desconocido. No se debería utilizar</summary>
		Unkwnown,
		/// <summary>Método GET</summary>
		Get,
		/// <summary>Método POST</summary>
		Post,
		/// <summary>Método PUT</summary>
		Put,
		/// <summary>Método DELETE</summary>
		Delete,
		/// <summary>Método HEAD</summary>
		Head,
		/// <summary>Método OPTIONS</summary>
		Options
	}

	internal CallApiSentence(string target, string endPoint, MethodType method) : base(target)
	{
		EndPoint = endPoint;
		Method = method;
	}

	/// <summary>
	///		Punto de entrada de la API
	/// </summary>
	internal string EndPoint { get; }

	/// <summary>
	///		Método de llamada a la API
	/// </summary>
	internal MethodType Method { get; }

	/// <summary>
	///		Cabeceras
	/// </summary>
	internal Dictionary<string, string> Headers { get; } = new();

	/// <summary>
	///		Asignaciones de los datos de la respuesta
	/// </summary>
	internal List<CallApiAssignmentSentence> Assignments { get; } = new();

	/// <summary>
	///		Cuerpo de la llamada a la API
	/// </summary>
	internal string Body { get; set; } = default!;

	/// <summary>
	///		Sentencias que se ejecutan dependiendo de los resultados
	/// </summary>
	internal List<CallApiResultSentence> WhenResults { get; } = new();

	/// <summary>
	///		Sentencias a ejecutar cuando se dan otros resultados
	/// </summary>
	internal List<LibInterpreter.Models.Sentences.SentenceBase> Else { get; } = new();
}