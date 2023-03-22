using Bau.Libraries.LibJobProcessor.Manager;
using Bau.Libraries.LibJobProcessor.Manager.Models;
using Microsoft.Extensions.Logging;

namespace EtlJobs.Test.Infrastructure;

/// <summary>
///     Configuración para pruebas
/// </summary>
public class TestsFixture: IDisposable
{
    public TestsFixture()
    {
    }

    /// <summary>
    ///     Obtiene el nombre de un archivo de pruebas
    /// </summary>
    public string GetTestFileName(string relativeFileName)
    {
        return Path.Combine(GetExecutionPath(), relativeFileName);
    }

    /// <summary>
    ///     Obtiene el directorio de ejecución
    /// </summary>
    public string GetExecutionPath()
    {
        return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? string.Empty;
    }

	/// <summary>
	///		Obtiene el manager de procesos
	/// </summary>
	public JobProjectManager GetJobManager(string fileProject, string fileContext)
	{
		ConfigurationModel configuration = new()
												{
													ProjectFile = GetTestFileName(fileProject),
													ContextFile = GetTestFileName(fileContext)
												};
			// Devuelve el manager de procesos
			return GetJobManager(configuration);
	}

	/// <summary>
	///		Obtiene el manager de procesos
	/// </summary>
	public JobProjectManager GetJobManager(ConfigurationModel configuration)
	{
		JobProjectManager manager = new JobProjectManager(CreateLogger(), configuration);

			// Asigna los procesadores
			manager.AddProcessor(new Bau.Libraries.LibJobProcessor.Cloud.JobCloudManager());
			manager.AddProcessor(new Bau.Libraries.LibJobProcessor.Database.JobDatabaseManager());
			manager.AddProcessor(new Bau.Libraries.LibJobProcessor.FilesShell.JobFileShellManager());
			manager.AddProcessor(new Bau.Libraries.LibJobProcessor.FilesStructured.JobFileStructuredManager());
			manager.AddProcessor(new Bau.Libraries.LibJobProcessor.Rest.JobRestManager());
			// Asigna los manager de archivos
			manager.AddFileManager(new Bau.Libraries.LibFileManager.Desktop.DesktopFileManager());
			manager.AddFileManager(new Bau.Libraries.LibFileManager.BlobStorage.BlobStorageFileManager());
			// Devuelve el manager de procesos
			return manager;
	}

	/// <summary>
	///		Muestra los errores
	/// </summary>
	internal void ShowErrors(List<string> errors)
	{
		foreach (string error in errors)
			System.Diagnostics.Debug.WriteLine($"Script error: {error}");
	}

	/// <summary>
	///		Crea el logger
	/// </summary>
	private ILogger CreateLogger()
	{
		return LoggerFactory.Create(builder => {
													builder.AddFilter("Microsoft", LogLevel.Warning)
														.AddFilter("System", LogLevel.Warning)
														.AddFilter("NonHostConsoleApp.Program", LogLevel.Debug);
												}
									).CreateLogger("EtlJobsProcessor");
	}

    /// <summary>
    ///     Libera la memoria
    /// </summary>
    public void Dispose()
    {
        // ... clean up test data from the database ...
    }
}
