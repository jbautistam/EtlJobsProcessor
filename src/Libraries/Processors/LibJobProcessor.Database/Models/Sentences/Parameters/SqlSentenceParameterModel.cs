using Bau.Libraries.LibInterpreter.Models.Symbols;

namespace Bau.Libraries.LibJobProcessor.Database.Models.Sentences.Parameters;

/// <summary>
///		Clase con los datos de un filtro
/// </summary>
internal class SqlSentenceParameterModel
{
    internal SqlSentenceParameterModel(string name, SymbolModel.SymbolType type, object? defaultValue)
    {
        Name = name;
        Type = type;
        Default = defaultValue;
    }

    /// <summary>
    ///		Clona los datos del filtro
    /// </summary>
    internal SqlSentenceParameterModel Clone()
    {
        return new SqlSentenceParameterModel(Name, Type, Default)
                        {
                            VariableName = VariableName
                        };
    }

    /// <summary>
    ///     Nombre del parámetro
    /// </summary>
    internal string Name { get; }

    /// <summary>
    ///		Nombre de la variable de la que se recoge el valor
    /// </summary>
    internal string? VariableName { get; set; }

    /// <summary>
    ///		Tipo de datos
    /// </summary>
    internal SymbolModel.SymbolType Type { get; }

    /// <summary>
    ///		Valor predeterminado
    /// </summary>
    internal object? Default { get; set; }
}
