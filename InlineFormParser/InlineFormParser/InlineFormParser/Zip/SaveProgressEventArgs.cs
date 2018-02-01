#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:27 AM
// Modified:2018:02:01:9:50 AM:

#endregion

namespace InlineFormParser.Zip
{
	public class SaveProgressEventArgs : ZipProgressEventArgs
	{
		internal SaveProgressEventArgs(string archiveName, bool before, int entriesTotal, int entriesSaved, ZipEntry entry)
			: base(archiveName, (ZipProgressEventType) (before ? 9 : 10))
		{
			EntriesTotal = entriesTotal;
			CurrentEntry = entry;
			EntriesSaved = entriesSaved;
		}

		internal SaveProgressEventArgs()
		{
		}

		internal SaveProgressEventArgs(string archiveName, ZipProgressEventType flavor)
			: base(archiveName, flavor)
		{
		}

		public int EntriesSaved { get; }

		internal static SaveProgressEventArgs ByteUpdate(string archiveName, ZipEntry entry, long bytesXferred,
			long totalBytes)
		{
			SaveProgressEventArgs saveProgressEventArgs =
				new SaveProgressEventArgs(archiveName, ZipProgressEventType.Saving_EntryBytesRead);
			saveProgressEventArgs.ArchiveName = archiveName;
			saveProgressEventArgs.CurrentEntry = entry;
			saveProgressEventArgs.BytesTransferred = bytesXferred;
			saveProgressEventArgs.TotalBytesToTransfer = totalBytes;
			return saveProgressEventArgs;
		}

		internal static SaveProgressEventArgs Started(string archiveName)
		{
			return new SaveProgressEventArgs(archiveName, ZipProgressEventType.Saving_Started);
		}

		internal static SaveProgressEventArgs Completed(string archiveName)
		{
			return new SaveProgressEventArgs(archiveName, ZipProgressEventType.Saving_Completed);
		}
	}
}