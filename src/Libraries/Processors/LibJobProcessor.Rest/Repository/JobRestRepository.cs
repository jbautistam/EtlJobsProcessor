using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibJobProcessor.Rest.Models.Sentences;
using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Repository;

namespace Bau.Libraries.LibJobProcessor.Rest.Repository;

/// <summary>
///		Clase para cargar datos de proceso
/// </summary>
internal class JobRestRepository : BaseJobRepository
{
	// Constantes privadas
	private const string TagTarget = "Target";
	private const string TagEnabled = "Enabled";
	private const string TagCallApiSentence = "CallApi";
	private const string TagEndPoint = "EndPoint";
	private const string TagMethod = "Method";
	private const string TagBody = "Body";
	private const string TagWhenResult = "WhenResult";
	private const string TagFrom = "From";
	private const string TagTo = "To";
	private const string TagElse = "Else";
	private const string TagHeader = "Header";
	private const string TagName = "Name";
	private const string TagValue = "Value";
	private const string TagAssign = "Assign";
	private const string TagType = "Type";
	private const string TagVariable = "Variable";

	public JobRestRepository(IProgramRepository programRepository) : base(programRepository)
	{
	}

	/// <summary>
	///		Carga la sentencia de un nodo
	/// </summary>
	public override SentenceBase? Load(MLNode rootML)
	{
		return rootML.Name switch
					{
						TagCallApiSentence => LoadCallApi(rootML),
						_ => null
					};
	}

	/// <summary>
	///		Carga los datos de un método
	/// </summary>
	private CallApiSentence LoadCallApi(MLNode rootML)
	{
		CallApiSentence sentence = new(rootML.Attributes[TagTarget].Value.TrimIgnoreNull(),
									   rootML.Attributes[TagEndPoint].Value.TrimIgnoreNull(),
									   rootML.Attributes[TagMethod].Value.GetEnum(CallApiSentence.MethodType.Unkwnown));

			// Asigna las propiedades
			AssignDefaultProperties(sentence, rootML);
			// Obtiene los posibles resultados
			foreach (MLNode nodeML in rootML.Nodes)
				switch (nodeML.Name)
				{
					case TagHeader:
							sentence.Headers.Add(nodeML.Attributes[TagName].Value.TrimIgnoreNull(), GetHeaderValue(nodeML));							
						break;
					case TagAssign:
							sentence.Assignments.Add(new CallApiAssignmentSentence(nodeML.Attributes[TagName].Value.TrimIgnoreNull(),
																				   nodeML.Attributes[TagType].Value.GetEnum(CallApiAssignmentSentence.ContentType.Body),
																				   nodeML.Attributes[TagVariable].Value.TrimIgnoreNull()));
						break;
					case TagBody:
							sentence.Body = nodeML.Value.TrimIgnoreNull();
						break;
					case TagWhenResult:
							sentence.WhenResults.Add(LoadResult(nodeML));
						break;
					case TagElse:
							sentence.Else.AddRange(LoadSentences(nodeML.Nodes));
						break;
				}
			// Si no hay nada en los resultados ni en el else, recoge las sentencias del cuerpo en el else
			if (sentence.WhenResults.Count == 0 && sentence.Else.Count == 0)
				sentence.Else.AddRange(LoadSentences(rootML.Nodes, TagBody, TagHeader, TagAssign));
			// Devuelve la sentencia
			return sentence;

			// Obtiene el valor de la cabecera (del atributo o del valor)
			string GetHeaderValue(MLNode rootML)
			{
				string value = rootML.Attributes[TagValue].Value.TrimIgnoreNull();

					// Obtiene el valor del nodo
					if (string.IsNullOrWhiteSpace(value))
						value = rootML.Value.TrimIgnoreNull();
					// Devuelve el valor
					return value;
			}
	}

	/// <summary>
	///		Carga la sentencia de tratamiento de resultados
	/// </summary>
	private CallApiResultSentence LoadResult(MLNode rootML)
	{
		CallApiResultSentence sentence = new(rootML.Attributes[TagFrom].Value.GetInt(0),
											 rootML.Attributes[TagTo].Value.GetInt(0));

			// Carga las sentencias
			sentence.Sentences.AddRange(ProgramRepository.LoadSentences(rootML.Nodes));
			// Devuelve la sentencia
			return sentence;
	}

	/// <summary>
	///		Asigna los datos básicos de una sentencia
	/// </summary>
	private void AssignDefaultProperties(BaseRestSentence sentence, MLNode rootML)
	{
		sentence.Enabled = rootML.Attributes[TagEnabled].Value.GetBool(true);
		sentence.Timeout = ProgramRepository.GetTimeout(rootML, TimeSpan.FromMinutes(5));
	}
}
