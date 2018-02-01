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

namespace InlineFormParser.Zip
{
	public class AddProgressEventArgs : ZipProgressEventArgs
	{
		internal AddProgressEventArgs()
		{
		}

		private AddProgressEventArgs(string archiveName, ZipProgressEventType flavor)
			: base(archiveName, flavor)
		{
		}

		internal static AddProgressEventArgs AfterEntry(string archiveName, ZipEntry entry, int entriesTotal)
		{
			AddProgressEventArgs addProgressEventArgs =
				new AddProgressEventArgs(archiveName, ZipProgressEventType.Adding_AfterAddEntry);
			addProgressEventArgs.EntriesTotal = entriesTotal;
			addProgressEventArgs.CurrentEntry = entry;
			return addProgressEventArgs;
		}

		internal static AddProgressEventArgs Started(string archiveName)
		{
			return new AddProgressEventArgs(archiveName, ZipProgressEventType.Adding_Started);
		}

		internal static AddProgressEventArgs Completed(string archiveName)
		{
			return new AddProgressEventArgs(archiveName, ZipProgressEventType.Adding_Completed);
		}
	}
}