using Bau.Libraries.LibInterpreter.Models.Sentences;

namespace Bau.Libraries.LibJobProcessor.Rest.Models.Sentences;

/// <summary>
///		Clase base para las sentencias
/// </summary>
internal abstract class RestBaseSentence : SentenceBase
{
	/// <summary>
	///		Indica si se debe ejecutar la instrucción
	/// </summary>
	internal bool Enabled { get; set; } = true;

	/// <summary>
	///		Tiempo de espera
	/// </summary>
	internal TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(5);
}
