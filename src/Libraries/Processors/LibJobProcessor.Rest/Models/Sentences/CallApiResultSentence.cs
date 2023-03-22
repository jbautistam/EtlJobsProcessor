using Bau.Libraries.LibInterpreter.Models.Sentences;

namespace Bau.Libraries.LibJobProcessor.Rest.Models.Sentences;

/// <summary>
///		Resultados de las sentencias de llamadas
/// </summary>
internal class CallApiResultSentence
{
	internal CallApiResultSentence(int from, int to)
	{
		From = from;
		To = to;
	}

	/// <summary>
	///		Resultado del método: valor inicial
	/// </summary>
	internal int From { get; }

	/// <summary>
	///		Resultado del método: valor final
	/// </summary>
	internal int To { get; }

	/// <summary>
	///		Sentencias a ejecutar si el resultado está entre estos valores
	/// </summary>
	internal List<SentenceBase> Sentences { get; } = new();
}
