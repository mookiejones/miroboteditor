#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:33 AM
// Modified:2018:02:01:9:48 AM:

#endregion

namespace InlineFormParser.Model
{
	public enum MessageType
	{
		Unknown = -1,
		Info,
		Warning,
		Error,
		State,
		Acknowledgment,
		Wait,
		Dialog,
		Event,
		InfoOrAcknowledgment,
		LogOnly
	}
}