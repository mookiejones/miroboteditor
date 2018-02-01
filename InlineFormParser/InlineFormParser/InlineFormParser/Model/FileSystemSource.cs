#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:33 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System.IO;

#endregion

namespace InlineFormParser.Model
{
	public class FileSystemSource : FileSource
	{
		public FileSystemSource(string path)
		{
			Path = path;
		}

		public string Path { get; }

		public override Stream OpenReader()
		{
			return File.OpenRead(Path);
		}

		public override string ToString()
		{
			return Path;
		}
	}
}