using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibJobProcessor.FilesShell.Models.Sentences;
using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Repository;

namespace Bau.Libraries.LibJobProcessor.FilesShell.Repository;

/// <summary>
///		Clase para cargar datos de proceso sobre archivos
/// </summary>
public class JobFilesShellRepository : BaseJobRepository
{
	// Constantes privadas
	private const string TagProviderFile = "ProviderFile";
	private const string TagProviderFileKey = "ProviderFileKey";
	private const string TagCopy = "Copy";
	private const string TagDelete = "Delete";
	private const string TagFrom = "From";
	private const string TagTo = "To";
	private const string TagMask = "Mask";
	private const string TagRecursive = "Recursive";
	private const string TagFlattenPaths = "FlattenPaths";
	private const string TagPath = "Path";
	private const string TagExecute = "Execute";
	private const string TagProcess = "Process";
	private const string TagArgument = "Argument";
	private const string TagKey = "Key";
	private const string TagValue = "Value";
	private const string TagTransformFileName = "TransformFileName";
	private const string TagIfExists = "IfExists";
	private const string TagThen = "Then";
	private const string TagElse = "Else";

	public JobFilesShellRepository(IProgramRepository programRepository) : base(programRepository)
	{
	}

	/// <summary>
	///		Carga los datos de una sentencia
	/// </summary>
	public override SentenceBase? Load(MLNode rootML)
	{
		return rootML.Name switch
					{
						TagCopy => LoadCopySentence(rootML),
						TagDelete => LoadDeleteSentence(rootML),
						TagExecute => LoadExecuteSentence(rootML),
						TagIfExists => LoadIfExistSentence(rootML),
						_ => null
					};
	}

	/// <summary>
	///		Carga la sentencia para descargar un archivo
	/// </summary>
	private SentenceBase LoadCopySentence(MLNode rootML)
	{
		CopySentence sentence = new(rootML.Attributes[TagProviderFile].Value.TrimIgnoreNull(),
									rootML.Attributes[TagProviderFileKey].Value.TrimIgnoreNull(),
									rootML.Attributes[TagFrom].Value.TrimIgnoreNull(),
									rootML.Attributes[TagTo].Value.TrimIgnoreNull(),
									rootML.Attributes[TagMask].Value.TrimIgnoreNull()
								   );

			// Asigna las propiedades
			sentence.Recursive = rootML.Attributes[TagRecursive].Value.GetBool(false);
			sentence.FlattenPaths = rootML.Attributes[TagFlattenPaths].Value.GetBool(false);
			// Devuelve la sentencia
			return sentence;
	}

	/// <summary>
	///		Carga una sentencia de borrado
	/// </summary>
	private SentenceBase LoadDeleteSentence(MLNode rootML)
	{
		return new DeleteSentence(rootML.Attributes[TagProviderFile].Value.TrimIgnoreNull(),
								  rootML.Attributes[TagProviderFileKey].Value.TrimIgnoreNull(),
								  rootML.Attributes[TagPath].Value.TrimIgnoreNull(),
								  rootML.Attributes[TagMask].Value.TrimIgnoreNull());
	}

	/// <summary>
	///		Carga una sentencia de ejecución de un proceso
	/// </summary>
	private SentenceBase LoadExecuteSentence(MLNode rootML)
	{
		ExecuteSentence sentence = new(rootML.Attributes[TagProviderFile].Value.TrimIgnoreNull(),
									   rootML.Attributes[TagProviderFileKey].Value.TrimIgnoreNull(),
									   rootML.Attributes[TagProcess].Value.TrimIgnoreNull());

			// Asigna las propiedades
			sentence.Timeout = ProgramRepository.GetTimeout(rootML, TimeSpan.FromMinutes(5));
			// Carga los argumentos
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagArgument)
					sentence.Arguments.Add(new ExecuteSentence.ExecuteSentenceArgument(nodeML.Attributes[TagKey].Value.TrimIgnoreNull(),
																					   nodeML.Attributes[TagValue].Value.TrimIgnoreNull(),
																					   nodeML.Attributes[TagTransformFileName].Value.GetBool()
																					  )
										   );
			// Devuelve la sentencia
			return sentence;
	}

	/// <summary>
	///		Carga una sentencia IfExists
	/// </summary>
	private SentenceBase LoadIfExistSentence(MLNode rootML)
	{
		IfExistsSentence sentence = new(rootML.Attributes[TagProviderFile].Value.TrimIgnoreNull(),
										rootML.Attributes[TagProviderFileKey].Value.TrimIgnoreNull(),
										rootML.Attributes[TagPath].Value.TrimIgnoreNull());
		bool loaded = false;

			// Carga las sentencias
			foreach (MLNode nodeML in rootML.Nodes)
				switch (nodeML.Name)
				{
					case TagThen:
							sentence.ThenSentences.AddRange(LoadSentences(nodeML.Nodes));
							loaded = true;
						break;
					case TagElse:
							sentence.ElseSentences.AddRange(LoadSentences(nodeML.Nodes));
							loaded = true;
						break;
				}
			// Si no se ha cargado nada, todo lo que haya dentro se considera parte del then
			if (!loaded)
				sentence.ThenSentences.AddRange(LoadSentences(rootML.Nodes));
			// Devuelve la sentencia
			return sentence;
	}
}