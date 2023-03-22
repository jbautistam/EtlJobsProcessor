namespace Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Parameters;

/// <summary>
///		Sentencia que se envía al proveedor
/// </summary>
internal class SqlSentenceModel
{
    internal SqlSentenceModel(string sql, TimeSpan timeout)
    {
        Sql = sql;
        Timeout = timeout;
    }

    /// <summary>
    ///		Clona la sentencia
    /// </summary>
    internal SqlSentenceModel Clone()
    {
        SqlSentenceModel target = new SqlSentenceModel(Sql, Timeout);

            // Añade los filtros
            foreach (SqlSentenceParameterModel parameter in Parameters)
                target.Parameters.Add(parameter.Clone());
            // Devuelve el comando clonado
            return target;
    }

    /// <summary>
    ///		Comandos
    /// </summary>
    internal string Sql { get; }

    /// <summary>
    ///		Tiempo de espera del comando
    /// </summary>
    internal TimeSpan Timeout { get; }

    /// <summary>
    ///		Parámetros de ejecución
    /// </summary>
    internal List<SqlSentenceParameterModel> Parameters { get; } = new();
}
