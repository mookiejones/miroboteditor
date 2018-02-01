#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:04 AM
// Modified:2018:02:01:9:49 AM:

#endregion

namespace InlineFormParser.Zip
{
	internal static class InternalInflateConstants
	{
		internal static readonly int[] InflateMask = new int[17]
		{
			0,
			1,
			3,
			7,
			15,
			31,
			63,
			127,
			255,
			511,
			1023,
			2047,
			4095,
			8191,
			16383,
			32767,
			65535
		};
	}
}