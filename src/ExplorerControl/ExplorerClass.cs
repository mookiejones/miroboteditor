/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 10/30/2012
 * Time: 12:37
 * 
 */

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using miRobotEditor.GUI.ExplorerControl;
using miRobotEditor.ViewModel;

namespace miRobotEditor.ExplorerControl
{
    /// <summary>
    ///     Description of ExplorerClass.
    /// </summary>
    public sealed class ExplorerClass : TreeView, IComparable
    {
        //TODO Fix this
// ReSharper disable InconsistentNaming
#pragma warning disable 169
// ReSharper disable UnusedMember.Local
        private const int FOLDER = 0;
        private const int FOLDEROPEN = 1;
        private const int REMOVABLE = 5;
        private const int CDDRIVE = 3;
        private const int FIXEDDRIVE = 2;
        private const int GENERICFILE = 6;
        private const int NETWORK = 7;
        // ReSharper restore InconsistentNaming
        // ReSharper restore UnusedMember.Local
#pragma warning restore 169


        public string SelectedFile { get; set; }
        public string SelectedDirectory { get; set; }

        public event FileSelectedEventHandler OnFileSelected;

        public int CompareTo(object obj)
        {
            var directoryInfo = (DirectoryInfo) obj;
            return String.CompareOrdinal(Name, directoryInfo.Name);
        }

        public ExplorerClass()
        {
            HideSelection = false;
        }

        public void AddRobotNode(string sFile, string sName)
        {
            SelectedNode = Nodes.Add(sFile, sName, 11, 11);
            SelectedNode.ToolTipText = sFile;
        }

        private void RaiseFileSelected(object sender, FileSelectedEventArgs e)
        {
            if (OnFileSelected != null)
                OnFileSelected(sender, e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (File.Exists(SelectedNode.FullPath))
            {
                RaiseFileSelected(this, new FileSelectedEventArgs {Filename = SelectedNode.FullPath});
            }

            base.OnMouseDoubleClick(e);
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            if (SelectedNode != null)
            {
                string fp = SelectedNode.FullPath;

                SelectedFile = File.Exists(fp) ? fp : String.Empty;
                SelectedDirectory = Directory.Exists(fp) ? fp : String.Empty;
            }
            base.OnAfterSelect(e);
        }


        public void ShowTree()
        {
            Nodes.Clear();
            checked
            {
                try
                {
                    foreach (DriveInfo d in DriveInfo.GetDrives())
                    {
                        switch (d.DriveType)
                        {
                            case DriveType.Unknown:
                            {
                                AddNode(d.Name, 7, 7);
                                break;
                            }
                            case DriveType.Removable:
                                switch (d.Name)
                                {
                                    case "A:\\":
                                    case "B:\\":
                                        AddNode(d.Name, REMOVABLE, REMOVABLE);
                                        break;
                                    default:
                                        AddNode(d.Name, REMOVABLE, REMOVABLE);
                                        break;
                                }
                                break;
                            case DriveType.Fixed:
                            {
                                AddNode(d.Name, FIXEDDRIVE, FIXEDDRIVE);
                                break;
                            }
                            case DriveType.Network:
                            {
                                AddNode(d.Name, NETWORK, NETWORK);
                                break;
                            }
                            case DriveType.CDRom:
                            {
                                AddNode(d.Name, CDDRIVE, CDDRIVE);
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageViewModel.AddError("ExplorerClass", ex);
                }
            }
        }

        private void AddNode(string name, int unselected, int selected)
        {
            var node = new TreeNode(name, unselected, selected);
            Nodes.Add(node);
            node.Tag = name;
            node.Nodes.Add(String.Empty);
        }

        public void ShowTree(string path, bool bArchiveRoot, string sRobName, bool bSelect)
        {
            int num = 5;
            string text = path;
            if (bArchiveRoot)
            {
                num = 11;
                text = sRobName;
            }

            var treeNode = new TreeNode(text, num, num);


            if (bArchiveRoot)
            {
                treeNode.Tag = path;
                Nodes.Add(treeNode);
            }
            else
            {
                Nodes.Add(treeNode);
            }
            FillTreeNode(treeNode, String.Empty);
            if (bSelect)
            {
                SelectedNode = treeNode;
            }
        }

        public void ShowTree(DriveType driveType)
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            checked
            {
                foreach (DriveInfo driveInfo in drives.Where(driveInfo => driveInfo.DriveType == driveType))
                {
                    ShowTree(driveInfo.Name, false, "", false);
                }
            }
        }

        [Localizable(false)]
        public void FillTreeNode(TreeNode node, string root)
        {
            if (root == null) throw new ArgumentNullException("root");
            checked
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    string text = node.FullPath;
                    const string folder = @"\";
                    if (String.CompareOrdinal(text, folder) == 0)
                    {
                        text = node.ToString();
                    }
                    else
                    {
                        if (String.CompareOrdinal(text.Substring(1, 1), ":") != 0)
                        {
                            root = node.Text;
                            text = root + text.Substring(text.IndexOf(folder, StringComparison.Ordinal));
                        }
                    }
                    var directoryInfo = new DirectoryInfo(text);
                    DirectoryInfo[] directories = directoryInfo.GetDirectories();
                    var comparer = new Comparer();
                    Array.Sort(directories, comparer);
                    foreach (
                        TreeNode treeNode in
                            directories.Select(d => new TreeNode(d.Name, 0, 1) {Tag = node.Tag.ToString()}))
                    {
                        node.Nodes.Add(treeNode);
                        treeNode.Nodes.Add("");
                    }

                    string[] files = Directory.GetFiles(text, FileExplorerControl.Instance.Filter);
                    Array.Sort(files);
                    string[] array2 = files;
                    foreach (string f in array2)
                    {
                        var treeNode2 = new TreeNode(Path.GetFileName(f)) {Tag = node.Tag.ToString()};
                        string extension = Path.GetExtension(f);
                        if (extension != null)
                        {
                            string left = extension.ToLower();
                            switch (left)
                            {
                                case ".src":
                                    treeNode2.SelectedImageIndex = 6;
                                    treeNode2.ImageIndex = 6;
                                    break;
                                case ".dat":
                                    treeNode2.SelectedImageIndex = 6;
                                    treeNode2.ImageIndex = 6;
                                    break;
                                case ".sub":
                                    treeNode2.SelectedImageIndex = 6;
                                    treeNode2.ImageIndex = 6;
                                    break;
                                case ".zip":
                                    treeNode2.SelectedImageIndex = 6;
                                    treeNode2.ImageIndex = 6;
                                    break;
                                default:
                                    treeNode2.SelectedImageIndex = 6;
                                    treeNode2.ImageIndex = 6;
                                    break;
                            }
                        }


                        node.Nodes.Add(treeNode2);
                    }
                    Cursor = Cursors.Default;
                }
                catch (Exception ex)
                {
                    MessageViewModel.AddError("ExplorerClass.FillTreeNode", ex);
                    Cursor = Cursors.Default;
                }
            }
        }

        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;
            BeginUpdate();
            node.Nodes.Clear();
            string sRoot = e.Node.Tag.ToString();
            FillTreeNode(node, sRoot);
            EndUpdate();
            base.OnBeforeExpand(e);
        }
    }
}