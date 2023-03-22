using Bau.Libraries.LibDataStructures.Collections;

namespace Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Connections;

/// <summary>
///		Conexión a base de datos
/// </summary>
public class DataBaseConnectionModel : Core.Models.Jobs.JobContextModel
{
    public DataBaseConnectionModel(string key, string type) : base(key)
    {
        Type = type;
    }

    /// <summary>
    ///		Nombre de la conexión
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    ///		Descripción de la conexión
    /// </summary>
    public string Description { get; set; } = default!;

    /// <summary>
    ///		Tipo de base de datos
    /// </summary>
    public string Type { get; }

    /// <summary>
    ///		Parámetros de conexión a la base de datos
    /// </summary>
    public NormalizedDictionary<string> Parameters { get; } = new();
}
