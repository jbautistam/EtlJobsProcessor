using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;

namespace Bau.Libraries.LibJobProcessor.Core.Interfaces.Repository;

/// <summary>
///		Repositorio para <see cref="JobContextModel"/>
/// </summary>
public interface IContextRepository
{
    /// <summary>
    ///		Carga los datos de contexto asociado a un nodo
    /// </summary>
    JobContextModel? Load(LibMarkupLanguage.MLNode rootML);
}
