using System.Data;

using Bau.Libraries.LibInterpreter.Interpreter.Context.Variables;
using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibInterpreter.Models.Symbols;
using Bau.Libraries.DbAggregator.Models;
using Bau.Libraries.DbAggregator;
using Bau.Libraries.LibDataStructures.Collections;
using Bau.Libraries.LibDbScripts.Parser;
using Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Parameters;
using Bau.Libraries.LibJobProcessor.Database.Models.Sentences;
using Bau.Libraries.LibJobProcessor.Database.Interpreter.Context;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Interpreter;
using Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Connections;

namespace Bau.Libraries.LibJobProcessor.Database.Interpreter;

/// <summary>
///		Clase para lectura y ejecución de un script sobre base de datos
/// </summary>
internal class DbScriptInterpreter : BaseJobSentenceIntepreter
{
    internal DbScriptInterpreter(IProgramInterpreter programInterpreter) : base(programInterpreter)
    {
    }

    /// <summary>
    ///		Inicializa el intérprete
    /// </summary>
    public async override Task InitializeAsync(CancellationToken cancellationToken)
    {
        // Evita las advertencias
        await Task.Delay(1, cancellationToken);
        // Limpia las transacciones
        Transactions.Clear();
        // Obtiene el manager de proveedores a base de datos
        GetProviderManager();
    }

    /// <summary>
    ///		Obtiene e inicializa el controlador de proveedores de datos
    /// </summary>
    private void GetProviderManager()
    {
        // Crea los proveedores de las conexiones que aparecen en el contexto
        foreach (Core.Models.Jobs.JobContextModel context in ProgramInterpreter.GetContexts())
            if (context is DataBaseConnectionModel dataBaseConnection)
            {
                ConnectionModel dbConnection = new(dataBaseConnection.Key, dataBaseConnection.Type);

                    // Añade los parámetros
                    foreach ((string key, string parameter) in dataBaseConnection.Parameters.Enumerate())
                        dbConnection.Parameters.Add(key, ApplyVariables(parameter) ?? string.Empty);
                    // Añade la conexión
                    DataProviderManager.AddConnection(dbConnection);
            }
    }

    /// <summary>
    ///		Ejecuta una sentencia
    /// </summary>
    public async override Task<bool> ProcessAsync(SentenceBase sentenceBase, CancellationToken cancellationToken)
    {
        bool executed = true;

            // Ejecuta la sentencia
            switch (sentenceBase)
            {
                case SentenceForEach sentence:
                        await ExecuteForEachAsync(sentence, cancellationToken);
                    break;
                case SentenceIfExists sentence:
                        await ExecuteIfExistsAsync(sentence, cancellationToken);
                    break;
                case SentenceExecute sentence:
                        ExecuteDataCommand(sentence);
                    break;
                case SentenceExecuteScript sentence:
                        ExecuteScriptSql(sentence);
                    break;
                case SentenceDataBatch sentence:
                        ExecuteDataBatch(sentence);
                    break;
                case SentenceBulkCopy sentence:
                        ExecuteBulkCopy(sentence);
                    break;
                case SentenceOpenReader sentence:
                        await ExecuteOpenReaderAsync(sentence, cancellationToken);
                    break;
                case SentenceWriteTo sentence:
                        await ExecuteWriteToAsync(sentence, cancellationToken);
                    break;
                default:
                        executed = false;
                    break;
            }
            // Devuelve el valor que indica si se ha ejecutado correctamente
            return executed;
    }

    /// <summary>
    ///		Ejecuta una sentencia foreach
    /// </summary>
    private async Task ExecuteForEachAsync(SentenceForEach sentence, CancellationToken cancellationToken)
    {
        ProviderModel? provider = GetProvider(sentence.Source);

            // Log
            LogInfo("Start foreach sentence");
            // Ejecuta la sentencia
            if (provider is null)
                AddError($"Can't find provider {sentence.Source}");
            else
            {
                CommandModel? command = ConvertProviderCommand(sentence.Command, out string error);

                    if (!string.IsNullOrWhiteSpace(error))
                        AddError($"Error when convert command. {error}");
                    else if (command is null)
                        AddError($"Can't convert command: {sentence.Command}");
                    else
                    {
                        int startRow = 0;

                            // Ejecuta la consulta sobre el proveedor
                            try
                            {
                                foreach (DataTable table in GetDataTable(provider, command))
                                    if (table.Rows.Count > 0)
                                    {
                                        // Ejecuta las instrucciones
                                        foreach (DataRow row in table.Rows)
                                        {
                                            // Crea un contexto
                                            ProgramInterpreter.AddScope();
                                            // Añade el índice de fila a la lista de variables
                                            ProgramInterpreter.AddVariable("RowIndex", SymbolModel.SymbolType.Numeric, 
                                                                           startRow + table.Rows.IndexOf(row));
                                            // Añade las columnas
                                            for (int index = 0; index < table.Columns.Count; index++)
                                                if (row.IsNull(index) || row[index] == null)
                                                    ProgramInterpreter.AddVariable(table.Columns[index].ColumnName, SymbolModel.SymbolType.Unknown, null);
                                                else
                                                    ProgramInterpreter.AddVariable(table.Columns[index].ColumnName, SymbolModel.SymbolType.Unknown, row[index]);
                                            // Ejecuta las sentencias
                                            await ProcessAsync(sentence.SentencesWithData, cancellationToken);
                                            // Limpia el contexto
                                            ProgramInterpreter.RemoveScope();
                                        }
                                        // Añade el total de filas
                                        startRow += table.Rows.Count;
                                    }
                            }
                            catch (Exception exception)
                            {
                                AddError($"Error when load data. {exception.Message}");
                            }
                            // Si no se han encontrado datos, ejecuta las sentencias adecuadas
                            if (startRow == 0)
                                await ProcessAsync(sentence.SentencesEmptyData, cancellationToken);
                    }
            }
    }

    /// <summary>
    ///		Ejecuta una sentencia de copia masiva
    /// </summary>
    private void ExecuteBulkCopy(SentenceBulkCopy sentence)
    {
        ProviderModel? source = GetProvider(sentence.Source);
        ProviderModel? target = GetProvider(sentence.Target);

            LogInfo($"Start bulckcopy to table {sentence.Table}");
            if (source is null)
                AddError($"Can't find source provider: {sentence.Source}");
            else if (target is null)
                AddError($"Can't find target provider: {sentence.Target}");
            else if (string.IsNullOrWhiteSpace(sentence.Table))
                AddError("The target table is undefined at BulkCopy sentence");
            else
            {
                CommandModel? command = ConvertProviderCommand(sentence.Command, out string error);

                    if (!string.IsNullOrWhiteSpace(error))
                        AddError($"Error when convert command. {error}");
                    else if (command is null)
                        AddError($"Error when convert command. {sentence.Command.Sql}");
                    else // Ejecuta la consulta sobre el proveedor
                        try
                        {
                            using (IDataReader reader = source.OpenReader(command, TimeSpan.FromMinutes(10)))
                            {
                                target.BulkCopy(reader, sentence.Table, sentence.Mappings, sentence.BatchSize, TimeSpan.FromMinutes(15));
                            }
                        }
                        catch (Exception exception)
                        {
                            AddError($"Error when execute bulkCopy. {exception.Message}", exception);
                            LogDebug($"Source: {sentence.Source}. Target: {sentence.Target}");
                            LogDebug($"Command: {command.Sql}");
                        }
            }
    }

    /// <summary>
    ///		Ejecuta la sentencia que comprueba si existe un valor en la tabla de datos
    /// </summary>
    private async Task ExecuteIfExistsAsync(SentenceIfExists sentence, CancellationToken cancellationToken)
    {
        ProviderModel? provider = GetProvider(sentence.Source);

            LogInfo("Start if exists");
            if (provider is null)
                AddError($"Can't find provider {sentence.Source}");
            else
            {
                CommandModel command = ConvertProviderCommand(sentence.Command, out string error);

                    // Log
                    LogDebug("ExecuteIfExists", sentence.Command);
                    // Ejecuta el comando
                    if (!string.IsNullOrWhiteSpace(error))
                        AddError($"Error when convert command. {error}");
                    else
                    {
                        bool exists = ExistsData(provider, command, sentence.Command.Timeout);

                            if (!CheckHasError())
                            {
                                if (exists && sentence.SentencesThen.Count > 0)
                                    await ProcessWithContextAsync(sentence.SentencesThen, cancellationToken);
                                else if (!exists && sentence.SentencesElse.Count > 0)
                                    await ProcessWithContextAsync(sentence.SentencesElse, cancellationToken);
                            }
                    }
            }
    }

    /// <summary>
    ///		Comprueba si existen datas para un consulta
    /// </summary>
    private bool ExistsData(ProviderModel provider, CommandModel command, TimeSpan timeout)
    {
        bool exists = false;

            // Comprueba si existen datos
            try
            {
                using (IDataReader reader = provider.OpenReader(command, timeout))
                {
                    if (reader.Read())
                        exists = true;
                }
            }
            catch (Exception exception)
            {
                AddError($"Error when check data exists {command.Sql}", exception);
            }
            // Devuelve el valor que indica si existen datos
            return exists;
    }

    /// <summary>
    ///		Obtiene la tabla de datos
    /// </summary>
    private IEnumerable<DataTable> GetDataTable(ProviderModel provider, CommandModel command)
    {
        int pageIndex = 0;

            // Log
            LogInfo("Read datatable");
            // Carga los datos
            foreach (DataTable table in provider.LoadData(command))
            {
                // Log
                LogDebug($"Reading page {++pageIndex}", command);
                // Devuelve la tabla
                yield return table;
            }
    }

    /// <summary>
    ///		Ejecuta una sentencia de ejecución de comando de datos sobre el proveedor
    /// </summary>
    private void ExecuteDataCommand(SentenceExecute sentence)
    {
        (int _, string error) = ExecuteDataCommand(sentence.Target, sentence.Command);

            if (!string.IsNullOrWhiteSpace(error))
                AddError(error);
    }

    /// <summary>
    ///     Ejecuta la sentencia de apertura de una consulta sobre el proveedor
    /// </summary>
    private async Task ExecuteOpenReaderAsync(SentenceOpenReader sentence, CancellationToken cancellationToken)
    {
        ProviderModel? provider = GetProvider(sentence.Source);

			// Log
			LogInfo($"Open Sql reader on {sentence.Source}");
			// Procesa la apertura del lector
			if (provider is null)
				AddError($"Can't find the provider {sentence.Source}");
			else
				using (IDataReader reader = provider.OpenReader(CreateDataProviderCommand(sentence.Command.Sql, sentence.Command.Timeout), sentence.Command.Timeout))
				{
					VariableModel stream = ProgramInterpreter.AddVariable(sentence.Name, SymbolModel.SymbolType.Object, reader);

						// Ejecuta las sentencias
						await ProgramInterpreter.ProcessAsync(sentence.Sentences, cancellationToken);
						// Elimina la variable
						ProgramInterpreter.RemoveVariable(stream.Name);	
				}
			// Log
			LogInfo($"End Sql reader on {sentence.Source}");
    }

    /// <summary>
    ///     Ejecuta la escritura sobre una tabla
    /// </summary>
    private async Task ExecuteWriteToAsync(SentenceWriteTo sentence, CancellationToken cancellationToken)
    {
        ProviderModel? provider = GetProvider(sentence.Target);

            // Evita las advertencias
            await Task.Delay(1, cancellationToken);
			// Log
			LogInfo($"Start write to {sentence.Target} - {sentence.Table} from {sentence.Source}");
			// Procesa la apertura del lector
			if (provider is null)
				AddError($"Can't find the provider {sentence.Source}");
			else
            {
                VariableModel? variable = ProgramInterpreter.GetVariable(sentence.Source);

                    // Copia los datos
                    if (variable is null)
                        AddError($"Variable {sentence.Source} undefined");
                    else if (variable.Type != SymbolModel.SymbolType.Object || variable.Value is not IDataReader reader)
                        AddError($"Variable {sentence.Source} is not a DataReader");
                    else
                    {
                        long records = provider.BulkCopy(reader, sentence.Table, sentence.Mappings, sentence.BatchSize, sentence.Timeout);

                            LogInfo($"WriteTo: {records:#,##0} copied to {sentence.Table}");
                    }
			}
			// Log
			LogInfo($"End write to {sentence.Target} - {sentence.Target} from {sentence.Source}");
    }

    /// <summary>
    ///		Ejecuta un archivo de script SQL
    /// </summary>
    private void ExecuteScriptSql(SentenceExecuteScript sentence)
    {
        ProviderModel? provider = GetProvider(sentence.Target);

            LogInfo($"Execute script {sentence.FileName}");
            if (provider is null)
                AddError($"Can't find the provider. Key: '{sentence.Target}'");
            else if (string.IsNullOrWhiteSpace(sentence.FileName))
                AddError($"The script filename is not defined");
            else
            {
                string fileName = GetFullFileName(sentence.ProviderFile, sentence.ProviderFileKey, sentence.FileName);

                    // Ejecuta el script
                    if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
                        AddError($"Cant find the file '{fileName}'");
                    else if (!sentence.MustParse)
                        ExecuteScriptSqlRaw(provider, fileName, sentence.Timeout, sentence.SkipParameters);
                    else
                        ExecuteScriptSqlParsed(provider, fileName, sentence);
            }
    }

    /// <summary>
    ///		Ejecuta un script SQL (sin interpretarlo)
    /// </summary>
    private void ExecuteScriptSqlRaw(ProviderModel provider, string fileName, TimeSpan timeout, bool skipParameters)
    {
        string error = string.Empty;

            // Ejecuta el comando completo del archivo SQL
            try
            {
                string sql = LibHelper.Files.HelperFiles.LoadTextFile(fileName);

                    provider.Execute(CreateDataProviderCommand(sql, timeout, skipParameters));
            }
            catch (Exception exception)
            {
                error = $"Error when execute script {Path.GetFileName(fileName)}. {exception.Message}";
            }
            // Añade el error
            if (!string.IsNullOrWhiteSpace(error))
                AddError(error);
    }

    /// <summary>
    ///		Ejecuta un script SQL interpretándolo antes
    /// </summary>
    private void ExecuteScriptSqlParsed(ProviderModel provider, string fileName, SentenceExecuteScript sentence)
    {
        List<SqlSectionModel> sections = new SqlParser().TokenizeByFile(fileName, MapVariables(GetVariables(), sentence.Mapping), out string error);

            // Si no hay ningún error, ejecuta el script
            if (string.IsNullOrWhiteSpace(error))
            {
                // Recorre las seccionaes
                foreach (SqlSectionModel section in sections)
                    if (string.IsNullOrWhiteSpace(error) && section.Type == SqlSectionModel.SectionType.Sql &&
                            !string.IsNullOrWhiteSpace(section.Content))
                        try
                        {
                            provider.Execute(CreateDataProviderCommand(section.Content, sentence.Timeout));
                        }
                        catch (Exception exception)
                        {
                            error = $"Error when execute script {Path.GetFileName(fileName)}. {exception.Message}";
                        }
            }
            // Añade el error
            if (!string.IsNullOrWhiteSpace(error))
                AddError(error);
    }

    /// <summary>
    ///		Mapea las variables
    /// </summary>
    private Dictionary<string, object?> MapVariables(NormalizedDictionary<object?> parameters, List<(string variable, string to)> mappings)
    {
        Dictionary<string, object?> result = new();

            // Mapea las variables al nuevo diccionario
            if (mappings == null || mappings.Count == 0)
                result = parameters.ToDictionary();
            else
                foreach ((string variable, string to) in mappings)
                    if (parameters.TryGetValue(variable, out object? value))
                        result.Add(to, value);
                    else
                        result.Add(to, null);
            // Devuelve el diccionario convertido
            return result;
    }

    /// <summary>
    ///		Convierte el comando del proveedor
    /// </summary>
    internal CommandModel ConvertProviderCommand(SqlSentenceModel sentence, out string error)
    {
        string sql = new SqlParser().ParseCommand(sentence.Sql, GetVariables().ToDictionary(), out error);
        CommandModel? command = new CommandModel(sql, sentence.Timeout);

            // Añade los parámetros al comando si no ha habido ningún error
            if (string.IsNullOrWhiteSpace(error))
                foreach (SqlSentenceParameterModel parameter in sentence.Parameters)
                {
                    object? value = parameter.Default;

                        // Obtiene el valor del parámetro a partir de la variable
                        if (!string.IsNullOrWhiteSpace(parameter.VariableName))
                        {
                            VariableModel? variable = ProgramInterpreter.GetVariable(parameter.VariableName);

                                if (variable is null)
                                    throw new Exceptions.JobDatabaseException($"Can't find the variable {parameter.VariableName}");
                                else
                                    value = variable.Value;
                        }
                        // Añade el parámetro
                        command.Parameters.Add(parameter.Name, value);
                }
            // Devuelve el comando del proveedor
            return command;
    }

    /// <summary>
    ///		Convierte una cadena SQL en un comando del proveedor
    /// </summary>
    internal CommandModel CreateDataProviderCommand(string sql, TimeSpan timeout, bool skipParameters = false)
    {
        CommandModel command = new CommandModel(sql, timeout);

            // Añade las variables del contexto
            if (!skipParameters)
                foreach (KeyValuePair<string, VariableModel> variable in ProgramInterpreter.GetVariablesRecursive())
                    if (!command.Parameters.ContainsKey(variable.Key))
                        command.Parameters.Add(variable.Key, variable.Value.Value);
            // Devuelve el comando del proveedor
            return command;
    }

    /// <summary>
    ///		Ejecuta una sentencia de lote
    /// </summary>
    private void ExecuteDataBatch(SentenceDataBatch sentence)
    {
        switch (sentence.Type)
        {
            case SentenceDataBatch.BatchCommand.BeginTransaction:
                    ExecuteBeginTransaction(sentence.Target);
                break;
            case SentenceDataBatch.BatchCommand.CommitTransaction:
                    ExecuteCommitTransaction(sentence.Target);
                break;
            case SentenceDataBatch.BatchCommand.RollbackTransaction:
                    ExecuteRollbackTransaction(sentence.Target);
                break;
        }
    }

    /// <summary>
    ///		Abre una transacción
    /// </summary>
    private void ExecuteBeginTransaction(string providerKey)
    {
        ProviderModel? provider = GetProvider(providerKey);

            // Log
            LogInfo($"Opening transaction for provider {providerKey}");
            // Abre las transacciones para el proveedor
            if (provider is null)
                AddError($"Can't find the provider. Key: '{providerKey}'");
            else
                Transactions.Add(provider);
    }

    /// <summary>
    ///		Ejecuta el commit de una transacción
    /// </summary>
    private void ExecuteCommitTransaction(string providerKey)
    {
        BatchTransactionModel? transaction = Transactions.Get(providerKey);

            LogInfo($"Commit transaction for provider {providerKey}");
            if (transaction is null)
                AddError($"Can't find an open transaction for provider {providerKey}");
            else
            {
                // Ejecuta los comandos
                try
                {
                    transaction.Provider.Execute(transaction.Commands);
                }
                catch (Exception exception)
                {
                    AddError($"Error when execute commands batch. {exception.Message}");
                }
                // Borra la transacción
                Transactions.Remove(providerKey);
            }
    }

    /// <summary>
    ///		Ejecuta el rollback de una transacción
    /// </summary>
    private void ExecuteRollbackTransaction(string providerKey)
    {
        BatchTransactionModel? transaction = Transactions.Get(providerKey);

            LogInfo($"Rollback transaction for provider {providerKey}");
            if (transaction is null)
                AddError($"Can't find an open transaction for provider {providerKey}");
            else
                Transactions.Remove(providerKey);
    }

    /// <summary>
    ///		Ejecuta un comando sobre el proveedor
    /// </summary>
    private (int result, string error) ExecuteDataCommand(string target, SqlSentenceModel providerCommand)
    {
        ProviderModel? provider = GetProvider(target);
        int result = 0;
        string error;

            // Log
            LogInfo("Start execute data command");
            // Ejecuta el comando
            if (provider is null)
                error = $"Can't find the provider. Key: '{target}'";
            else
            {
                CommandModel command = ConvertProviderCommand(providerCommand, out error);

                    // Log
                    LogDebug("Execute", command);
                    // Ejecuta el comando
                    if (string.IsNullOrEmpty(error))
                    {
                        if (Transactions.Exists(provider))
                        {
                            LogDebug("Add command to batch", command);
                            Transactions.Add(provider, command);
                        }
                        else
                        {
                            try
                            {
                                result = provider.Execute(command);
                            }
                            catch (Exception exception)
                            {
                                // Instrucciones de depuración
                                LogDebug($"Error when execute SQL. Provider: {provider.Key}");
                                if (command is null)
                                    LogDebug("Command is null");
                                else
                                    LogDebug(command.Sql);
                                LogDebug($"Exception: {exception.Message}");
                                // Guarda el error
                                error = $"Error when execute command. {exception.Message}";
                            }
                        }
                    }
            }
            // Devuelve el error
            return (result, error);
    }

    /// <summary>
    ///		Obtiene el proveedor asociado a una clave
    /// </summary>
    internal ProviderModel? GetProvider(string? key)
    {
        // Si la clave es una variable, sustituye por el contenido de la variable
        if (!string.IsNullOrWhiteSpace(key))
            key = ApplyVariablesToKey(key);
        // Devuelve el proveedor
        return DataProviderManager.GetProvider(key);
    }

    /// <summary>
    ///		Aplica las variables a la clave solicitada
    /// </summary>
    private string ApplyVariablesToKey(string value)
    {
        // Si la clave es una variable, sustituye por el contenido de la variable
        if (!string.IsNullOrWhiteSpace(value) && value.StartsWith("{{") && value.EndsWith("}}"))
        {
            VariableModel? variable = ProgramInterpreter.GetVariable(value.Substring(2, value.Length - 4));

                if (variable is not null)
                    value = variable.Value?.ToString() ?? string.Empty;
        }
        // Devuelve la clave
        return value;
    }

    /// <summary>
    ///		Convierte las variables del contexto
    /// </summary>
    private NormalizedDictionary<object?> GetVariables()
    {
        NormalizedDictionary<object?> variables = new();

            // Convierte las variables del contexto actual
            foreach (KeyValuePair<string, VariableModel> variable in ProgramInterpreter.GetVariablesRecursive())
                variables.Add(variable.Key, variable.Value.Value);
            // Devuelve las variables
            return variables;
    }

    /// <summary>
    ///		Añade el mensaje de depuración de una sentencia de comando sobre el proveedor
    /// </summary>
    private void LogDebug(string header, CommandModel command)
    {
        LogDebug($"{header}. Sql {command.Sql}");
        foreach (KeyValuePair<string, object?> parameter in command.Parameters)
            LogDebug($"\t{parameter.Key}: {parameter.Value?.ToString()}");
    }

    /// <summary>
    ///		Añade la depuración de una sentencia
    /// </summary>
    private void LogDebug(string header, SqlSentenceModel command)
    {
        LogDebug($"{header}. Sql {command.Sql}");
        foreach (SqlSentenceParameterModel parameter in command.Parameters)
            LogDebug($"\t{parameter.Name} ({parameter.Type.ToString()}): {parameter.Default?.ToString()}");
    }

    /// <summary>
    ///		Manager de los proveedores de datos
    /// </summary>
    private DbAggregatorManager DataProviderManager { get; } = new();

    /// <summary>
    ///		Transacciones activas en la ejecución del script
    /// </summary>
    private BatchTransactionModelCollection Transactions { get; } = new();
}