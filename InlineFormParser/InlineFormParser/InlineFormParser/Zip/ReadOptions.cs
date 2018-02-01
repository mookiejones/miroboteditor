#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:26 AM
// Modified:2018:02:01:9:50 AM:

#endregion

#region Usings

using System;
using System.IO;
using System.Text;

#endregion

namespace InlineFormParser.Zip
{
	public class ReadOptions
	{
		public EventHandler<ReadProgressEventArgs> ReadProgress { get; set; }

		public TextWriter StatusMessageWriter { get; set; }

		public Encoding Encoding { get; set; }
	}
}