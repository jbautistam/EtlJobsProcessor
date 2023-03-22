using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;

namespace Bau.Libraries.LibFileManager.BlobStorage.Repository;

/// <summary>
///		Clase para cargar datos de contexto para los programas de tratamiento de Cloud
/// </summary>
internal class BlobStorageContextRepository
{
	// Constantes privadas
	private const string TagRoot = "Cloud";
	private const string TagKey = "Key";
	private const string TagContainer = "Container";
	private const string TagConnectionString = "ConnectionString";

	/// <summary>
	///		Carga el contexto de un nodo
	/// </summary>
	public Models.ConnectionModel? Load(MLNode rootML)
	{
		if (rootML.Name == TagRoot)
			return new Models.ConnectionModel(rootML.Attributes[TagKey].Value.TrimIgnoreNull(),
											  rootML.Attributes[TagContainer].Value.TrimIgnoreNull(),
											  GetConnectionString(rootML));
		else
			return null;
	}

	/// <summary>
	///		Obtiene la conexión asociada a un nodo
	/// </summary>
	private string GetConnectionString(MLNode rootML)
	{
		string result = rootML.Attributes[TagConnectionString].Value.TrimIgnoreNull();

			// Si la cadena de conexión no está en el nodo, recoge el valor
			if (string.IsNullOrWhiteSpace(result))
				result = rootML.Value.TrimIgnoreNull();
			// Devuelve el resultado
			return result;
	}
}