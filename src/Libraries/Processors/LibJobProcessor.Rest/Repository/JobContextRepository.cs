using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Repository;
using Bau.Libraries.LibMarkupLanguage;

namespace Bau.Libraries.LibJobProcessor.Rest.Repository;

/// <summary>
///		Clase para cargar datos de contexto
/// </summary>
internal class JobContextRepository : IContextRepository
{
	// Constantes privadas
	private const string TagRoot = "RestContext";
	private const string TagKey = "Key";
	private const string TagUrlBase = "Url";
	private const string TagUser = "User";
	private const string TagPassword = "Password";

	/// <summary>
	///		Carga la sentencia de un nodo
	/// </summary>
	public Core.Models.Jobs.JobContextModel? Load(MLNode rootML)
	{
		if (rootML.Name == TagRoot)
			return LoadRestContext(rootML);
		else
			return null;
	}

	/// <summary>
	///		Carga los datos de contexto de un servidor
	/// </summary>
	private Models.RestContextModel LoadRestContext(MLNode rootML)
	{
		return new Models.RestContextModel(rootML.Attributes[TagKey].Value.TrimIgnoreNull(),
										   rootML.Attributes[TagUrlBase].Value.TrimIgnoreNull());
	}
}
