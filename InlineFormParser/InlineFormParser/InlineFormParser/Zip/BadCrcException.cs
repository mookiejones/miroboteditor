#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:16 AM
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
	[Guid("ebc25cf6-9120-4283-b972-0e5520d00009")]
	public class BadCrcException : ZipException
	{
		public BadCrcException()
		{
		}

		public BadCrcException(string message)
			: base(message)
		{
		}

		protected BadCrcException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}