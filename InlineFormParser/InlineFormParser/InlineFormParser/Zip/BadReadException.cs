#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:47 AM
// Modified:2018:02:01:9:49 AM:

#endregion

#region Usings

using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

#endregion

namespace InlineFormParser.Zip
{
	[Serializable]
	[Guid("ebc25cf6-9120-4283-b972-0e5520d0000A")]
	public class BadReadException : ZipException
	{
		public BadReadException()
		{
		}

		public BadReadException(string message)
			: base(message)
		{
		}

		public BadReadException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected BadReadException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}