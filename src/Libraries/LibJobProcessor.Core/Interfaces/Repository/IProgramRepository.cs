using Bau.Libraries.LibInterpreter.Models.Symbols;

namespace Bau.Libraries.LibJobProcessor.Core.Interfaces.Repository;

/// <summary>
///		Repositorio para carga de programas
/// </summary>
public interface IProgramRepository
{
    /// <summary>
    ///		Carga una colección de sentencias
    /// </summary>
    List<LibInterpreter.Models.Sentences.SentenceBase> LoadSentences(LibMarkupLanguage.MLNodesCollection nodesML, params string[] skipNodes);

    /// <summary>
    ///		Obtiene el timeout definido en el nodo
    /// </summary>
    TimeSpan GetTimeout(LibMarkupLanguage.MLNode rootML, TimeSpan defaultTimeOut);

    /// <summary>
    ///		Convierte una cadena con un valor en un objeto dependiente del tipo
    /// </summary>
    object? ConvertStringValue(SymbolModel.SymbolType type, string value);

    /// <summary>
    ///		Directorio base del archivo que se está cargando
    /// </summary>
    string ProjectPath { get; }
}
