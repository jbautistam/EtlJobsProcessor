using FluentAssertions;
using Bau.Libraries.LibJobProcessor.Manager;

namespace EtlJobs.Test;

public class Rest_should : IClassFixture<Infrastructure.TestsFixture>
{
    Infrastructure.TestsFixture _fixture;

    public Rest_should(Infrastructure.TestsFixture fixture)
    {
        _fixture = fixture;
    }

	[Fact]
	public async Task execute()
	{
		JobProjectManager manager = _fixture.GetJobManager("Samples/Rest/Project.xml", "Samples/Rest/Context.xml");

			// Procesa el proyecto
			await manager.ProcessAsync(CancellationToken.None);
			// Comprueba si ha habido errores
			if (manager.HasError)
				_fixture.ShowErrors(manager.Errors);
			manager.HasError.Should().BeFalse();
	}
}