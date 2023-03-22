using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibJobProcessor.Database.Models;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Repository;
using Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Connections;

namespace Bau.Libraries.LibJobProcessor.Database.Repository;

/// <summary>
///		Clase de lectura de contexto
/// </summary>
internal class DbContextRepository : IContextRepository
{
    // Constantes privadas
    private const string TagDataBaseRoot = "Database";
    private const string TagStorageRoot = "Storage";
    private const string TagKey = "Key";
    private const string TagType = "Type";
    private const string TagName = "Name";
    private const string TagDescription = "Description";
    private const string TagConnectionString = "ConnectionString";

    /// <summary>
    ///		Carga una sentencia del nodo
    /// </summary>
    public Core.Models.Jobs.JobContextModel? Load(MLNode rootML)
    {
        return rootML.Name switch
                    {
                        TagDataBaseRoot => LoadDataBaseConnection(rootML),
                        TagStorageRoot => LoadStorageConnection(rootML),
                        _ => null
                    };
    }

    /// <summary>
    ///		Obtiene la conexión a base de datos de un nodo
    /// </summary>
    private DataBaseConnectionModel LoadDataBaseConnection(MLNode rootML)
    {
        DataBaseConnectionModel connection = new DataBaseConnectionModel(rootML.Attributes[TagKey].Value.TrimIgnoreNull(),
                                                                         rootML.Attributes[TagType].Value.TrimIgnoreNull());

            // Asigna el resto de propiedades
            foreach (MLNode nodeML in rootML.Nodes)
                switch (nodeML.Name)
                {
                    case TagName:
                            connection.Name = nodeML.Value.TrimIgnoreNull();
                        break;
                    case TagDescription:
                            connection.Description = nodeML.Value.TrimIgnoreNull();
                        break;
                    default:
                            connection.Parameters.Add(nodeML.Name, nodeML.Value.TrimIgnoreNull());
                        break;
                }
            // Devuelve la conexión
            return connection;
    }

    /// <summary>
    ///		Obtiene la conexión a base de datos de un nodo
    /// </summary>
    private StorageConnectionModel LoadStorageConnection(MLNode rootML)
    {
        return new StorageConnectionModel(rootML.Attributes[TagKey].Value.TrimIgnoreNull(), GetConnectionString(rootML));
    }

    /// <summary>
    ///		Obtiene la cadena de conexión del atributo de un nodo o su cuerpo
    /// </summary>
    private string GetConnectionString(MLNode rootML)
    {
        string result = rootML.Attributes[TagConnectionString].Value.TrimIgnoreNull();

            // Si no se ha recogido ningún valor, recoge el valor del cuerpo
            if (string.IsNullOrWhiteSpace(result))
                result = rootML.Value.TrimIgnoreNull();
            // Devuelve la cadena de conexión
            return result;
    }
}
