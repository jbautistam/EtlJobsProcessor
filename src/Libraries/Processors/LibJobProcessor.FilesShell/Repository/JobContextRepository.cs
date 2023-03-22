﻿using Bau.Libraries.LibMarkupLanguage;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Repository;

namespace Bau.Libraries.LibJobProcessor.FilesShell.Repository;

/// <summary>
///		Clase para cargar datos de proceso sobre archivos
/// </summary>
public class JobContextRepository : IContextRepository
{
	/// <summary>
	///		Carga el contexto de un nodo
	/// </summary>
	public Core.Models.Jobs.JobContextModel? Load(MLNode rootML)
	{
		return null;
	}
}