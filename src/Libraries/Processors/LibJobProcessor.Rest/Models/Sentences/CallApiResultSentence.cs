using Bau.Libraries.LibInterpreter.Models.Sentences;

namespace Bau.Libraries.LibJobProcessor.Rest.Models.Sentences;

/// <summary>
///		Sentencia a ejecutar cuando se da un resultado
/// </summary>
internal class CallApiResultSentence : RestBaseSentence
{
	/// <summary>
	///		Resultado del método: valor inicial
	/// </summary>
	public int ResultFrom { get; set; }

	/// <summary>
	///		Resultado del método: valor final
	/// </summary>
	public int ResultTo { get; set; }

	/// <summary>
	///		Sentencias a ejecutar si el resultado está entre estos valores
	/// </summary>
	internal List<SentenceBase> Sentences { get; } = new();
}
