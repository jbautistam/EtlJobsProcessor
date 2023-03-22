using Bau.Libraries.LibJobProcessor.Rest.Models.Sentences;
using Bau.Libraries.LibInterpreter.Models.Sentences;
using Bau.Libraries.LibJobProcessor.Core.Interfaces.Interpreter;

namespace Bau.Libraries.LibJobProcessor.Rest.Manager;

/// <summary>
///		Intérprete del script
/// </summary>
internal class ScriptInterpreter : BaseJobSentenceIntepreter
{
	internal ScriptInterpreter(IProgramInterpreter programInterpreter) : base(programInterpreter) {}

	/// <summary>
	///		Inicializa el intérprete
	/// </summary>
	public override async Task InitializeAsync(CancellationToken cancellationToken)
	{
		await Task.Delay(1);
	}

	/// <summary>
	///		Procesa las sentencias del script
	/// </summary>
	public async override Task<bool> ProcessAsync(SentenceBase sentenceBase, CancellationToken cancellationToken)
	{
		switch (sentenceBase)
		{
			case CallApiSentence sentence:
					await ProcessCallApiAsync(sentence, cancellationToken);
				return true;
			default:
				return false;
		}
	}

	/// <summary>
	///		Procesa una llamada a la API
	/// </summary>
	private async Task ProcessCallApiAsync(CallApiSentence sentence, CancellationToken cancellationToken)
	{
		List<SentenceBase> sentences = await new Processor.ApiProcessor(this).ProcessAsync(sentence, cancellationToken);

			if (sentences.Count > 0)
				await ProcessAsync(sentences, cancellationToken);
	}
}