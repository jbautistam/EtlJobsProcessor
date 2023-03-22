using Microsoft.Extensions.Logging;

using Bau.Libraries.LibJobProcessor.Manager;
using Bau.Libraries.LibJobProcessor.Manager.Models;

namespace EtlJobsProcessor;

/// <summary>
///		Clase de inicio de la aplicación
/// </summary>
internal static class Program
{
	/// <summary>
	///		Rutina principal de la aplicación
	/// </summary>
	internal async static Task Main(string[] args)
	{
		ILogger logger = CreateLogger();
		ConfigurationModel configuration = new();

			// Interpreta la configuración
			configuration.Parse(args);
			// Valida la configuración antes de ejecutar el script
			if (!configuration.Validate(out string error))
			{
				Console.WriteLine("Can't read the console arguments");
				Console.WriteLine("Use: EtlJobsProcessor.exe --project ScriptFileName --context ContextFileName [--debug]");
				Console.WriteLine("Where ScriptFileName is the file name that contains the script and ContextFileName is the file that contains context information");
				Console.WriteLine("Error when load configuration");
				Console.WriteLine(error);
			}
			else
			{
				CancellationTokenSource cancellationTokenSource = new();

					// Log
					logger.LogInformation($"Start execute {configuration.ProjectFile}");
					// Ejecuta el script
					try
					{
						JobProjectManager manager = GetJobManager(logger, configuration);

							if (configuration.IsDebug)
								Console.WriteLine(manager.GetDebug());
							else 
							{
								// Procesa la aplicación
								await manager.ProcessAsync(cancellationTokenSource.Token);
								// Log
								logger.LogInformation($"End execution '{configuration.ProjectFile}'");
								// Log de errores
								if (manager.Errors.Count > 0)
								{
									logger.LogError("Execution errors");
									foreach (string executionError in manager.Errors)
										logger.LogError(executionError);
								}
							}
					}
					catch (Exception exception)
					{
						logger.LogError(exception, $"Error when execute {configuration.ProjectFile}");
					}
			}
			// Espera a que el usuario pulse una tecla
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine();
			Console.WriteLine("Press any key...");
			Console.ReadKey();
	}

	/// <summary>
	///		Crea el logger
	/// </summary>
	private static ILogger CreateLogger()
	{
		return LoggerFactory.Create(builder => {
												 builder
														.AddFilter("Microsoft", LogLevel.Warning)
														.AddFilter("System", LogLevel.Warning)
														.AddFilter("NonHostConsoleApp.Program", LogLevel.Debug)
														.SetMinimumLevel(LogLevel.Debug)
														.AddConsole();
												}
									).CreateLogger("EtlJobsProcessor");
	}

	/// <summary>
	///		Obtiene el manager de procesos
	/// </summary>
	private static JobProjectManager GetJobManager(ILogger logger, ConfigurationModel configuration)
	{
		JobProjectManager manager = new JobProjectManager(logger, configuration);

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
	///		Directorio base de la aplicación
	/// </summary>
	internal static string PathBaseApplication
	{
		get { return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? string.Empty; }
	}
}
