namespace Bau.Libraries.LibJobProcessor.Core.Interfaces.Repository;

/// <summary>
///		Interface para los repositorios de los procesadores
/// </summary>
public interface IProcessorRepository
{
    /// <summary>
    ///		Carga los datos de una sentencia de un nodo
    /// </summary>
    LibInterpreter.Models.Sentences.SentenceBase? Load(LibMarkupLanguage.MLNode rootML);

    /// <summary>
    ///		Repositorio padre
    /// </summary>
    IProgramRepository ProgramRepository { get; }
}
