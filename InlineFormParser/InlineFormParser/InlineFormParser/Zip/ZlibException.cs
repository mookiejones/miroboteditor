#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:01 AM
// Modified:2018:02:01:9:49 AM:

#endregion

#region Usings

using System;
using System.Runtime.InteropServices;

#endregion

namespace InlineFormParser.Zip
{
	[Guid("ebc25cf6-9120-4283-b972-0e5520d0000E")]
	public class ZlibException : Exception
	{
		public ZlibException()
		{
		}

		public ZlibException(string s)
			: base(s)
		{
		}
	}
}