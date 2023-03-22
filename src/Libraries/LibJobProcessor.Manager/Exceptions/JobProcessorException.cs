using System.Runtime.Serialization;

namespace Bau.Libraries.LibJobProcessor.Manager.Exceptions;

/// <summary>
///		Excepciones de <see cref="JobProjectManager"/>
/// </summary>
public class JobProcessorException : Exception
{
	public JobProcessorException() {}

	public JobProcessorException(string? message) : base(message) {}

	public JobProcessorException(string? message, Exception? innerException) : base(message, innerException) {}

	protected JobProcessorException(SerializationInfo info, StreamingContext context) : base(info, context) {}
}
