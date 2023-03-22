using Bau.Libraries.DbAggregator.Models;

namespace Bau.Libraries.LibJobProcessor.Database.Interpreter.Context;

/// <summary>
///		Clase con los datos de un lote pendiente de ejecución
/// </summary>
internal class BatchTransactionModel
{
    public BatchTransactionModel(ProviderModel provider)
    {
        Provider = provider;
    }

    /// <summary>
    ///		Proveedor sobre el que se ejecuta la transacción
    /// </summary>
    internal ProviderModel Provider { get; }

    /// <summary>
    ///		Comandos asociados a la transacción
    /// </summary>
    internal List<CommandModel> Commands { get; } = new();
}
