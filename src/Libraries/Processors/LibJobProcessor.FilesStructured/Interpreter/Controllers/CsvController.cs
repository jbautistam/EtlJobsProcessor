using System.Data;
using Bau.Libraries.LibCsvFiles.Models;
using Bau.Libraries.LibJobProcessor.FilesStructured.Models.Sentences;

namespace Bau.Libraries.LibJobProcessor.FilesStructured.Interpreter.Controllers;

/// <summary>
///		Controlador para archivos CSV
/// </summary>
internal class CsvController : BaseFileController
{
	internal CsvController(ScriptInterpreter interpreter, OpenCsvReaderSentence? sentenceRead, WriteToCsvSentence? sentenceWrite) : base(interpreter) 
	{
		SentenceRead = sentenceRead ?? default!;
		SentenceWrite = sentenceWrite ?? default!;
	}
 
	/// <summary>
	///		Abre un IDataReader
	/// </summary>
	internal override async Task<IDataReader> OpenReaderAsync(Core.Models.Files.StreamUnionModel streamUnion, CancellationToken cancellationToken)
	{
		LibCsvFiles.CsvReader reader = new(GetCsvFileModel(SentenceRead.Properties), GetCsvColumns(SentenceRead.Columns));

			// Evita las advertencias
			await Task.Delay(1, cancellationToken);
			// Abre el archivo
			reader.Open(new StreamReader(streamUnion.Stream));
			// Devuelve el lector
			return reader;
	}

	/// <summary>
	///		Escribe a un archivo
	/// </summary>
	internal override async Task WriteToAsync(IDataReader reader, Core.Models.Files.StreamUnionModel streamTarget, CancellationToken cancellationToken)
	{
		try
		{
			using (LibCsvFiles.CsvWriter writer = new LibCsvFiles.CsvWriter(GetCsvFileModel(SentenceWrite.Properties)))
			{
				// Evita el error de await
				await Task.Delay(1, cancellationToken);
				// Abre el archivo
				writer.Open(new StreamWriter(streamTarget.Stream));
				// Escribe las cabeceras
				writer.WriteHeaders(GetCsvColumnsOutput(reader, SentenceWrite.Columns, SentenceWrite.Mappings));
				// Escribe las filas de datos
				while (reader.Read())
					writer.WriteRow(GetValues(reader, SentenceWrite.Mappings));
				// Escribe en el archivo de salida
				writer.Flush();
				writer.Close();
			}
		}
		catch (Exception exception)
		{
			Interpreter.AddError($"Error when convert '{SentenceWrite.Source}' to '{SentenceWrite.Target}'", exception);
		}
	}

	/// <summary>
	///		Convierte las propiedades del archivo
	/// </summary>
	private FileModel GetCsvFileModel(CsvProperties properties)
	{
		return new FileModel
						{
							DateFormat = properties.DateFormat,
							DecimalSeparator = properties.DecimalSeparator,
							ThousandsSeparator = properties.ThousandsSeparator,
							TrueValue = properties.TrueValue,
							FalseValue = properties.FalseValue,
							Separator = properties.Separator,
							WithHeader = properties.WithHeader
						};
	}

	/// <summary>
	///		Obtiene la lista de columnas a partir de los campos del IDataReader y de los datos de mapeo
	/// </summary>
	private	List<ColumnModel> GetCsvColumnsOutput(IDataReader reader, List<FileColumnModel> columns, List<(string source, string target)> mappings)
	{
		List<ColumnModel> csvColumnsInput = GetCsvColumns(columns);
		List<ColumnModel> csvColumnsOutput = new();

			// Se crea la lista de columnas de entrada. Si no se han definido especialmente, se recogen del IDataReader
			if (columns.Count == 0)
				for (int index = 0; index < reader.FieldCount; index++)
					csvColumnsInput.Add(new ColumnModel
										{
											Name = reader.GetName(index), 
											Type = ColumnModel.ColumnType.Unknown
										}
								  );
			// Añade las columnas que estén en los mapeos o todas las columnas de entrada
			if (mappings.Count > 0)
				foreach ((string _, string target) in mappings)
					csvColumnsOutput.Add(new ColumnModel
											{
												Name = target,
												Type = ColumnModel.ColumnType.Unknown
											}
								  );
			else
				csvColumnsOutput.AddRange(csvColumnsInput);
			// Devuelve la lista de columnas
			return csvColumnsOutput;
	}

	/// <summary>
	///		Convierte las columnas del archivo
	/// </summary>
	private List<ColumnModel> GetCsvColumns(List<FileColumnModel> columns)
	{
		List<ColumnModel> csvColumns = new();

			// Convierte las columnas
			foreach (FileColumnModel column in columns)
				csvColumns.Add(new ColumnModel
											{
												Name = column.Name,
												Type = Convert(column.Type)
											}
							  );
			// Devuelve las columnas
			return csvColumns;

		// Convierte el tipo de columna
		ColumnModel.ColumnType Convert(FileColumnModel.ColumnType type)
		{
			return type switch
					{
						FileColumnModel.ColumnType.Boolean => ColumnModel.ColumnType.Boolean,
						FileColumnModel.ColumnType.Integer => ColumnModel.ColumnType.Integer,
						FileColumnModel.ColumnType.Decimal => ColumnModel.ColumnType.Decimal,
						FileColumnModel.ColumnType.DateTime => ColumnModel.ColumnType.DateTime,
						FileColumnModel.ColumnType.String => ColumnModel.ColumnType.String,
						_ => ColumnModel.ColumnType.Unknown
					};
		}
	}

	/// <summary>
	///		Obtiene los valores del IDataReader correspondientes a los valores de salida
	/// </summary>
	private List<object?> GetValues(IDataReader reader, List<(string source, string target)> mappings)
	{
		List<object?> values = new();

			// Obtiene los datos
			foreach ((string source, string target) in mappings)
			{
				object? value = GetValue(source);

					// Convierte el valor de null
					if (value is DBNull)
						value = null;
					// Añade el valor a la lista
					values.Add(value);
			}
			// Devuelve la lista de valores
			return values;

			// Obtiene un valor del IDataReader
			object? GetValue(string name)
			{
				// Busca el nombre del campo en el IDataReader
				for (int index = 0; index < reader.FieldCount; index++)
					if (reader.GetName(index).Equals(name, StringComparison.CurrentCultureIgnoreCase))
						return reader.GetValue(index);
				// Si ha llegado hasta aquí es porque no ha encontrado nada
				return null;
			}
	}

	/// <summary>
	///		Datos de la sentencia de lectura
	/// </summary>
	internal OpenCsvReaderSentence SentenceRead { get; }

	/// <summary>
	///		Datos de la sentencia de escritura
	/// </summary>
	internal WriteToCsvSentence SentenceWrite { get; }
}
