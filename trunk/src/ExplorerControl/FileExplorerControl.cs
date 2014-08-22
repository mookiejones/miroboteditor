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

namespace miRobotEditor.ExplorerControl
{
    /// <summary>
    ///     Description of FileExplorerControl.
    /// </summary>
    public sealed partial class FileExplorerControl : UserControl
    {
        #region Events

        public event FileSelectedEventHandler FileSelected
        {
            add { explorer.OnFileSelected += value; }
            remove { explorer.OnFileSelected -= value; }
        }

        public new event TreeNodeMouseClickEventHandler OnMouseClick;
        public new event KeyEventHandler OnKeyUp;
        public event TreeViewEventHandler OnAfterSelect;

        #endregion

        [Localizable(false)] private string _filter = "*.*";

        public FileExplorerControl()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            Instance = this;
            explorer.ShowTree();
        }

        public static FileExplorerControl Instance { get; set; }

        [Localizable(false)]
        public String Filter
        {
            get { return String.IsNullOrEmpty(_filter) ? "*.*" : _filter; }
            set { _filter = value; }
        }

        private void RaiseAfterSelect(object sender, TreeViewEventArgs e)
        {
            if (OnAfterSelect != null)
                OnAfterSelect(sender, e);
        }

        private void RaiseKeyUp(object sender, KeyEventArgs e)
        {
            if (OnKeyUp != null)
                OnKeyUp(sender, e);
        }

        private void RaiseMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (OnMouseClick != null)
                OnMouseClick(sender, e);
        }

        private void CopyFile(object sender, EventArgs e)
        {
            // TODO: Implement CopyFile
        }

        private void CutFile(object sender, EventArgs e)
        {
            // TODO: Implement Cut_File
        }

        private void PasteFile(object sender, EventArgs e)
        {
            // TODO: Implement Cut_File
        }

        private void DeleteFile(object sender, EventArgs e)
        {
            // TODO: Implement Cut_File
        }

        private void Refresh(object sender, EventArgs e)
        {
            if (explorer.SelectedNode == null) return;

            explorer.SelectedNode.Nodes.Clear();
            explorer.FillTreeNode(explorer.SelectedNode, explorer.SelectedNode.Tag.ToString());
            // TODO: Implement Refresh
        }

        private void ContextOpening(object sender, CancelEventArgs e)
        {
            bool enabled =
                !(String.IsNullOrEmpty(explorer.SelectedFile) && String.IsNullOrEmpty(explorer.SelectedDirectory));
            mnuCopy.Enabled = enabled;
            mnuCut.Enabled = enabled;
            mnuDelete.Enabled = enabled;
            mnuPaste.Enabled = enabled;
        }
    }
}