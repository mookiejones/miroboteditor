// Copyright (c) AlphaSierraPapa for the miRobotEditor Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace DMC_Engineering.miRobotEditor.Sda
{
	/// <summary>
	/// EventArgs for the <see cref="miRobotEditorHost.FileLoaded">miRobotEditorHost.FileLoaded</see>
	/// and <see cref="miRobotEditorHost.FileSaved">miRobotEditorHost.FileSaved</see> events.
	/// </summary>
	[Serializable]
	public class FileEventArgs : EventArgs
	{
		string fileName;
		
		/// <summary>
		/// Gets the file name.
		/// </summary>
		public string FileName {
			get {
				return fileName;
			}
		}
		
		/// <summary>
		/// Creates a new instance of the FileEventArgs class.
		/// </summary>
		public FileEventArgs(string fileName)
		{
			this.fileName = fileName;
		}
	}
}
