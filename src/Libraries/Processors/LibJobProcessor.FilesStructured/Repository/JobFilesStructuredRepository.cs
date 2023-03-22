using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibJobProcessor.FilesStructured.Models.Sentences;
using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Repository;

namespace Bau.Libraries.LibJobProcessor.FilesStructured.Repository;

/// <summary>
///		Clase para cargar datos de proceso sobre archivos estructurados
/// </summary>
public class JobFilesStructuredRepository : BaseJobRepository
{
	// Constantes privadas
	private const string TagColumn = "Column";
	private const string TagKey = "Key";
	private const string TagName = "Name";
	private const string TagType = "Type";
	private const string TagOpenExcelReader = "OpenExcelReader";
	private const string TagSheetIndex = "SheetIndex";
	private const string TagWithHeader = "WithHeader";
	private const string TagOpenParquetReader = "OpenParquetReader";
	private const string TagOpenCsvReader = "OpenCsvReader";
	private const string TagSource = "Source";
	private const string TagTarget = "Target";
	private const string TagSeparator = "Separator";
	private const string TagDateFormat = "DateFormat";
	private const string TagDecimalSeparator = "DecimalSeparator";
	private const string TagThousandsSeparator = "ThousandsSeparator";
	private const string TagTrueValue = "TrueValue";
	private const string TagFalseValue = "FalseValue";
	private const string TagWriteToCsv = "WriteToCsv";
	private const string TagWriteToParquet = "WriteToParquet";
	private const string TagWriteToExcel = "WriteToExcel";
	private const string TagMapping = "Mapping";

	public JobFilesStructuredRepository(IProgramRepository programRepository) : base(programRepository)
	{
	}

	/// <summary>
	///		Carga los datos de una sentencia
	/// </summary>
	public override SentenceBase? Load(MLNode rootML)
	{
		return rootML.Name switch
					{
						TagOpenCsvReader => LoadOpenCsvReaderSentence(rootML),
						TagOpenParquetReader => LoadOpenParquetReaderSentence(rootML),
						TagOpenExcelReader => LoadOpenExcelReaderSentence(rootML),
						TagWriteToCsv => LoadWriteToCsvSentence(rootML),
						TagWriteToParquet => LoadWriteToParquetSentence(rootML),
						TagWriteToExcel => LoadWriteToExcelSentence(rootML),
						_ => null
					};
	}

	/// <summary>
	///		Carga la sentencia de apertura de un IDataReader sobre un archivo CSV
	/// </summary>
	private SentenceBase LoadOpenCsvReaderSentence(MLNode rootML)
	{
		OpenCsvReaderSentence sentence = new(rootML.Attributes[TagKey].Value.TrimIgnoreNull(), 
											 rootML.Attributes[TagSource].Value.TrimIgnoreNull(), GetCsvProperties(rootML));

			// Añade las columnas
			sentence.Columns.AddRange(LoadColumns(rootML));
			// Añade las sentencias
			sentence.Sentences.AddRange(LoadSentences(rootML.Nodes, TagColumn));
			// Devuelve la sentencia leida
			return sentence;
	}

	/// <summary>
	///		Carga la sentencia de escritura sobre un archivo CSV
	/// </summary>
	private SentenceBase LoadWriteToCsvSentence(MLNode rootML)
	{
		WriteToCsvSentence sentence = new(rootML.Attributes[TagSource].Value.TrimIgnoreNull(),
										  rootML.Attributes[TagTarget].Value.TrimIgnoreNull(),
										  GetCsvProperties(rootML));

			// Añade las columnas y los mapeos
			sentence.Columns.AddRange(LoadColumns(rootML));
			sentence.Mappings.AddRange(LoadMappings(rootML));
			// Devuelve la sentencia leida
			return sentence;
	}

	/// <summary>
	///		Obtiene las propiedades de un archivo CSV
	/// </summary>
	private CsvProperties GetCsvProperties(MLNode nodeML)
	{
		return new CsvProperties(nodeML.Attributes[TagWithHeader].Value.GetBool(true),
								 nodeML.Attributes[TagSeparator].Value.TrimIgnoreNull(),
								 nodeML.Attributes[TagDateFormat].Value.TrimIgnoreNull(),
								 nodeML.Attributes[TagDecimalSeparator].Value.TrimIgnoreNull(),
								 nodeML.Attributes[TagThousandsSeparator].Value.TrimIgnoreNull(),
								 nodeML.Attributes[TagTrueValue].Value.TrimIgnoreNull(),
								 nodeML.Attributes[TagFalseValue].Value.TrimIgnoreNull());
	}

	/// <summary>
	///		Carga la sentencia de apertura de un IDataReader sobre un archivo Parquet
	/// </summary>
	private SentenceBase LoadOpenParquetReaderSentence(MLNode rootML)
	{
		OpenParquetReaderSentence sentence = new(rootML.Attributes[TagKey].Value.TrimIgnoreNull(), 
												 rootML.Attributes[TagSource].Value.TrimIgnoreNull());

			// Añade las columnas
			sentence.Columns.AddRange(LoadColumns(rootML));
			// Añade las sentencias
			sentence.Sentences.AddRange(LoadSentences(rootML.Nodes, TagColumn));
			// Devuelve la sentencia leida
			return sentence;
	}

	/// <summary>
	///		Carga la sentencia de escritura sobre un archivo parquet
	/// </summary>
	private SentenceBase LoadWriteToParquetSentence(MLNode rootML)
	{
		WriteToParquetSentence sentence = new(rootML.Attributes[TagSource].Value.TrimIgnoreNull(),
											  rootML.Attributes[TagTarget].Value.TrimIgnoreNull());

			// Añade las columnas y los mapeos
			sentence.Columns.AddRange(LoadColumns(rootML));
			sentence.Mappings.AddRange(LoadMappings(rootML));
			// Devuelve la sentencia leida
			return sentence;
	}

	/// <summary>
	///		Carga la sentencia de apertura de un IDataReader sobre un archivo Excel
	/// </summary>
	private SentenceBase LoadOpenExcelReaderSentence(MLNode rootML)
	{
		OpenExcelReaderSentence sentence = new(rootML.Attributes[TagKey].Value.TrimIgnoreNull(), 
											   rootML.Attributes[TagSource].Value.TrimIgnoreNull(), GetExcelProperties(rootML));

			// Añade las columnas
			sentence.Columns.AddRange(LoadColumns(rootML));
			// Añade las sentencias
			sentence.Sentences.AddRange(LoadSentences(rootML.Nodes, TagColumn));
			// Devuelve la sentencia leida
			return sentence;
	}

	/// <summary>
	///		Carga la sentencia de escritura sobre un archivo Excel
	/// </summary>
	private SentenceBase LoadWriteToExcelSentence(MLNode rootML)
	{
		WriteToExcelSentence sentence = new(rootML.Attributes[TagSource].Value.TrimIgnoreNull(),
											rootML.Attributes[TagTarget].Value.TrimIgnoreNull(),
											GetExcelProperties(rootML));

			// Añade las columnas y los mapeos
			sentence.Columns.AddRange(LoadColumns(rootML));
			sentence.Mappings.AddRange(LoadMappings(rootML));
			// Devuelve la sentencia leida
			return sentence;
	}

	/// <summary>
	///		Obtiene las propiedades de un archivo Excel
	/// </summary>
	private ExcelProperties GetExcelProperties(MLNode nodeML)
	{
		return new ExcelProperties(nodeML.Attributes[TagSheetIndex].Value.GetInt(1),
								   nodeML.Attributes[TagWithHeader].Value.GetBool(true));
	}

	/// <summary>
	///		Carga la definición de columnas de un archivo
	/// </summary>
	private List<FileColumnModel> LoadColumns(MLNode rootML)
	{
		List<FileColumnModel> columns = new();

			// Carga la lista de columnas
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagColumn)
					columns.Add(new FileColumnModel(nodeML.Attributes[TagName].Value.TrimIgnoreNull(),
													nodeML.Attributes[TagType].Value.GetEnum(FileColumnModel.ColumnType.String)));
			// Devuelve la lista de columnas
			return columns;
	}

	/// <summary>
	///		Carga la lista de mapeos
	/// </summary>
	private List<(string source, string target)> LoadMappings(MLNode rootML)
	{
		List<(string source, string target)> mappings = new();

			// Carga los mapeos
			foreach (MLNode nodeML in rootML.Nodes)
				if (nodeML.Name == TagMapping)
					mappings.Add((nodeML.Attributes[TagSource].Value.TrimIgnoreNull(),
								  nodeML.Attributes[TagTarget].Value.TrimIgnoreNull()));
			// Devuelve la lista de mapeos
			return mappings;
	}
}