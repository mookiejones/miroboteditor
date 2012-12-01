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
using System.Collections;
namespace miRobotEditor.GUI.ExplorerControl
{
	
	public delegate void FileSelectedEventHandler (object sender, FileSelectedEventArgs e);
	/// <summary>
	/// Description of ExplorerClass.
	/// </summary>
	public class ExplorerClass:TreeView,IComparable
	{

        //TODO Fix this
		private const int FOLDER = 0;
		private const int FOLDEROPEN=1;
		private const int REMOVABLE = 5;
		private const int CDDRIVE=3;
		private const int FIXEDDRIVE=2;
		private const int GENERICFILE=5;
		
		
		public string SelectedFile{get;set;}
		public string SelectedDirectory{get;set;}
	
		public event FileSelectedEventHandler OnFileSelected;
		
		public int CompareTo(object obj)
		{
			var directoryInfo = (DirectoryInfo)obj;
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
		
		void RaiseFileSelected(object sender, FileSelectedEventArgs e)
		{
			if (OnFileSelected!=null)
				OnFileSelected(sender,e);
		}
		
		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{			
			if (File.Exists(SelectedNode.FullPath))
			    {
			    	RaiseFileSelected(this,new FileSelectedEventArgs{Filename=SelectedNode.FullPath});
			    }
			
			base.OnMouseDoubleClick(e);
		}
		
		protected override void OnAfterSelect(TreeViewEventArgs e)
		{
			if (SelectedNode!=null)
			{
			    var fp = SelectedNode.FullPath;

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
					foreach (var d in DriveInfo.GetDrives())
					{
						switch (d.DriveType)
						{
							case DriveType.Unknown:
							{
								AddNode(d.Name,7,7);
								break;
							}
							case DriveType.Removable:
									switch (d.Name)
									{
										case "A:\\":
										case "B:\\":
											AddNode(d.Name,REMOVABLE,REMOVABLE);
											break;
										default:
											AddNode(d.Name,REMOVABLE,REMOVABLE);
											break;
									}							
								break;
							case DriveType.Fixed:
							{
								AddNode(d.Name,FIXEDDRIVE,FIXEDDRIVE);
								break;
							}
							case DriveType.Network:
							{
								AddNode(d.Name,10,10);
								break;
							}
							case DriveType.CDRom:
							{
									AddNode(d.Name,CDDRIVE,CDDRIVE);
								break;
							}
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
			//TODO		MainWindow.Instance.Output.Add("ExplorerClass",ex.ToString());
				}
			}
		}
		
		private void AddNode(string name, int unselected, int selected)
		{
			var node = new TreeNode(name,unselected,selected);			
			Nodes.Add(node);
			node.Tag=name;
			node.Nodes.Add(String.Empty);
		}
	
		public void ShowTree(string path, bool bArchiveRoot , string sRobName , bool bSelect )
		{
			var num = 5;
			var text = path;
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
			FillTreeNode(treeNode,String.Empty);
			if (bSelect)
			{
				SelectedNode = treeNode;
			}
		}
	
		public void ShowTree(DriveType driveType)
		{
			var drives = DriveInfo.GetDrives();
			checked
			{
				foreach (var driveInfo in drives.Where(driveInfo => driveInfo.DriveType == driveType))
				{
				    ShowTree(driveInfo.Name, false, "", false);
				}
			}
		}

	    [Localizable(false)]
	    public void FillTreeNode(TreeNode node, string root)
		{
			checked
			{
				try
				{
					Cursor = Cursors.WaitCursor;
					var text = node.FullPath;
				    const string folder = @"\";
					if (String.CompareOrdinal(text, folder)==0)
					{
						text = node.ToString();
					}
					else
					{
						if (String.CompareOrdinal(text.Substring(1,1), ":")!=0)
						{
							
							root = node.Text;
							text = root + text.Substring(text.IndexOf(folder, StringComparison.Ordinal));
						}
					}
					var directoryInfo = new DirectoryInfo(text);
					var directories = directoryInfo.GetDirectories();
					var comparer = new Comparer();
					Array.Sort(directories, comparer);					
					foreach (var d in directories)
					{						
						var treeNode = new TreeNode(d.Name, 0, 1) {Tag = node.Tag.ToString()};
					    node.Nodes.Add(treeNode);
						treeNode.Nodes.Add("");
					}
					
					var files = Directory.GetFiles(text, FileExplorerControl.Instance.Filter);
					Array.Sort(files);
					var array2 = files;
					foreach (var f in array2)
					{
					    var treeNode2 = new TreeNode(Path.GetFileName(f)) {Tag = node.Tag.ToString()};
					    var left = Path.GetExtension(f).ToLower();
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
							
						
						node.Nodes.Add(treeNode2);
					}
					Cursor = Cursors.Default;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				//TODO	MainWindow.Instance.Output.Add("FileExplorerClass",ex.ToString());
					Cursor = Cursors.Default;
				}
			}
		}
	
		protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
		{
			var node = e.Node;
			BeginUpdate();
			node.Nodes.Clear();
			var sRoot = e.Node.Tag.ToString();			
			FillTreeNode(node, sRoot);
			EndUpdate();
			base.OnBeforeExpand(e);
		}
	}
	class Comparer:IComparer
	{
		public int Compare(object x, object y)
		{
			var directoryInfo = (DirectoryInfo)x;
			var directoryInfo2 = (DirectoryInfo)y;
			var name = directoryInfo.Name;
			var name2 = directoryInfo2.Name;
			return String.CompareOrdinal(name, name2);
		}
	}
	public class FileSelectedEventArgs:EventArgs
	{
		public string Filename{get;set;}
	}
}
