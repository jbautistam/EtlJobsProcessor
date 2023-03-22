using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibMarkupLanguage;

namespace Bau.Libraries.LibJobProcessor.Core.Interfaces.Repository;

/// <summary>
///		Repositorio base para los trabajos
/// </summary>
public abstract class BaseJobRepository : IProcessorRepository
{
    protected BaseJobRepository(IProgramRepository programRepository)
    {
        ProgramRepository = programRepository;
    }

    /// <summary>
    ///		Carga una sentencia
    /// </summary>
    public abstract SentenceBase? Load(MLNode rootML);

    /// <summary>
    ///		Carga una colección de sentencias
    /// </summary>
    public List<SentenceBase> LoadSentences(MLNodesCollection nodesML, params string[] skipNodes)
    {
        return ProgramRepository.LoadSentences(nodesML, skipNodes);
    }

    /// <summary>
    ///		Repositorio principal
    /// </summary>
    public IProgramRepository ProgramRepository { get; }
}
