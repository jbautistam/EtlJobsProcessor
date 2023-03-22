namespace Bau.Libraries.LibJobProcessor.Database.Models;

/// <summary>
///		Conexión a base de datos
/// </summary>
public class StorageConnectionModel : Core.Models.Jobs.JobContextModel
{
	public StorageConnectionModel(string key, string connectionString) : base(key)
	{
		ConnectionString = connectionString;
	}

	/// <summary>
	///		Cadena de conexión
	/// </summary>
	public string ConnectionString { get; }
}
