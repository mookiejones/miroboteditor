#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:37 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System.IO;

#endregion

namespace InlineFormParser.Model
{
	public abstract class FileSource
	{
		public abstract Stream OpenReader();
	}
}