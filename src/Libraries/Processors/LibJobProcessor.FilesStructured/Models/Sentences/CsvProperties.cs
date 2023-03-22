namespace Bau.Libraries.LibJobProcessor.FilesStructured.Models.Sentences;

/// <summary>
///		Propiedades de un archivo CSV
/// </summary>
internal class CsvProperties
{
	internal CsvProperties(bool withHeader, string separator, string dateFormat, string decimalSeparator, string thousandsSeparator,
						   string trueValue, string falseValue)
	{
		WithHeader = withHeader;
		Separator = AssignWithDefault(separator, ",");
		DateFormat = AssignWithDefault(dateFormat, "yyyy-MM-dd");
		DecimalSeparator = AssignWithDefault(decimalSeparator, ".");
		ThousandsSeparator = AssignWithDefault(thousandsSeparator, "");
		TrueValue = AssignWithDefault(trueValue, "1");
		FalseValue = AssignWithDefault(falseValue, "0");
	}

	/// <summary>
	///		Asigna un valor o el predeterminado
	/// </summary>
	private string AssignWithDefault(string value, string defaultValue)
	{
		if (string.IsNullOrWhiteSpace(value))
			return defaultValue;
		else
			return value;
	}

	/// <summary>
	///		Indica si la primera línea es la cabecera
	/// </summary>
	internal bool WithHeader { get; }

	/// <summary>
	///		Separador de campos
	/// </summary>
	internal string Separator { get; }

	/// <summary>
	///		Formato de fecha
	/// </summary>
	internal string DateFormat { get; }

	/// <summary>
	///		Separador de decimales
	/// </summary>
	internal string DecimalSeparator { get; }

	/// <summary>
	///		Separador de miles
	/// </summary>
	internal string ThousandsSeparator { get; }

	/// <summary>
	///		Cadena para los valores verdaderos
	/// </summary>
	internal string TrueValue { get; }

	/// <summary>
	///		Cadena para los valores falsos
	/// </summary>
	internal string FalseValue { get; }
}
