namespace Bau.Libraries.LibJobProcessor.Rest.Models;

public class RestContextModel : Core.Models.Jobs.JobContextModel
{
	public RestContextModel(string key, string url) : base(key)
	{
		Url = url;
	}

	/// <summary>
	///		Url de conexión
	/// </summary>
	public string Url { get; }
}
