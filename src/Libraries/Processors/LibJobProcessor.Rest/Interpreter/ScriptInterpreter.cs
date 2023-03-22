using Bau.Libraries.LibJobProcessor.Rest.Models.Sentences;
using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Interpreter;
using Bau.Libraries.LibRestClient;

namespace Bau.Libraries.LibJobProcessor.Rest.Interpreter;

/// <summary>
///		Intérprete del script
/// </summary>
internal class ScriptInterpreter : BaseJobSentenceIntepreter
{
	internal ScriptInterpreter(IProgramInterpreter programInterpreter) : base(programInterpreter) 
	{
		ProvidersCache = new(this);
	}

	/// <summary>
	///		Inicializa el intérprete
	/// </summary>
	public override async Task InitializeAsync(CancellationToken cancellationToken)
	{
		await Task.Delay(1, cancellationToken);
	}

	/// <summary>
	///		Procesa las sentencias del script
	/// </summary>
	public async override Task<bool> ProcessAsync(SentenceBase sentenceBase, CancellationToken cancellationToken)
	{
		bool executed = true;

			// Ejecuta la sentencia
			switch (sentenceBase)
			{
				case CallApiSentence sentence:
						await ProcessCallApiAsync(sentence, cancellationToken);
					break;
				default: // indica que no se ha ejecutado nada
						executed = false;
					break;
			}
			// Devuelve el valor que indica si se ha ejecutado
			return executed;
	}

	/// <summary>
	///		Procesa una llamada a la API
	/// </summary>
	private async Task ProcessCallApiAsync(CallApiSentence sentence, CancellationToken cancellationToken)
	{
		RestConnection? connection = ProvidersCache.Get(sentence.Target);

			// Log
			LogInfo($"Start call api {sentence.EndPoint} {sentence.Method.ToString()}");
			// Procesa la sentencia
			if (connection is null)
				AddError($"Can't find Rest context for {sentence.Target}");
			else
			{
				List<SentenceBase> sentences;

					// Añade un ámbito nuevo a la ejecución
					ProgramInterpreter.AddScope();
					// Ejecuta la llamada a la API
					sentences = await new Processor.ApiProcessor(this, connection).ProcessAsync(sentence, cancellationToken);
					// Ejecuta las sentencias
					if (sentences.Count > 0)
						await ProcessAsync(sentences, cancellationToken);
					// Elimina el ámbito de la ejecución
					ProgramInterpreter.RemoveScope();
			}
			// Log
			LogInfo($"End call api {sentence.EndPoint} {sentence.Method.ToString()}");
	}

	/// <summary>
	///		Caché de proveedores
	/// </summary>
	private RestProvidersCache ProvidersCache { get; }
}