namespace Bau.Libraries.LibJobProcessor.Rest.Models.Sentences;

/// <summary>
///		Sentencia de definición de API
/// </summary>
internal class CallApiSentence : RestBaseSentence
{
	/// <summary>
	///		Dirección de la API
	/// </summary>
	internal string? Url { get; set; }

	/// <summary>
	///		Usuario
	/// </summary>
	internal string? User { get; set; }

	/// <summary>
	///		Contraseña
	/// </summary>
	internal string? Password { get; set; }

	/// <summary>
	///		Métodos a los que se debe llamar
	/// </summary>
	internal List<CallApiMethodSentence> Methods { get; } = new();
}
