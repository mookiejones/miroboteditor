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

namespace InlineFormParser.Zip
{
	public class ExtractProgressEventArgs : ZipProgressEventArgs
	{
		internal ExtractProgressEventArgs(string archiveName, bool before, int entriesTotal, int entriesExtracted,
			ZipEntry entry, string extractLocation)
			: base(archiveName, (ZipProgressEventType) (before ? 17 : 18))
		{
			EntriesTotal = entriesTotal;
			CurrentEntry = entry;
			EntriesExtracted = entriesExtracted;
			ExtractLocation = extractLocation;
		}

		internal ExtractProgressEventArgs(string archiveName, ZipProgressEventType flavor)
			: base(archiveName, flavor)
		{
		}

		internal ExtractProgressEventArgs()
		{
		}

		public int EntriesExtracted { get; }

		public string ExtractLocation { get; private set; }

		internal static ExtractProgressEventArgs BeforeExtractEntry(string archiveName, ZipEntry entry,
			string extractLocation)
		{
			ExtractProgressEventArgs extractProgressEventArgs = new ExtractProgressEventArgs();
			extractProgressEventArgs.ArchiveName = archiveName;
			extractProgressEventArgs.EventType = ZipProgressEventType.Extracting_BeforeExtractEntry;
			extractProgressEventArgs.CurrentEntry = entry;
			extractProgressEventArgs.ExtractLocation = extractLocation;
			return extractProgressEventArgs;
		}

		internal static ExtractProgressEventArgs ExtractExisting(string archiveName, ZipEntry entry, string extractLocation)
		{
			ExtractProgressEventArgs extractProgressEventArgs = new ExtractProgressEventArgs();
			extractProgressEventArgs.ArchiveName = archiveName;
			extractProgressEventArgs.EventType = ZipProgressEventType.Extracting_ExtractEntryWouldOverwrite;
			extractProgressEventArgs.CurrentEntry = entry;
			extractProgressEventArgs.ExtractLocation = extractLocation;
			return extractProgressEventArgs;
		}

		internal static ExtractProgressEventArgs AfterExtractEntry(string archiveName, ZipEntry entry, string extractLocation)
		{
			ExtractProgressEventArgs extractProgressEventArgs = new ExtractProgressEventArgs();
			extractProgressEventArgs.ArchiveName = archiveName;
			extractProgressEventArgs.EventType = ZipProgressEventType.Extracting_AfterExtractEntry;
			extractProgressEventArgs.CurrentEntry = entry;
			extractProgressEventArgs.ExtractLocation = extractLocation;
			return extractProgressEventArgs;
		}

		internal static ExtractProgressEventArgs ExtractAllStarted(string archiveName, string extractLocation)
		{
			ExtractProgressEventArgs extractProgressEventArgs =
				new ExtractProgressEventArgs(archiveName, ZipProgressEventType.Extracting_BeforeExtractAll);
			extractProgressEventArgs.ExtractLocation = extractLocation;
			return extractProgressEventArgs;
		}

		internal static ExtractProgressEventArgs ExtractAllCompleted(string archiveName, string extractLocation)
		{
			ExtractProgressEventArgs extractProgressEventArgs =
				new ExtractProgressEventArgs(archiveName, ZipProgressEventType.Extracting_AfterExtractAll);
			extractProgressEventArgs.ExtractLocation = extractLocation;
			return extractProgressEventArgs;
		}

		internal static ExtractProgressEventArgs ByteUpdate(string archiveName, ZipEntry entry, long bytesWritten,
			long totalBytes)
		{
			ExtractProgressEventArgs extractProgressEventArgs =
				new ExtractProgressEventArgs(archiveName, ZipProgressEventType.Extracting_EntryBytesWritten);
			extractProgressEventArgs.ArchiveName = archiveName;
			extractProgressEventArgs.CurrentEntry = entry;
			extractProgressEventArgs.BytesTransferred = bytesWritten;
			extractProgressEventArgs.TotalBytesToTransfer = totalBytes;
			return extractProgressEventArgs;
		}
	}
}