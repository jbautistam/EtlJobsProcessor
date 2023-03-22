using System.Runtime.Serialization;

namespace Bau.Libraries.LibJobProcessor.Database.Exceptions;

/// <summary>
///		Excepción del procesador de base de datos
/// </summary>
public class JobDatabaseException : Exception
{
	public JobDatabaseException() {}

	public JobDatabaseException(string? message) : base(message) {}

	public JobDatabaseException(string? message, Exception? innerException) : base(message, innerException) {}

	protected JobDatabaseException(SerializationInfo info, StreamingContext context) : base(info, context) {}
}
