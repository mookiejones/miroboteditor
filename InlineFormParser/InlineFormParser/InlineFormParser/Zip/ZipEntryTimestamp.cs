#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:51 AM
// Modified:2018:02:01:9:49 AM:

#endregion

#region Usings

using System;

#endregion

namespace InlineFormParser.Zip
{
	[Flags]
	public enum ZipEntryTimestamp
	{
		None = 0,
		DOS = 1,
		Windows = 2,
		Unix = 4,
		InfoZip1 = 8
	}
}