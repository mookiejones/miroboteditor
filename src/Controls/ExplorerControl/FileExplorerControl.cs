/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 10/30/2012
 * Time: 12:37
 * 
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace miRobotEditor.Controls.ExplorerControl
{
	/// <summary>
	/// Description of FileExplorerControl.
	/// </summary>
	public partial class FileExplorerControl : UserControl
	{
		public static FileExplorerControl Instance{get;set;}
		public String Filter{get {return cmbFilter.SelectedText;}}
		public FileExplorerControl()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			Instance = this;
			

		}
	}
}
