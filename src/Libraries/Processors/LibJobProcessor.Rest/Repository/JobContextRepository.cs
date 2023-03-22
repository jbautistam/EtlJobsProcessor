using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Repository;
using Bau.Libraries.LibJobProcessor.Rest.Models;
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
	private const string TagSecurityBasic = "SecurityBasic";
	private const string TagSecurityApiKey = "SecurityApiKey";
	private const string TagSecurityJwt = "SecurityJwt";
	private const string TagUser = "User";
	private const string TagPassword = "Password";
	private const string TagApiKey = "ApiKey";
	private const string TagUrlAuthority = "UrlAuthority";
	private const string TagClientId = "ClientId";
	private const string TagClientSecret = "ClientSecret";
	private const string TagScopes = "Scopes";

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
	private RestContextModel LoadRestContext(MLNode rootML)
	{
		RestContextModel context = new(rootML.Attributes[TagKey].Value.TrimIgnoreNull(),
									   rootML.Attributes[TagUrlBase].Value.TrimIgnoreNull());

			// Carga la seguridad
			foreach (MLNode nodeML in rootML.Nodes)
				switch (nodeML.Name)
				{
					case TagSecurityBasic:
							LoadSecurityBasic(context.Security, nodeML);
						break;
					case TagSecurityApiKey:
							LoadSecurityApiKey(context.Security, nodeML);
						break;
					case TagSecurityJwt:
							LoadSecurityJwt(context.Security, nodeML);
						break;
				}
			// Devuleve el contexto
			return context;
	}

	/// <summary>
	///		Carga la seguridad básica
	/// </summary>
	private void LoadSecurityBasic(RestContextSecurityModel security, MLNode nodeML)
	{
		security.Type = RestContextSecurityModel.SecurityType.Basic;
		security.Parameters.Add(RestContextSecurityModel.User, nodeML.Attributes[TagUser].Value.TrimIgnoreNull());
		security.Parameters.Add(RestContextSecurityModel.Password, nodeML.Attributes[TagPassword].Value.TrimIgnoreNull());
	}

	/// <summary>
	///		Carga los datos de seguridad con ApiKey
	/// </summary>
	private void LoadSecurityApiKey(RestContextSecurityModel security, MLNode nodeML)
	{
		security.Type = RestContextSecurityModel.SecurityType.ApiKey;
		security.Parameters.Add(RestContextSecurityModel.ApiKey, nodeML.Attributes[TagApiKey].Value.TrimIgnoreNull());
	}

	/// <summary>
	///		Carga la seguridad JWT
	/// </summary>
	private void LoadSecurityJwt(RestContextSecurityModel security, MLNode nodeML)
	{
		security.Type = RestContextSecurityModel.SecurityType.Jwt;
		security.Parameters.Add(RestContextSecurityModel.UrlAuthority, nodeML.Attributes[TagUrlAuthority].Value.TrimIgnoreNull());
		security.Parameters.Add(RestContextSecurityModel.ClientId, nodeML.Attributes[TagClientId].Value.TrimIgnoreNull());
		security.Parameters.Add(RestContextSecurityModel.ClientSecret, nodeML.Attributes[TagClientSecret].Value.TrimIgnoreNull());
		security.Parameters.Add(RestContextSecurityModel.Scopes, nodeML.Attributes[TagScopes].Value.TrimIgnoreNull());
	}
}
