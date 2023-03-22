using Bau.Libraries.LibHelper.Extensors;
using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibJobProcessor.Core.Interfaces;
using Bau.Libraries.LibJobProcessor.Core.Models.Jobs;
using Bau.Libraries.LibDataStructures.Collections;

namespace Bau.Libraries.LibJobProcessor.Manager.Repository;

/// <summary>
///		Clase de lectura de <see cref="JobContextModel"/>
/// </summary>
internal class JobContextRepository
{
	// Constantes privadas
	private const string TagRoot = "EtlContext";
	private const string TagParameter = "Parameter";
	private const string TagKey = "Key";
	private const string TagType = "Type";
	private const string TagValue = "Value";
	private const string TagComputeMode = "ComputeDateMode";
	private const string TagIncrement = "Increment";
	private const string TagInterval = "Interval";
	private const string TagMode = "Mode";

	/// <summary>
	///		Carga los datos de una serie de <see cref="JobContextModel"/> sobre el proyecto
	/// </summary>
	internal void Load(JobProjectManager manager, Core.Models.JobProjectModel project, string fileName)
	{
		MLFile fileML = new LibMarkupLanguage.Services.XML.XMLParser().Load(fileName);

			if (fileML is not null)
				foreach (MLNode rootML in fileML.Nodes)
					if (rootML.Name == TagRoot)
						foreach (MLNode nodeML in rootML.Nodes)
							switch (nodeML.Name)
							{
								case TagParameter:
										project.ProjectExecutionContext.Parameters.Add(LoadParameter(nodeML));	
									break;
								default:
										LoadContext(manager, project, nodeML);
									break;
							}
	}

	/// <summary>
	///		Carga el contexto de un procesador
	/// </summary>
	private void LoadContext(JobProjectManager manager, Core.Models.JobProjectModel project, MLNode rootML)
	{
		// Agrega los contextos de los procesadores
		foreach (IJobProcessor processor in manager.Processors)
		{
			JobContextModel? context = processor.GetContextRepository().Load(rootML);

				if (context is not null)
					project.ProjectExecutionContext.Contexts.Add(context);
		}
		// Lee los contextos de los administradores de archivos
		foreach (Core.Interfaces.FilesManager.IFileManager fileManager in manager.FilesManagers)
			fileManager.LoadContext(rootML);
	}

	/// <summary>
	///		Carga un parámetro
	/// </summary>
	private JobParameterModel LoadParameter(MLNode rootML)
	{
		JobParameterModel parameter = new(rootML.Attributes[TagKey].Value.TrimIgnoreNull(), 
										  rootML.Attributes[TagType].Value.GetEnum(JobParameterModel.ParameterType.String));

			// Obtiene el valor
			switch (parameter.Type)
			{
				case JobParameterModel.ParameterType.Numeric:
						parameter.Value = rootML.Attributes[TagValue].Value.GetDouble(0);
					break;
				case JobParameterModel.ParameterType.Boolean:
						parameter.Value = rootML.Attributes[TagValue].Value.GetBool();
					break;
				case JobParameterModel.ParameterType.DateTime:
						ConvertDate(rootML, parameter);
					break;
				default:
						if (string.IsNullOrWhiteSpace(rootML.Attributes[TagValue].Value))
							parameter.Value = rootML.Value.TrimIgnoreNull();
						else
							parameter.Value = rootML.Attributes[TagValue].Value.TrimIgnoreNull();
					break;
			}
			// Devuelve el parámetro
			return parameter;
	}

	/// <summary>
	///		Convierte una fecha
	/// </summary>
	private void ConvertDate(MLNode nodeML, JobParameterModel parameter)
	{
		if (!string.IsNullOrWhiteSpace(nodeML.Attributes[TagValue].Value))
			parameter.Value = nodeML.Attributes[TagValue].Value.GetDateTime();
		else
		{
			DateTime date = DateTime.Now.Date;

				// Asigna los valores iniciales
				parameter.DateMode = nodeML.Attributes[TagComputeMode].Value.GetEnum(JobParameterModel.ComputeDateMode.Today);
				parameter.Interval = nodeML.Attributes[TagInterval].Value.GetEnum(JobParameterModel.IntervalType.Day);
				parameter.Increment = nodeML.Attributes[TagIncrement].Value.GetInt(0);
				parameter.Mode = nodeML.Attributes[TagMode].Value.GetEnum(JobParameterModel.IntervalMode.Unknown);
				// Se recoge la fecha (si se ha introducido alguna)
				if (parameter.DateMode == JobParameterModel.ComputeDateMode.Fixed)
					date = nodeML.Attributes[TagValue].Value.GetDateTime(DateTime.Now);
				// Ajusta el valor con los parámetros del XML
				if (parameter.Increment != 0)
					switch (parameter.Interval)
					{
						case JobParameterModel.IntervalType.Day:
								date = date.AddDays(parameter.Increment);
							break;
						case JobParameterModel.IntervalType.Month:
								date = date.AddMonths(parameter.Increment);
							break;
						case JobParameterModel.IntervalType.Year:
								date = date.AddYears(parameter.Increment);
							break;
					}
				// Ajusta la fecha
				switch (parameter.Mode)
				{
					case JobParameterModel.IntervalMode.PreviousMonday:
							date = GetPreviousMonday(date);
						break;
					case JobParameterModel.IntervalMode.NextMonday:
							date = GetNextMonday(date);
						break;
					case JobParameterModel.IntervalMode.MonthStart:
							date = GetFirstMonthDay(date);
						break;
					case JobParameterModel.IntervalMode.MonthEnd:
							date = GetLastMonthDay(date);
						break;
				}
				// Asigna la fecha calculada
				parameter.Value = date;
		}
	}

	/// <summary>
	///		Obtiene el lunes anterior a una fecha (o la misma fecha si ya es lunes)
	/// </summary>
	private DateTime GetPreviousMonday(DateTime date)
	{
		// Busca el lunes anterior
		while (date.DayOfWeek != DayOfWeek.Monday)
			date = date.AddDays(-1);
		// Devuelve la fecha
		return date;
	}

	/// <summary>
	///		Obtiene el lunes siguiente a una fecha (o la misma fecha si ya es lunes)
	/// </summary>
	private DateTime GetNextMonday(DateTime date)
	{
		// Busca el lunes anterior
		while (date.DayOfWeek != DayOfWeek.Monday)
			date = date.AddDays(1);
		// Devuelve la fecha
		return date;
	}

	/// <summary>
	///		Obtiene el primer día del mes
	/// </summary>
	private DateTime GetFirstMonthDay(DateTime date)
	{
		return new DateTime(date.Year, date.Month, 1);
	}

	/// <summary>
	///		Obtiene el último día del mes
	/// </summary>
	private DateTime GetLastMonthDay(DateTime date)
	{
		return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
	}
}