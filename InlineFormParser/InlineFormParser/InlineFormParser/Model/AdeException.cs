using System;
using System.Runtime.Serialization;

namespace InlineFormParser.Model
{
	[Serializable]
	public abstract class AdeException : Exception
	{
		protected AdeException(string message, Exception innerException)
			: base(message, innerException)
		{
			Tracing.ExceptionCreatedLine("{0} created: \n{1}", GetType().ToString(), message);
		}

		protected AdeException(string message)
			: base(message)
		{
			Tracing.ExceptionCreatedLine("{0} created: \n{1}", GetType().ToString(), message);
		}

		protected AdeException()
		{
			Tracing.ExceptionCreatedLine("{0} created.", GetType().ToString());
		}

		protected AdeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}