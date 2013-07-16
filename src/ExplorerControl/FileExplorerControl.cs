/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 10/30/2012
 * Time: 12:37
 * 
 */
using System;
using System.ComponentModel;
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

	    [Localizable(false)] private string _filter = "*.*";

	    [Localizable(false)]
	    public String Filter
		{
			get
			{
			    return String.IsNullOrEmpty(_filter) ? "*.*" : _filter;
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
       
		void CopyFile(object sender, EventArgs e)
		{
			// TODO: Implement CopyFile
		}
		
		void CutFile(object sender, EventArgs e)
		{
			// TODO: Implement Cut_File
		}

		void PasteFile(object sender, EventArgs e)
		{
			// TODO: Implement Cut_File
		}

		void DeleteFile(object sender, EventArgs e)
		{			
			// TODO: Implement Cut_File
		}
		
		void Refresh(object sender, EventArgs e)
		{
		    if (explorer.SelectedNode == null) return;

		    explorer.SelectedNode.Nodes.Clear();
		    explorer.FillTreeNode(explorer.SelectedNode,explorer.SelectedNode.Tag.ToString());
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
