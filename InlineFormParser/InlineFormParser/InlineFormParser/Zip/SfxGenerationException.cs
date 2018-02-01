#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:29 AM
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
	[Guid("ebc25cf6-9120-4283-b972-0e5520d00008")]
	public class SfxGenerationException : ZipException
	{
		public SfxGenerationException()
		{
		}

		public SfxGenerationException(string message)
			: base(message)
		{
		}

		protected SfxGenerationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}