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
	public enum ZipProgressEventType
	{
		Adding_Started,
		Adding_AfterAddEntry,
		Adding_Completed,
		Reading_Started,
		Reading_BeforeReadEntry,
		Reading_AfterReadEntry,
		Reading_Completed,
		Reading_ArchiveBytesRead,
		Saving_Started,
		Saving_BeforeWriteEntry,
		Saving_AfterWriteEntry,
		Saving_Completed,
		Saving_AfterSaveTempArchive,
		Saving_BeforeRenameTempArchive,
		Saving_AfterRenameTempArchive,
		Saving_AfterCompileSelfExtractor,
		Saving_EntryBytesRead,
		Extracting_BeforeExtractEntry,
		Extracting_AfterExtractEntry,
		Extracting_ExtractEntryWouldOverwrite,
		Extracting_EntryBytesWritten,
		Extracting_BeforeExtractAll,
		Extracting_AfterExtractAll,
		Error_Saving
	}
}