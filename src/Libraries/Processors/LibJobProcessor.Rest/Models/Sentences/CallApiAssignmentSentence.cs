namespace Bau.Libraries.LibJobProcessor.Rest.Models.Sentences;

/// <summary>
///		Asignación de valores de las respuestas a variables
/// </summary>
internal class CallApiAssignmentSentence
{
	/// <summary>
	///		Tipo de contenido a asignar
	/// </summary>
	internal enum ContentType
	{
		/// <summary>Del cuerpo</summary>
		Body,
		/// <summary>De la cabecera</summary>
		Header,
		/// <summary>Código de la respuesta</summary>
		StatusCode
	}

	internal CallApiAssignmentSentence(string key, ContentType type, string variable)
	{
		Key = key;
		Type = type;
		Variable = variable;
	}

	/// <summary>
	///		Clave de la cabecera en su caso
	/// </summary>
	internal string Key { get; }

	/// <summary>
	///		Tipo de contenido a almacenar
	/// </summary>
	internal ContentType Type { get; }

	/// <summary>
	///		Variable donde se almacena
	/// </summary>
	internal string Variable { get; }
}
