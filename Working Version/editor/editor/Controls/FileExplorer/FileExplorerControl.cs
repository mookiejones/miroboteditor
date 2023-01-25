using CommunityToolkit.Mvvm.Messaging;
using miRobotEditor.Enums;
using miRobotEditor.Languages;
using miRobotEditor.Messages;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace miRobotEditor.Controls.FileExplorer
{
    public sealed class FileExplorerControl : TreeView, IComparable
    {
        // ReSharper disable UnusedMember.Local
        // ReSharper disable InconsistentNaming
        private const int FOLDER = 0;

        private const int FOLDEROPEN = 1;
        private const int REMOVABLE = 5;
        private const int CDDRIVE = 3;
        private const int FIXEDDRIVE = 2;
        private const int GENERICFILE = 6;
        private const int NETWORK = 7;

        public FileExplorerControl()
        {
            base.HideSelection = false;
            ShowTree();
        }

        // ReSharper restore InconsistentNaming
        // ReSharper restore UnusedMember.Local
        public string SelectedFile { get; set; }

        public string SelectedDirectory { get; set; }

        public int CompareTo(object obj)
        {
            DirectoryInfo directoryInfo = (DirectoryInfo)obj;
            return string.CompareOrdinal(Name, directoryInfo.Name);
        }

        public event FileSelectedEventHandler OnFileSelected;

        public void AddRobotNode(string sFile, string sName)
        {
            SelectedNode = Nodes.Add(sFile, sName, 11, 11);
            SelectedNode.ToolTipText = sFile;
        }

        private void RaiseFileSelected(object sender, FileSelectedEventArgs e) => OnFileSelected?.Invoke(sender, e);

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (SelectedNode == null) return;
            if (File.Exists(SelectedNode.FullPath))
            {
                RaiseFileSelected(this, new FileSelectedEventArgs
                {
                    Filename = SelectedNode.FullPath
                });
            }
            base.OnMouseDoubleClick(e);
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            if (SelectedNode != null)
            {
                string fullPath = SelectedNode.FullPath;
                SelectedFile = File.Exists(fullPath) ? fullPath : string.Empty;
                SelectedDirectory = Directory.Exists(fullPath) ? fullPath : string.Empty;
            }
            base.OnAfterSelect(e);
        }

        public void ShowTree()
        {
            base.Nodes.Clear();
            try
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                foreach (DriveInfo driveInfo in drives)
                {
                    switch (driveInfo.DriveType)
                    {
                        case DriveType.Unknown:
                            AddNode(driveInfo.Name, 7, 7);
                            break;

                        case DriveType.Removable:
                            {
                                string name = driveInfo.Name;
                                if (name == null)
                                {
                                    goto IL_9E;
                                }
                                if (name != "A:\\" && name != "B:\\")
                                {
                                    goto IL_9E;
                                }
                                AddNode(driveInfo.Name, 5, 5);
                                break;
                            IL_9E:
                                AddNode(driveInfo.Name, 5, 5);
                                break;
                            }
                        case DriveType.Fixed:
                            AddNode(driveInfo.Name, 2, 2);
                            break;

                        case DriveType.Network:
                            AddNode(driveInfo.Name, 7, 7);
                            break;

                        case DriveType.CDRom:
                            AddNode(driveInfo.Name, 3, 3);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage msg = new("ExplorerClass", ex, MessageType.Error);
                _ = WeakReferenceMessenger.Default.Send<IMessage>(msg);
            }
        }

        private void AddNode(string name, int unselected, int selected)
        {
            TreeNode treeNode = new(name, unselected, selected);
            _ = Nodes.Add(treeNode);
            treeNode.Tag = name;
            _ = treeNode.Nodes.Add(string.Empty);
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
            TreeNode treeNode = new(text, num, num);
            if (bArchiveRoot)
            {
                treeNode.Tag = path;
                _ = Nodes.Add(treeNode);
            }
            else
            {
                _ = Nodes.Add(treeNode);
            }
            FillTreeNode(treeNode, string.Empty);
            if (bSelect)
            {
                SelectedNode = treeNode;
            }
        }

        public void ShowTree(DriveType driveType)
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo current in
                from driveInfo in drives
                where driveInfo.DriveType == driveType
                select driveInfo)
            {
                ShowTree(current.Name, false, "", false);
            }
        }

        [Localizable(false)]
        public void FillTreeNode(TreeNode node, string root)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }
            try
            {
                Cursor = Cursors.WaitCursor;
                string text = node.FullPath;
                if (string.CompareOrdinal(text, "\\") == 0)
                {
                    text = node.ToString();
                }
                else
                {
                    if (string.CompareOrdinal(text.Substring(1, 1), ":") != 0)
                    {
                        root = node.Text;
                        text = root + text.Substring(text.IndexOf("\\", StringComparison.Ordinal));
                    }
                }
                DirectoryInfo directoryInfo = new(text);
                DirectoryInfo[] directories = directoryInfo.GetDirectories();
                //Comparer comparer = new Comparer(CultureInfo.InvariantCulture);
                //Array.Sort(directories, comparer);


                var nodes = directories.Select(o => new TreeNode(o.Name, 0, 1) { Tag = node.Tag.ToString() });
                foreach(var n in nodes)
                {
                    node.Nodes.Add(n);
                    n.Nodes.Add("");
                }
 


 
                string[] files = Directory.GetFiles(text, Views.FileExplorerControl.Instance.Filter);
                Array.Sort(files);
                string[] array = files;
                string[] array2 = array;

             
                foreach (string path in array2)
                {
                    TreeNode treeNode = new(Path.GetFileName(path))
                    {
                        Tag = node.Tag.ToString()
                    };
                    string extension = Path.GetExtension(path)??"".ToLowerInvariant();
                    if (!string.IsNullOrEmpty(extension))
                    {
                        var isKuka = KUKA.IsKukaFile(extension);
                        if (isKuka)
                        {
                            treeNode.SelectedImageIndex = 6;
                            treeNode.ImageIndex = 6;
                        }
                    }
                    _ = node.Nodes.Add(treeNode);
                }
                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                ErrorMessage msg = new("ExplorerClass.FillTreeNode", ex, MessageType.Error);
                _ = WeakReferenceMessenger.Default.Send<IMessage>(msg);
                Cursor = Cursors.Default;
            }
        }

        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;
            base.BeginUpdate();
            node.Nodes.Clear();
            string root = $"{e.Node.Tag}";
            FillTreeNode(node, root);
            base.EndUpdate();
            base.OnBeforeExpand(e);
        }

        public class FileSelectedEventArgs : EventArgs
        {
            public string Filename { get; set; }
        }
    }


}
