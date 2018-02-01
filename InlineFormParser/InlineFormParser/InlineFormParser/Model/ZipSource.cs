#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:34 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System.IO;
using InlineFormParser.Zip;

#endregion

namespace InlineFormParser.Model
{
	public class ZipSource : FileSource
	{
		internal ZipSource(string zipPath, ZipEntry entry)
		{
			ZipPath = zipPath;
			Entry = entry;
		}

		public string ZipPath { get; }

		public ZipEntry Entry { get; }

		public override Stream OpenReader()
		{
			return Entry.OpenReader();
		}

		public override string ToString()
		{
			return $"{ZipPath}:{Entry.FileName}";
		}
	}
}