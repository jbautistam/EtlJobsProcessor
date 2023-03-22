using Bau.Libraries.LibInterpreter.Models.Sentences;

namespace Bau.Libraries.LibJobProcessor.Database.Models.Sentences;

/// <summary>
///		Sentencia de inicio de lote
/// </summary>
internal class SentenceDataBatch : SentenceBase
{
    /// <summary>
    ///		Tipo de comando
    /// </summary>
    internal enum BatchCommand
    {
        /// <summary>Arranca una transacción</summary>
        BeginTransaction,
        /// <summary>Confirma una transacción</summary>
        CommitTransaction,
        /// <summary>Deshace la transacción</summary>
        RollbackTransaction
    }

    internal SentenceDataBatch(string target, BatchCommand type)
    {
        Target = target;
        Type = type;
    }

    /// <summary>
    ///		Proveedor sobre el que se ejecuta la transacción
    /// </summary>
    internal string Target { get; }

    /// <summary>
    ///		Indica el tipo de sentencia
    /// </summary>
    internal BatchCommand Type { get; }
}
