using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibInterpreter.Models.Symbols;
using Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Parameters;
using Bau.Libraries.LibJobProcessor.Database.Models.Sentences;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Repository;

namespace Bau.Libraries.LibJobProcessor.Database.Repository;

/// <summary>
///		Clase de lectura de los scripts
/// </summary>
internal class DbScriptRepository : BaseJobRepository
{
    // Constantes privadas
    private const string TagProviderFile = "ProviderFile";
    private const string TagProviderFileKey = "ProviderFileKey";
    private const string TagFileName = "FileName";
    private const string TagSentenceExecute = "Execute";
    private const string TagSentenceBulkCopy = "BulkCopy";
    private const string TagTable = "Table";
    private const string TagBatchSize = "BatchSize";
    private const string TagProviderCommand = "Command";
    private const string TagSource = "Source";
    private const string TagTarget = "Target";
    private const string TagType = "Type";
    private const string TagName = "Name";
    private const string TagParameter = "Parameter";
    private const string TagDefault = "Default";
    private const string TagEmptyData = "EmptyData";
    private const string TagWithData = "WithData";
    private const string TagSentenceForEach = "ForEach";
    private const string TagThen = "Then";
    private const string TagElse = "Else";
    private const string TagVariable = "Variable";
    private const string TagSentenceIfExists = "IfExists";
    private const string TagSentenceBeginTransaction = "BeginTransaction";
    private const string TagSentenceCommitTransaction = "CommitTransaction";
    private const string TagSentenceRollbackTransaction = "RollbackTransaction";
    private const string TagSentenceExecuteScript = "ExecuteScript";
    private const string TagMustParse = "MustParse";
    private const string TagSkipParameters = "SkipParameters";
    private const string TagMapping = "Mapping";
    private const string TagSentenceOpenReader = "OpenSqlReader";
    private const string TagKey = "Key";
    private const string TagSentenceWriteToDatabase = "WriteToDatabase";

    public DbScriptRepository(IProgramRepository programRepository) : base(programRepository) {}

    /// <summary>
    ///		Carga una sentencia del nodo
    /// </summary>
    public override SentenceBase? Load(MLNode rootML)
    {
		return rootML.Name switch
		{
			TagSentenceExecute => LoadSentenceExecute(rootML),
			TagSentenceBulkCopy => LoadSentenceBulkCopy(rootML),
			TagSentenceForEach => LoadSentenceForEach(rootML),
			TagSentenceIfExists => LoadSentenceIfExists(rootML),
			TagSentenceBeginTransaction => LoadSentenceBatch(rootML, SentenceDataBatch.BatchCommand.BeginTransaction),
			TagSentenceCommitTransaction => LoadSentenceBatch(rootML, SentenceDataBatch.BatchCommand.CommitTransaction),
			TagSentenceRollbackTransaction => LoadSentenceBatch(rootML, SentenceDataBatch.BatchCommand.RollbackTransaction),
			TagSentenceExecuteScript => LoadSentenceExecuteScript(rootML),
            TagSentenceOpenReader => LoadSentenceOpenReader(rootML),
            TagSentenceWriteToDatabase => LoadSentenceWriteToDatabase(rootML),
			_ => null,
		};
	}

    /// <summary>
    ///		Carga una sentencia de inicio o fin de lote
    /// </summary>
    private SentenceBase LoadSentenceBatch(MLNode rootML, SentenceDataBatch.BatchCommand type)
    {
        return new SentenceDataBatch(rootML.Attributes[TagTarget].Value.TrimIgnoreNull(), type);
    }

    /// <summary>
    ///		Carga los datos de una sentencia <see cref="SentenceForEach"/>
    /// </summary>
    private SentenceBase LoadSentenceForEach(MLNode rootML)
    {
        SentenceForEach sentence = new(rootML.Attributes[TagSource].Value.TrimIgnoreNull(),
                                       GetProviderCommand(rootML, TagProviderCommand));

            // Carga las instrucciones a ejecutar cuando hay o no hay datos
            foreach (MLNode nodeML in rootML.Nodes)
                switch (nodeML.Name)
                {
                    case TagEmptyData:
                            sentence.SentencesEmptyData.AddRange(LoadSentences(nodeML.Nodes));
                        break;
                    case TagWithData:
                            sentence.SentencesWithData.AddRange(LoadSentences(nodeML.Nodes));
                        break;
                }
            // Devuelve la sentencia
            return sentence;
    }

    /// <summary>
    ///     Carga la sentencia de apertura de un IDataReader
    /// </summary>
    private SentenceBase LoadSentenceOpenReader(MLNode rootML)
    {
        SentenceOpenReader sentence = new(rootML.Attributes[TagKey].Value.TrimIgnoreNull(), 
                                          rootML.Attributes[TagSource].Value.TrimIgnoreNull(),
                                          GetProviderCommand(rootML, TagProviderCommand));

            // Carga las sentencias hija
            if (rootML.Nodes.Count > 0)
                sentence.Sentences.AddRange(LoadSentences(rootML.Nodes, TagProviderCommand));
            // Devuelve la sentencia
            return sentence;
    }

    /// <summary>
    ///     Carga la sentencia de escritura a la base de datos
    /// </summary>
    private SentenceBase LoadSentenceWriteToDatabase(MLNode rootML)
    {
        SentenceWriteTo sentence = new (rootML.Attributes[TagSource].Value.TrimIgnoreNull(),
                                        rootML.Attributes[TagTarget].Value.TrimIgnoreNull(),
                                        rootML.Attributes[TagTable].Value.TrimIgnoreNull(),
                                        rootML.Attributes[TagBatchSize].Value.GetInt(100_000),
                                        ProgramRepository.GetTimeout(rootML, TimeSpan.FromMinutes(10)));

            // Añade los mapeos
            LoadMappings(sentence.Mappings, rootML);
            // Devuelve la sentencia
            return sentence;
    }

    /// <summary>
    ///		Sentencia de ejecución sobre el proveedor
    /// </summary>
    private SentenceBase LoadSentenceExecute(MLNode rootML)
    {
        return new SentenceExecute(rootML.Attributes[TagTarget].Value.TrimIgnoreNull(),
                                   GetProviderCommand(rootML, TagProviderCommand));
    }

    /// <summary>
    ///		Sentencia de ejecución de un script
    /// </summary>
    private SentenceBase LoadSentenceExecuteScript(MLNode rootML)
    {
        SentenceExecuteScript sentence = new(rootML.Attributes[TagProviderFile].Value.TrimIgnoreNull(),
                                             rootML.Attributes[TagProviderFileKey].Value.TrimIgnoreNull(),
                                             rootML.Attributes[TagTarget].Value.TrimIgnoreNull(),
                                             rootML.Attributes[TagFileName].Value.TrimIgnoreNull());

            // Asigna las propiedades
            sentence.MustParse = rootML.Attributes[TagMustParse].Value.GetBool();
            sentence.SkipParameters = rootML.Attributes[TagSkipParameters].Value.GetBool();
            // Asigna los mapeos de variables
            foreach (MLNode nodeML in rootML.Nodes)
                if (nodeML.Name == TagMapping)
                    sentence.Mapping.Add((nodeML.Attributes[TagVariable].Value.TrimIgnoreNull(), nodeML.Attributes[TagTarget].Value.TrimIgnoreNull()));
            // Devuelve la sentencia creada
            return sentence;
    }

    /// <summary>
    ///		Carga una sentencia de copia masiva
    /// </summary>
    private SentenceBulkCopy LoadSentenceBulkCopy(MLNode rootML)
    {
        SentenceBulkCopy sentence = new(rootML.Attributes[TagSource].Value.TrimIgnoreNull(),
                                            rootML.Attributes[TagTarget].Value.TrimIgnoreNull(),
                                            rootML.Attributes[TagTable].Value.TrimIgnoreNull(),
                                            GetProviderCommand(rootML, TagProviderCommand))
                                        {
                                            BatchSize = rootML.Attributes[TagBatchSize].Value.GetInt(30_000)
                                        };

            // Obtiene las columnas de mapeo
            LoadMappings(sentence.Mappings, rootML);
            // Devuelve la sentencia
            return sentence;
    }

    /// <summary>
    ///		Carga el diccionario de mapeo
    /// </summary>
    private void LoadMappings(Dictionary<string, string> mappings, MLNode rootML)
    {
        if (rootML.Nodes.Count != 0)
            foreach (MLNode node in rootML.Nodes)
                if (node.Name == TagMapping)
                    mappings.Add(node.Attributes[TagSource].Value.TrimIgnoreNull().ToUpperInvariant(),
                                 node.Attributes[TagTarget].Value.TrimIgnoreNull());
    }

    /// <summary>
    ///		Carga la sentencia que comprueba si existe un valor
    /// </summary>
    private SentenceBase LoadSentenceIfExists(MLNode rootML)
    {
        SentenceIfExists sentence = new(rootML.Attributes[TagSource].Value.TrimIgnoreNull(),
                                        GetProviderCommand(rootML, TagProviderCommand));
        bool loaded = false;

            // Carga las instrucciones a ejecutar cuando existe o no existe el dato
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
            // Si no se ha cargado nada, se carga todo el contenido sobre el then
            if (!loaded)
                sentence.SentencesThen.AddRange(LoadSentences(rootML.Nodes, TagProviderCommand));
            // Devuelve la sentencia
            return sentence;
    }

    /// <summary>
    ///		Carga una sentencia que se envía a un proveedor
    /// </summary>
    private SqlSentenceModel GetProviderCommand(MLNode rootML, string tagCommand)
    {
        if (rootML.Nodes.Count == 0 && !string.IsNullOrWhiteSpace(rootML.Value))
            return new SqlSentenceModel(rootML.Value.TrimIgnoreNull(), ProgramRepository.GetTimeout(rootML, TimeSpan.FromMinutes(5)));
        else
        {
            SqlSentenceModel? sentence = null;

                // Primero obtiene el comando
                foreach (MLNode nodeML in rootML.Nodes)
                    if (nodeML.Name == tagCommand)
                        sentence = new SqlSentenceModel(nodeML.Value.TrimIgnoreNull(), ProgramRepository.GetTimeout(rootML, TimeSpan.FromMinutes(5)));
                // Si ha generado la sentencia, añade los argumentos
                if (sentence is not null)
                {
                    // Añade los filtros
                    foreach (MLNode nodeML in rootML.Nodes)
                        if (nodeML.Name == TagParameter)
                            sentence.Parameters.Add(LoadParameter(nodeML));
                    // Devuelve la sentencia
                    return sentence;
                }
                else
                    throw new NotImplementedException("Can't find database command");
        }
    }

    /// <summary>
    ///		Carga los datos de un filtro
    /// </summary>
    private SqlSentenceParameterModel LoadParameter(MLNode rootML)
    {
        SqlSentenceParameterModel parameter = new(rootML.Attributes[TagName].Value.TrimIgnoreNull(),
                                                  rootML.Attributes[TagType].Value.GetEnum(SymbolModel.SymbolType.Unknown),
                                                  null);

            // Añade los datos del filtro
            parameter.VariableName = rootML.Attributes[TagVariable].Value.TrimIgnoreNull();
            // Obtiene el valor por defecto
            parameter.Default = ProgramRepository.ConvertStringValue(parameter.Type, rootML.Attributes[TagDefault].Value.TrimIgnoreNull());
            // Devuelve los datos del filtro
            return parameter;
    }
}
