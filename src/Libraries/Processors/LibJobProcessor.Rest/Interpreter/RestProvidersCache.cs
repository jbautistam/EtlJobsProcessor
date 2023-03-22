using Bau.Libraries.LibJobProcessor.Rest.Models;
using Bau.Libraries.LibRestClient;
using Bau.Libraries.LibRestClient.Security;

namespace Bau.Libraries.LibJobProcessor.Rest.Interpreter;

/// <summary>
///		Manager con los proveedores de WebApi
/// </summary>
internal class RestProvidersCache
{
	// Registros privados
	private record Connection(RestContextModel context, RestConnection connection);

	internal RestProvidersCache(ScriptInterpreter interpreter)
	{
		Interpreter = interpreter;
	}

	/// <summary>
	///		Obtiene la conexión establecida
	/// </summary>
	internal RestConnection? Get(string key)
	{
		// Busca la conexión en el diccionario, si no la encuentra, la añade
		if (!Connections.TryGetValue(key, out Connection? connection))
		{
			RestContextModel? context = GetRestContext(key);

				// Si se ha definido el contexto, crea y cachea una conexión a REST
				if (context is not null)
				{
					connection = new Connection(context, CreateRestConnection(context));
					Connections.Add(key, connection);
				}
		}
		// Devuelve la conexión
		return connection?.connection;
	}

	/// <summary>
	///		Obtiene el contexto
	/// </summary>
	private RestContextModel? GetRestContext(string target)
	{
		if (Interpreter.GetContext(target) is RestContextModel context)
			return context;
		else
			return null;
	}

	/// <summary>
	///		Crea una conexión a REST con los datos del contexto
	/// </summary>
	private RestConnection CreateRestConnection(RestContextModel context)
	{
		return new RestConnection(new Uri(context.Url), GetSecurity(context));
	}

	/// <summary>
	///		Obtiene la seguridad que se tiene que aplicar a las llamadas Rest
	/// </summary>
	private ISecurity? GetSecurity(RestContextModel context)
	{
		return context.Security.Type switch
				{
					RestContextSecurityModel.SecurityType.Basic => new LibRestClient.Security.Basic.BasicSecurity(context.Security.GetParameter(RestContextSecurityModel.User),
																													context.Security.GetParameter(RestContextSecurityModel.Password)),	
					RestContextSecurityModel.SecurityType.ApiKey => new LibRestClient.Security.ApiKey.ApiKeySecurity(context.Security.GetParameter(RestContextSecurityModel.ApiKey)),	
					RestContextSecurityModel.SecurityType.Jwt => new LibRestClient.Security.Jwt.JwtSecurity
																		(new LibRestClient.Security.Jwt.JwtCredentials(context.Security.GetParameter(RestContextSecurityModel.UrlAuthority),
																														context.Security.GetParameter(RestContextSecurityModel.ClientId),
																														context.Security.GetParameter(RestContextSecurityModel.ClientSecret),
																														context.Security.GetParameter(RestContextSecurityModel.Scopes))
																		),
					_ => null
				};
	}

	/// <summary>
	///		Intérprete
	/// </summary>
	internal ScriptInterpreter Interpreter { get; }

	/// <summary>
	///		Conexiones
	/// </summary>
	private Dictionary<string, Connection> Connections { get; } = new(StringComparer.InvariantCultureIgnoreCase);
}
