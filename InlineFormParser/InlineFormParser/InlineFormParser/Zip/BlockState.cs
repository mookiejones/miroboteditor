#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:57 AM
// Modified:2018:02:01:9:49 AM:

#endregion

namespace InlineFormParser.Zip
{
	internal enum BlockState
	{
		NeedMore,
		BlockDone,
		FinishStarted,
		FinishDone
	}
}