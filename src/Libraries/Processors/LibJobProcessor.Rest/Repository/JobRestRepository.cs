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
	private const string TagEnabled = "Enabled";
	private const string TagCallApiSentence = "CallApi";
	private const string TagUrl = "Url";
	private const string TagUser = "User";
	private const string TagPassword = "Password";
	private const string TagMethod = "Method";
	private const string TagType = "Type";
	private const string TagEndPoint = "EndPoint";
	private const string TagBody = "Body";
	private const string TagResult = "Result";
	private const string TagFrom = "From";
	private const string TagTo = "To";

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
						TagCallApiSentence => LoadApiDefinitionSentence(rootML),
						_ => null
					};
	}

	/// <summary>
	///		Carga una sentencia de deifinición de API
	/// </summary>
	private RestBaseSentence LoadApiDefinitionSentence(MLNode rootML)
	{
		CallApiSentence sentence = new CallApiSentence();

			// Asigna las propiedades
			AssignDefaultProperties(sentence, rootML);
			sentence.Url = rootML.Attributes[TagUrl].Value.TrimIgnoreNull();
			sentence.User = rootML.Attributes[TagUser].Value.TrimIgnoreNull();
			sentence.Password = rootML.Attributes[TagPassword].Value.TrimIgnoreNull();
			// Obtiene los métodos
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagMethod)
					sentence.Methods.Add(LoadMethod(nodeML));
			// Devuelve la sentencia
			return sentence;
	}

	/// <summary>
	///		Carga los datos de un método
	/// </summary>
	private CallApiMethodSentence LoadMethod(MLNode rootML)
	{
		CallApiMethodSentence sentence = new CallApiMethodSentence();

			// Asigna las propiedades
			AssignDefaultProperties(sentence, rootML);
			sentence.EndPoint = rootML.Attributes[TagEndPoint].Value.TrimIgnoreNull();
			sentence.Method = rootML.Attributes[TagType].Value.GetEnum(CallApiMethodSentence.MethodType.Unkwnown);
			sentence.Body = rootML.Nodes[TagBody].Value.TrimIgnoreNull();
			// Obtiene los resultados
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagResult)
					sentence.Results.Add(LoadResult(nodeML));
			// Devuelve la sentencia
			return sentence;
	}

	/// <summary>
	///		Carga la sentencia de tratamiento de resultados
	/// </summary>
	private CallApiResultSentence LoadResult(MLNode rootML)
	{
		CallApiResultSentence sentence = new CallApiResultSentence();

			// Obtiene los resultados
			sentence.ResultFrom = rootML.Attributes[TagFrom].Value.GetInt(0);
			sentence.ResultTo = rootML.Attributes[TagTo].Value.GetInt(0);
			// Carga las sentencias
			sentence.Sentences.AddRange(ProgramRepository.LoadSentences(rootML.Nodes));
			// Devuelve la sentencia
			return sentence;
	}

	/// <summary>
	///		Asigna los datos básicos de una sentencia
	/// </summary>
	private void AssignDefaultProperties(RestBaseSentence sentence, MLNode rootML)
	{
		sentence.Enabled = rootML.Attributes[TagEnabled].Value.GetBool(true);
		sentence.Timeout = ProgramRepository.GetTimeout(rootML, TimeSpan.FromMinutes(5));
	}
}
