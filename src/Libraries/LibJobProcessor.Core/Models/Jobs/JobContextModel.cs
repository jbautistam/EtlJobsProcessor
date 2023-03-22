namespace Bau.Libraries.LibJobProcessor.Core.Models.Jobs;

/// <summary>
///		Clase con el contexto de un procesador
/// </summary>
public class JobContextModel
{
    public JobContextModel(string key)
    {
        Key = key;
    }

    /// <summary>
    ///		Depuración del contexto
    /// </summary>
    public void Debug(System.Text.StringBuilder builder, string indent)
    {
        builder.AppendLine($"{indent}: {Key}");
    }

    /// <summary>
    ///     Clave del contexto
    /// </summary>
    public string Key { get; }
}
