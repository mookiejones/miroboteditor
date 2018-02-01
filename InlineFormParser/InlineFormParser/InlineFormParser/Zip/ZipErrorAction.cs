#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:10 AM
// Modified:2018:02:01:9:50 AM:

#endregion

namespace InlineFormParser.Zip
{
	public enum ZipErrorAction
	{
		Throw,
		Skip,
		Retry,
		InvokeErrorEvent
	}
}