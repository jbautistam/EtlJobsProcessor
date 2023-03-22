using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibJobProcessor.Core.Models;
using Bau.Libraries.LibJobProcessor.Core.Interfaces;
using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibInterpreter.Models.Symbols;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Repository;
using Bau.Libraries.LibJobProcessor.Manager.Models.Sentences;

namespace Bau.Libraries.LibJobProcessor.Manager.Repository;

/// <summary>
///		Clase de lectura de <see cref="JobProjectModel"/>
/// </summary>
internal class JobProjectRepository : IProgramRepository
{
	// Constantes privadas
	private const string TagRoot = "EtlProject";
	private const string TagName = "Name";
	private const string TagDescription = "Description";
	private const string TagEnabled = "Enabled";
	private const string TagImport = "Import";
	private const string TagFile = "FileName";
	private const string TagBlock = "Block";
	private const string TagMessage = "Message";
	private const string TagTimeout = "Timeout";
	private const string TagException = "Exception";
	private const string TagKey = "Key";
	private const string TagSentencePrint = "Print";
	private const string TagValue = "Value";
	private const string TagSentenceIf = "If";
	private const string TagThen = "Then";
	private const string TagElse = "Else";
	private const string TagCondition = "Condition";
	private const string TagSentenceString = "String";
	private const string TagSentenceNumeric = "Numeric";
	private const string TagSentenceBoolean = "Boolean";
	private const string TagSentenceDate = "Date";
	private const string TagSentenceLet = "Let";
	private const string TagType = "Type";
	private const string TagVariable = "Variable";
	private const string TagSentenceFor = "For";
	private const string TagSentenceWhile = "While";
	private const string TagStart = "Start";
	private const string TagEnd = "End";
	private const string TagStep = "Step";
	private const string TagDateNow = "Now";
	private const string TagSentenceOpenFile = "OpenFile";
	private const string TagMode = "Mode";
	private const string TagProviderFile = "ProviderFile";
	private const string TagProviderFileKey = "ProviderFileKey";

	internal JobProjectRepository(JobProjectManager manager, string fileName)
	{
		FileName = fileName;
		Manager = manager;
		foreach (IJobProcessor processor in manager.Processors)
			Repositories.Add(processor.GetRepository(this));
	}

	/// <summary>
	///		Carga los datos de un <see cref="JobProjectModel"/>
	/// </summary>
	internal JobProjectModel Load()
	{
		JobProjectModel project = new(Path.GetDirectoryName(FileName) ?? string.Empty);

			// Carga los datos del archivo
			if (File.Exists(FileName))
			{
				MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(FileName);

					// Obtiene los nodos
					if (fileML is not null)
						foreach (MLNode rootML in fileML.Nodes)
							if (rootML.Name == TagRoot)
							{
								// Asigna el nombre del proyecto
								project.Name = rootML.Nodes[TagName].Value.TrimIgnoreNull();
								project.Description = rootML.Nodes[TagDescription].Value.TrimIgnoreNull();
								// Carga las sentencias de los nodos
								project.Program.AddRange(LoadSentences(rootML.Nodes, TagName, TagDescription));
							}
			}
			else
				throw new Exceptions.JobProcessorException($"Can't find the file {FileName}");
			// Devuelve los datos del proyecto
			return project;
	}

	/// <summary>
	///		Importa las sentencias de un archivo sobre el programa principal
	/// </summary>
	private List<SentenceBase> ImportFile(MLNode rootML)
	{
		string fileName = rootML.Attributes[TagFile].Value.TrimIgnoreNull();

			if (string.IsNullOrWhiteSpace(fileName))
				throw new Exceptions.JobProcessorException($"There is not exist value at attribute {TagFile} at node {TagImport}");
			else
				return new JobProjectRepository(Manager, Path.Combine(ProjectPath, fileName))
								.Load().Program;
	}

	/// <summary>
	///		Carga una sentencia de bloque
	/// </summary>
	private SentenceBase LoadBlockSentence(MLNode rootML)
	{
		SentenceBlock sentence = new()
									{
										Message = rootML.Attributes[TagMessage].Value.TrimIgnoreNull()
									};

			// Carga las sentencias hija
			sentence.Sentences.AddRange(LoadSentences(rootML.Nodes));
			// Devuelve la sentencia
			return sentence;
	}

	/// <summary>
	///		Carga una sentencia de un nodo desde los diferentes procesadores
	/// </summary>
	private SentenceBase LoadXmlNode(MLNode rootML)
	{
		// Carga la sentencia
		foreach (IProcessorRepository repository in Repositories)
		{
			SentenceBase? sentence = repository.Load(rootML);

				if (sentence is not null)
					return sentence;
		}
		// Si ha llegado hasta aquí se lanza una excepción
		throw new Exceptions.JobProcessorException($"Error when load project. Sentence undefined: {rootML.Name.TrimIgnoreNull()}");
	}

	/// <summary>
	///		Carga una serie de sentencias
	/// </summary>
	public List<SentenceBase> LoadSentences(MLNodesCollection nodesML, params string[] skipNodes)
	{
		List<SentenceBase> sentences = new();

			// Carga las sentencias
			foreach (MLNode nodeML in nodesML)
				if (!MustSkip(nodeML, skipNodes))
					switch (nodeML.Name)
					{
						case TagImport:
								if (nodeML.Attributes[TagEnabled].Value.GetBool(true))
									sentences.AddRange(ImportFile(nodeML));
							break;
						case TagBlock:
								sentences.Add(LoadBlockSentence(nodeML));
							break;
						case TagException:
								sentences.Add(LoadExceptionSentence(nodeML));
							break;
						case TagSentencePrint:
								sentences.Add(LoadSentencePrint(nodeML));
							break;
						case TagSentenceIf:
								sentences.Add(LoadSentenceIf(nodeML));
							break;
						case TagSentenceString:
								sentences.Add(LoadSentenceDeclare(nodeML, SymbolModel.SymbolType.String));
							break;
						case TagSentenceNumeric:
								sentences.Add(LoadSentenceDeclare(nodeML, SymbolModel.SymbolType.Numeric));
							break;
						case TagSentenceBoolean:
								sentences.Add(LoadSentenceDeclare(nodeML, SymbolModel.SymbolType.Boolean));
							break;
						case TagSentenceDate:
								sentences.Add(LoadSentenceDeclare(nodeML, SymbolModel.SymbolType.Date));
							break;
						case TagSentenceLet:
								sentences.Add(LoadSentenceLet(nodeML));
							break;
						case TagSentenceFor:
								sentences.Add(LoadSentenceFor(nodeML));
							break;
						case TagSentenceWhile:
								sentences.Add(LoadSentenceWhile(nodeML));
							break;
						case TagSentenceOpenFile:
								sentences.Add(LoadSentenceOpenFile(nodeML));
							break;
						default:
								sentences.Add(LoadXmlNode(nodeML));
							break;
					}
			// Devuelve las sentencias cargadas
			return sentences;
	}

	/// <summary>
	///		Comprueba si se debe saltar la interpretación de un nodo
	/// </summary>
	private bool MustSkip(MLNode nodeML, string[] skipNodes)
	{
		// Comprueba los nodos que se deben saltar
		foreach (string skipNode in skipNodes)
			if (!string.IsNullOrEmpty(skipNode) && nodeML.Name.Equals(skipNode))
				return true;
		// Si ha llegado hasta aquí es porque no ha encontrado ningún nodo a saltar
		return false;
	}

	/// <summary>
	///		Carga los datos de una excepción
	/// </summary>
	private SentenceBase LoadExceptionSentence(MLNode rootML)
	{
		return new SentenceException
						{
							Message = GetMessage(rootML)
						};
	}

	/// <summary>
	///		Carga una sentencia de impresión
	/// </summary>
	private SentenceBase LoadSentencePrint(MLNode rootML)
	{
		return new SentencePrint
						{
							Message = GetMessage(rootML)
						};
	}

	/// <summary>
	///		Carga una sentencia de declaración de variables
	/// </summary>
	private SentenceBase LoadSentenceDeclare(MLNode rootML, SymbolModel.SymbolType type)
	{
		SentenceDeclare sentence = new();

			// Asigna las propiedades
			sentence.Variable.Type = type;
			sentence.Variable.Name = rootML.Attributes[TagName].Value.TrimIgnoreNull();;
			sentence.ExpressionString = rootML.Attributes[TagValue].Value.TrimIgnoreNull();;
			// Devuelve la sentencia
			return sentence;
	}

	/// <summary>
	///		Carga una sentencia de asignación
	/// </summary>
	private SentenceBase LoadSentenceLet(MLNode rootML)
	{
		SentenceLet sentence = new();

			// Asigna las propiedades
			sentence.Type = rootML.Attributes[TagType].Value.GetEnum(SymbolModel.SymbolType.Unknown);
			sentence.Variable = rootML.Attributes[TagName].Value.TrimIgnoreNull();;
			sentence.ExpressionString = rootML.Value.TrimIgnoreNull();;
			// Devuelve la sentencia
			return sentence;
	}

	/// <summary>
	///		Carga una sentencia for
	/// </summary>
	private SentenceBase LoadSentenceFor(MLNode rootML)
	{
		SentenceFor sentence = new();

			// Asigna las propiedades
			sentence.Variable.Type = rootML.Attributes[TagType].Value.GetEnum(SymbolModel.SymbolType.Numeric);
			sentence.Variable.Name = rootML.Attributes[TagVariable].Value.TrimIgnoreNull();
			sentence.StartExpressionString = rootML.Attributes[TagStart].Value.TrimIgnoreNull();
			sentence.EndExpressionString = rootML.Attributes[TagEnd].Value.TrimIgnoreNull();
			sentence.StepExpressionString = rootML.Attributes[TagStep].Value.TrimIgnoreNull();
			// Carga las sentencias
			sentence.Sentences.AddRange(LoadSentences(rootML.Nodes));
			// Devuelve la sentencia
			return sentence;
	}

	/// <summary>
	///		Carga una sentencia If
	/// </summary>
	private SentenceBase LoadSentenceIf(MLNode rootML)
	{
		SentenceIf sentence = new();
		bool loaded = false;

			// Carga la condición
			sentence.Condition = rootML.Attributes[TagCondition].Value.TrimIgnoreNull();
			// Carga las sentencias de la parte then y else
			foreach (MLNode nodeML in rootML.Nodes)
				switch (nodeML.Name)
				{
					case TagThen:
							sentence.SentencesThen.AddRange(LoadSentences(nodeML.Nodes));
							loaded = true;
						break;
					case TagElse:
							sentence.SentencesElse.AddRange(LoadSentences(nodeML.Nodes));
							loaded = true;
						break;
				}
			// Si no se ha cargado nada, se obtienen los datos del nodo
			if (!loaded)
				sentence.SentencesThen.AddRange(LoadSentences(rootML.Nodes));
			// Devuelve la sentencia
			return sentence;
	}

	/// <summary>
	///		Carga una sentencia While
	/// </summary>
	private SentenceBase LoadSentenceWhile(MLNode rootML)
	{
		SentenceWhile sentence = new();

			// Carga la condición y las sentencias
			sentence.ConditionString = rootML.Attributes[TagCondition].Value.TrimIgnoreNull();
			sentence.Sentences.AddRange(LoadSentences(rootML.Nodes));
			// Devuelve la sentencia
			return sentence;
	}

	/// <summary>
	///		Obtiene una sentencia de apertura de un archivo
	/// </summary>
	private SentenceBase LoadSentenceOpenFile(MLNode rootML)
	{
		SentenceUsingFile sentence = new(rootML.Attributes[TagProviderFile].Value.TrimIgnoreNull(),
										 rootML.Attributes[TagProviderFileKey].Value.TrimIgnoreNull(),
										 rootML.Attributes[TagKey].Value.TrimIgnoreNull(),
										 rootML.Attributes[TagMode].Value.GetEnum(Core.Models.Files.StreamUnionModel.OpenMode.Read),
										 rootML.Attributes[TagFile].Value.TrimIgnoreNull()
										);

			// Carga las sentencias hija
			sentence.Sentences.AddRange(LoadSentences(rootML.Nodes));
			// Devuelve la sentencia
			return sentence;
	}

	/// <summary>
	///		Obtiene el contenido de un atributo de mensaje o el valor del nodo si no existe el atributo
	/// </summary>
	private string GetMessage(MLNode rootML)
	{
		if (string.IsNullOrWhiteSpace(rootML.Attributes[TagMessage].Value))
			return rootML.Value.TrimIgnoreNull();
		else
			return rootML.Attributes[TagMessage].Value.TrimIgnoreNull();
	}

	/// <summary>
	///		Obtiene el timeout definido en el nodo
	/// </summary>
	public TimeSpan GetTimeout(MLNode rootML, TimeSpan defaultTimeOut)
	{
		TimeSpan? timeout = null;
		string attribute = rootML.Attributes[TagTimeout].Value;

			// Interpreta la cadena de timeout
			if (!string.IsNullOrWhiteSpace(attribute) && attribute.Length > 1)
			{
				string time = attribute[^1..].ToUpper();
				int value = attribute[..^1].GetInt(0);

					if (value != 0)
						switch (time)
						{
							case "D":
									if (value < 5)
										timeout = TimeSpan.FromDays(value);
									else
										timeout = TimeSpan.FromDays(4);
								break;
							case "H":
									timeout = TimeSpan.FromHours(value);
								break;
							case "M":
									timeout = TimeSpan.FromMinutes(value);
								break;
							case "S":
									timeout = TimeSpan.FromSeconds(value);
								break;
						}
			}
			// Devuelve el timeout de la sentencia
			return timeout ?? defaultTimeOut;
	}

	/// <summary>
	///		Convierte una cadena con un valor en un objeto dependiente del tipo
	/// </summary>
	public object? ConvertStringValue(SymbolModel.SymbolType type, string value)
	{
		if (string.IsNullOrEmpty(value))
			return null;
		else
			switch (type)
			{ 
				case SymbolModel.SymbolType.Boolean:
					return value.GetBool();
				case SymbolModel.SymbolType.Date:
					if (value.EqualsIgnoreCase(TagDateNow))
						return DateTime.Now;
					else
						return value.GetDateTime();
				case SymbolModel.SymbolType.Numeric:
					return value.GetDouble();
				default:
					return value;
			}
	}

	/// <summary>
	///		Manager del proyecto
	/// </summary>
	public JobProjectManager Manager { get; }

	/// <summary>
	///		Nombre del archivo
	/// </summary>
	internal string FileName { get; }

	/// <summary>
	///		Directorio del proyecto
	/// </summary>
	public string ProjectPath
	{ 
		get
		{
			if (!string.IsNullOrWhiteSpace(FileName))
				return Path.GetDirectoryName(FileName) ?? string.Empty;
			else
				return string.Empty;
		}
	}

	/// <summary>
	///		Repositorios de los procesadores
	/// </summary>
	internal List<IProcessorRepository> Repositories { get; } = new();
}