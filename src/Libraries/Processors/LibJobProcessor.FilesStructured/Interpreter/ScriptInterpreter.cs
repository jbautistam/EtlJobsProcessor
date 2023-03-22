using System.Data;

using Bau.Libraries.LibJobProcessor.FilesStructured.Models.Sentences;
using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Interpreter;
using Bau.Libraries.LibInterpreter.Interpreter.Context.Variables;
using Bau.Libraries.LibJobProcessor.Core.Models.Files;
using Bau.Libraries.LibJobProcessor.FilesStructured.Interpreter.Controllers;

namespace Bau.Libraries.LibJobProcessor.FilesStructured.Interpreter;

/// <summary>
///		Intérprete del script
/// </summary>
internal class ScriptInterpreter : BaseJobSentenceIntepreter
{
	public ScriptInterpreter(IProgramInterpreter programInterpreter) : base(programInterpreter)
	{
	}

	/// <summary>
	///		Inicializa el intérprete
	/// </summary>
	public override async Task InitializeAsync(CancellationToken cancellationToken)
	{
		await Task.Delay(1, cancellationToken);
	}

	/// <summary>
	///		Procesa una sentencia
	/// </summary>
	public async override Task<bool> ProcessAsync(SentenceBase sentenceBase, CancellationToken cancellationToken)
	{
		bool executed = true;

			// Ejecuta la sentencia
			switch (sentenceBase)
			{
				case OpenCsvReaderSentence sentence:
						await OpenFileReader(sentence, cancellationToken);
					break;
				case OpenParquetReaderSentence sentence:
						await OpenFileReader(sentence, cancellationToken);
					break;
				case OpenExcelReaderSentence sentence:
						await OpenFileReader(sentence, cancellationToken);
					break;
				case WriteToCsvSentence sentence:
						await WriteToFileAsync(sentence, cancellationToken);
					break;
				case WriteToParquetSentence sentence:
						await WriteToFileAsync(sentence, cancellationToken);
					break;
				case WriteToExcelSentence sentence:
						await WriteToFileAsync(sentence, cancellationToken);
					break;
				default:
						executed = false;
					break;
			}
			// Devuelve el valor que indica si ha ejecutado la sentencia
			return executed;
	}

	/// <summary>
	///		Abre un IDateaReader sobre un archivo
	/// </summary>
	private async Task OpenFileReader(BaseOpenFileReaderSentence sentence, CancellationToken cancellationToken)
	{
		VariableModel? file = ProgramInterpreter.GetVariable(sentence.Source);

			// Log
			LogInfo($"Open {sentence.Type.ToString()} reader {sentence.Source}");
			// Procesa la apertura
			if (file is null)
				AddError($"Can't find the file {sentence.Source}");
			else if (file.Type != LibInterpreter.Models.Symbols.SymbolModel.SymbolType.Object)
				AddError($"The variable {sentence.Source} is not file");
			else if (file.Value is StreamUnionModel streamUnion)
			{
				if (streamUnion.Stream is null)
					AddError($"The variable {sentence.Source} hasn't a reader stream");
				else
					using (IDataReader reader = await GetFileController(sentence).OpenReaderAsync(streamUnion, cancellationToken))
					{
						VariableModel stream = ProgramInterpreter.AddVariable(sentence.Name, LibInterpreter.Models.Symbols.SymbolModel.SymbolType.Object, reader);

							// Ejecuta las sentencias
							await ProgramInterpreter.ProcessAsync(sentence.Sentences, cancellationToken);
							// Elimina la variable
							ProgramInterpreter.RemoveVariable(stream.Name);	
					}
			}
			else
				AddError($"The variable {sentence.Source} doesn't contains an stream");
			// Log
			LogInfo($"End {sentence.Type.ToString()} reader {sentence.Source}");
	}

	/// <summary>
	///		Escribe un IDataReader sobre un archivo
	/// </summary>
	private async Task WriteToFileAsync(BaseWriteToSentence sentence, CancellationToken cancellationToken)
	{
		VariableModel? source = ProgramInterpreter.GetVariable(sentence.Source);

			// Log
			LogInfo($"Write to {sentence.Target} ({sentence.Type.ToString()}) from {sentence.Source}");
			// Procesa la apertura
			if (source is null)
				AddError($"Can't find the reader {sentence.Source}");
			else if (source.Type != LibInterpreter.Models.Symbols.SymbolModel.SymbolType.Object || source.Value is not IDataReader reader)
				AddError($"The variable {sentence.Source} is not reader");
			else
			{
				VariableModel? target = ProgramInterpreter.GetVariable(sentence.Target);

					if (target is null)
						AddError($"Can't find the file {sentence.Target}");
					else if (target.Value is StreamUnionModel streamUnion)
					{
						if (streamUnion.Stream is null)
							AddError($"The variable {sentence.Target} hasn't a write stream");
						else
							await GetFileController(sentence).WriteToAsync(reader, streamUnion, cancellationToken);
					}
					else
						AddError($"The variable {sentence.Target} doesn't contains an stream");
			}
			// Log
			LogInfo($"End write to {sentence.Target} ({sentence.Type.ToString()}) from {sentence.Source}");
	}

	/// <summary>
	///		Obtiene el controlador adecuado para el archivo
	/// </summary>
	private BaseFileController GetFileController(BaseOpenFileReaderSentence sentenceBase)
	{
		return sentenceBase switch
					{
						OpenCsvReaderSentence sentence => new CsvController(this, sentence, null),
						OpenParquetReaderSentence sentence => new ParquetController(this, sentence, null),
						OpenExcelReaderSentence sentence => new ExcelController(this, sentence, null),
						_ => throw new NotImplementedException($"Sentence type unknown: {sentenceBase.GetType().ToString()}")
					};
	}

	/// <summary>
	///		Obtiene el controlador adecuado para el archivo
	/// </summary>
	private BaseFileController GetFileController(BaseWriteToSentence sentenceBase)
	{
		return sentenceBase switch
					{
						WriteToCsvSentence sentence => new CsvController(this, null, sentence),
						WriteToParquetSentence sentence => new ParquetController(this, null, sentence),
						WriteToExcelSentence sentence => new ExcelController(this, null, sentence),
						_ => throw new NotImplementedException($"Sentence type unknown: {sentenceBase.GetType().ToString()}")
					};
	}
}