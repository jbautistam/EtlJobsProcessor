using Bau.Libraries.LibInterpreter.Models.Sentences;

namespace Bau.Libraries.LibJobProcessor.Manager.Models.Sentences;

/// <summary>
///		Bloque de sentencias
/// </summary>
public class SentenceBlock : SentenceBase
{
	/// <summary>
	///		Obtiene el mensaje o un valor predeterminado
	/// </summary>
	public string GetMessage(string defaultMessage)
	{
		if (string.IsNullOrWhiteSpace(Message))
			return defaultMessage;
		else
			return Message;
	}

	/// <summary>
	///		Mensaje asociado al bloque
	/// </summary>
	public string? Message { get; set; }

	/// <summary>
	///		Instrucciones del bloque
	/// </summary>
	public List<SentenceBase> Sentences { get; } = new();
}
