// Copyright (c) AlphaSierraPapa for the miRobotEditor Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;

namespace DMC_Engineering.miRobotEditor.Sda
{
	/// <summary>
	/// This class contains properties to control how the miRobotEditor
	/// workbench is being run.
	/// </summary>
	[Serializable]
	public sealed class WorkbenchSettings
	{
		bool runOnNewThread = true;
		Collection<string> fileList = new Collection<string>();
		
		/// <summary>
		/// Gets/Sets whether to create a new thread to run the workbench on.
		/// The default value is true.
		/// </summary>
		public bool RunOnNewThread {
			get {
				return runOnNewThread;
			}
			set {
				runOnNewThread = value;
			}
		}
		
		/// <summary>
		/// Put files to open at workbench startup into this collection.
		/// </summary>
		public Collection<string> InitialFileList {
			get {
				return fileList;
			}
		}
	}
}
