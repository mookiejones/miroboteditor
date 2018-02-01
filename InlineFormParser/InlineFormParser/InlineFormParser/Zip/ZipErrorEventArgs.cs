#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:28 AM
// Modified:2018:02:01:9:50 AM:

#endregion

#region Usings

using System;

#endregion

namespace InlineFormParser.Zip
{
	public class ZipErrorEventArgs : ZipProgressEventArgs
	{
		private ZipErrorEventArgs()
		{
		}

		public Exception Exception { get; private set; }

		public string FileName => CurrentEntry.LocalFileName;

		internal static ZipErrorEventArgs Saving(string archiveName, ZipEntry entry, Exception exception)
		{
			ZipErrorEventArgs zipErrorEventArgs = new ZipErrorEventArgs();
			zipErrorEventArgs.EventType = ZipProgressEventType.Error_Saving;
			zipErrorEventArgs.ArchiveName = archiveName;
			zipErrorEventArgs.CurrentEntry = entry;
			zipErrorEventArgs.Exception = exception;
			return zipErrorEventArgs;
		}
	}
}