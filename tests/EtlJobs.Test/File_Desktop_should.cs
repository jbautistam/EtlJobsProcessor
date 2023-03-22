using FluentAssertions;
using Bau.Libraries.LibJobProcessor.Manager;

using Bau.Libraries.LibJobProcessor.Core.Models;

namespace EtlJobs.Test;

/// <summary>
///		Pruebas de proyectos de archivos en local
/// </summary>
public class File_Desktop_should : IClassFixture<Infrastructure.TestsFixture>
{
	// Variables privadas
    private Infrastructure.TestsFixture _fixture;

    public File_Desktop_should(Infrastructure.TestsFixture fixture)
    {
        _fixture = fixture;
    }

	[Fact]
	public async Task execute()
	{
		JobProjectManager manager = _fixture.GetJobManager("Samples/Files/Project.xml", "Samples/Files/Context.xml");
		JobProjectModel project = manager.Load();

			// Procesa el proyecto
			await manager.ProcessAsync(project, CancellationToken.None);
			// Comprueba si ha habido errores
			if (manager.HasError)
				_fixture.ShowErrors(manager.Errors);
			manager.HasError.Should().BeFalse();
	}
}