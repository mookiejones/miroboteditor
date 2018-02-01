using System;
using System.Runtime.Serialization;

namespace InlineFormParser.Exceptions
{
	
	public class StartupCheckFailedException : Exception
	{
		public StartupCheckFailedException()
		{
		}

		public StartupCheckFailedException(string message)
			: base(message)
		{
		}

		protected StartupCheckFailedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public StartupCheckFailedException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
