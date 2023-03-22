using FluentAssertions;
using Bau.Libraries.LibJobProcessor.Manager;

using Bau.Libraries.LibJobProcessor.Core.Models;

namespace EtlJobs.Test;

/// <summary>
///		Pruebas de proyectos de base de datos
/// </summary>
public class Database_should : IClassFixture<Infrastructure.TestsFixture>
{
    Infrastructure.TestsFixture _fixture;

    public Database_should(Infrastructure.TestsFixture fixture)
    {
        _fixture = fixture;
    }

	[Fact]
	public async Task execute()
	{
		JobProjectManager manager = _fixture.GetJobManager("Samples/DataBase/Project.xml", "Samples/DataBase/Context.xml");
		JobProjectModel project = manager.Load();

			// Procesa el proyecto
			await manager.ProcessAsync(project, CancellationToken.None);
			// Comprueba si ha habido errores
			if (manager.HasError)
				_fixture.ShowErrors(manager.Errors);
			manager.HasError.Should().BeFalse();
	}
}