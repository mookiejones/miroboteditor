#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:01 AM
// Modified:2018:02:01:9:48 AM:

#endregion

namespace InlineFormParser.Common.Tracing
{
	public interface IPerformanceLogger
	{
		void LogTimestamp(string key);
	}
}