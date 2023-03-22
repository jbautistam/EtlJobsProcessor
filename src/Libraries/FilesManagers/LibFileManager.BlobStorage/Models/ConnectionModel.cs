namespace Bau.Libraries.LibFileManager.BlobStorage.Models;

/// <summary>
///		Datos de una conexión
/// </summary>
internal class ConnectionModel
{
	internal ConnectionModel(string key, string container, string connectionString)
	{
		Key = key;
		Container = container;
		ConnectionString = connectionString;
	}

	/// <summary>
	///		Clave de la conexión
	/// </summary>
	internal string Key { get; }

	/// <summary>
	///		Contenedor
	/// </summary>
	internal string Container { get; }

	/// <summary>
	///		Cadena de conexión
	/// </summary>
	internal string ConnectionString { get; }
}
