#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:25 AM
// Modified:2018:02:01:9:50 AM:

#endregion

#region Usings

using System;

#endregion

namespace InlineFormParser.Zip
{
	public class ZipProgressEventArgs : EventArgs
	{
		private bool _cancel;

		internal ZipProgressEventArgs()
		{
		}

		internal ZipProgressEventArgs(string archiveName, ZipProgressEventType flavor)
		{
			ArchiveName = archiveName;
			EventType = flavor;
		}

		public int EntriesTotal { get; set; }

		public ZipEntry CurrentEntry { get; set; }

		public bool Cancel
		{
			get => _cancel;
			set => _cancel = _cancel || value;
		}

		public ZipProgressEventType EventType { get; set; }

		public string ArchiveName { get; set; }

		public long BytesTransferred { get; set; }

		public long TotalBytesToTransfer { get; set; }
	}
}