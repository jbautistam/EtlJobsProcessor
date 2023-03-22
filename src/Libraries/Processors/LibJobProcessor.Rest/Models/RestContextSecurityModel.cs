namespace Bau.Libraries.LibJobProcessor.Rest.Models;

/// <summary>
///		Datos de seguridad para el contexto
/// </summary>
public class RestContextSecurityModel
{
	// Constantes internas
	public const string User = nameof(User);
	public const string Password = nameof(Password);
	public const string ApiKey = nameof(ApiKey);
	public const string UrlAuthority = nameof(UrlAuthority);
	public const string ClientId = nameof(ClientId);
	public const string ClientSecret = nameof(ClientSecret);
	public const string Scopes = nameof(Scopes);
	/// <summary>
	///		Tipo de seguridad
	/// </summary>
	public enum SecurityType
	{
		/// <summary>Ninguna</summary>
		None,
		/// <summary>Seguridad básica</summary>
		Basic,
		/// <summary>Clave api</summary>
		ApiKey,
		/// <summary>Jwt</summary>
		Jwt
	}

	/// <summary>
	///		Obtiene el valor de un parámetro
	/// </summary>
	public string GetParameter(string key)
	{
		if (Parameters.TryGetValue(key, out string? value))
			return value;
		else
			return string.Empty;
	}

	/// <summary>
	///		Tipo
	/// </summary>
	public SecurityType Type { get; set; } = SecurityType.None;

	/// <summary>
	///		Parámetros (usuario / contraseña, claves de api, authority...)
	/// </summary>
	public Dictionary<string, string> Parameters { get; } = new(StringComparer.InvariantCultureIgnoreCase);
}
