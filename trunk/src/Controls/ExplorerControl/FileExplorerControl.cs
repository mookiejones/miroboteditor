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
		
		#region Events
		public new event  TreeNodeMouseClickEventHandler OnDoubleClick;
		public new event TreeNodeMouseClickEventHandler OnMouseClick;
		public new  event KeyEventHandler OnKeyUp;
		public event TreeViewEventHandler OnAfterSelect;
		#endregion
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
		
		
	
		
	
		
		
		
		void RaiseAfterSelect(object sender, TreeViewEventArgs e)
		{
			if (OnAfterSelect!=null)
				OnAfterSelect(sender,e);
		}
		
		void RaiseKeyUp(object sender, KeyEventArgs e)
		{
		if (OnKeyUp!=null)
				OnKeyUp(sender,e);
		}
		
		void RaiseMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (OnMouseClick!=null)
				OnMouseClick(sender,e);
		}
		
		void RaiseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (OnDoubleClick!=null)
				OnDoubleClick(sender,e);
		}
	}
}
