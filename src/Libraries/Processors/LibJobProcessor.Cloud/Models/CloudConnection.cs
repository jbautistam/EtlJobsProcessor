using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;

namespace Bau.Libraries.LibJobProcessor.Cloud.Models;

/// <summary>
///		Parámetros de acceso a la nube
/// </summary>
internal class CloudConnection : JobContextModel
{
	/// <summary>
	///		Tipo de servicios de la nube
	/// </summary>
	internal enum CloudType
	{
		/// <summary>Desconocido. No se debería utilizar</summary>
		Unknown,
		/// <summary>Servicios de Azure</summary>
		Azure
	}

	internal CloudConnection(string key, CloudType type, string storageConnectionString) : base(key)
	{
		Key = key;
		Type = type;
		StorageConnectionString = storageConnectionString;
	}

	/// <summary>
	///		Clave de la configuración
	/// </summary>
	internal string Key { get; }

	/// <summary>
	///		Tipo de servicio
	/// </summary>
	internal CloudType Type { get; }

	/// <summary>
	///		Cadena de conexión
	/// </summary>
	internal string StorageConnectionString { get; }
}
