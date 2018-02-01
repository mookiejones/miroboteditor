using System;
using System.Runtime.Serialization;

namespace InlineFormParser.Model
{
	[Serializable]
	public sealed class InvalidConfigFileException : AdeException
	{
		public InvalidConfigFileException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public InvalidConfigFileException(string message)
			: base(message)
		{
		}

		public InvalidConfigFileException()
		{
		}

		private InvalidConfigFileException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}