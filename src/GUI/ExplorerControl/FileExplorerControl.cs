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
using System.IO;
using System.Windows.Forms;

namespace miRobotEditor.GUI.ExplorerControl
{
	

	
	/// <summary>
	/// Description of FileExplorerControl.
	/// </summary>
	public partial class FileExplorerControl : UserControl
	{
		
		
			
		#region Events
		public event FileSelectedEventHandler FileSelected
		{
			add{explorer.OnFileSelected+= value;}
			remove{explorer.OnFileSelected-=value;}
		}
		public new event TreeNodeMouseClickEventHandler OnMouseClick;
		public new  event KeyEventHandler OnKeyUp;
		public event TreeViewEventHandler OnAfterSelect;
		#endregion
		public static FileExplorerControl Instance{get;set;}
		
		private string _filter = "*.*";
		public String Filter
		{
			get
			{
				if (String.IsNullOrEmpty(_filter))
					return "*.*";
				return _filter;
			}
			set {_filter=value;}
		}
		
		public FileExplorerControl()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			Instance = this;
			explorer.ShowTree();
			
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
		
		
		
		void Copy_File(object sender, EventArgs e)
		{
			// TODO: Implement CopyFile
		}
		
		void Cut_File(object sender, EventArgs e)
		{
			// TODO: Implement Cut_File
		}

		void Paste_File(object sender, EventArgs e)
		{
			// TODO: Implement Cut_File
		}

		void Delete_File(object sender, EventArgs e)
		{			
			// TODO: Implement Cut_File
		}
		
		void Refresh(object sender, EventArgs e)
		{
			if (explorer.SelectedNode!=null)
			{
				explorer.SelectedNode.Nodes.Clear();
				var r = explorer.SelectedNode.Tag.ToString();
				explorer.FillTreeNode(explorer.SelectedNode,explorer.SelectedNode.Tag.ToString());
			}
			// TODO: Implement Refresh
		}
		
		void ContextOpening(object sender, CancelEventArgs e)
		{
			
			var enabled =  !(String.IsNullOrEmpty(explorer.SelectedFile) && String.IsNullOrEmpty(explorer.SelectedDirectory));
			mnuCopy.Enabled=enabled;
			mnuCut.Enabled=enabled;
			mnuDelete.Enabled=enabled;
			mnuPaste.Enabled=enabled;					
		}
	}
}
