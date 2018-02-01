#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:11 AM
// Modified:2018:02:01:9:50 AM:

#endregion

#region Usings

using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

#endregion

namespace InlineFormParser.Zip
{
	[Serializable]
	[Guid("ebc25cf6-9120-4283-b972-0e5520d00007")]
	public class BadStateException : ZipException
	{
		public BadStateException()
		{
		}

		public BadStateException(string message)
			: base(message)
		{
		}

		public BadStateException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected BadStateException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}