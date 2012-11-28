/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 10/30/2012
 * Time: 12:37
 * 
 */
using System;
using System.IO;
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
			return string.Compare(this.Name, directoryInfo.Name);
		}
		
		public ExplorerClass()
		{
			this.HideSelection = false;
		}
		public void AddRobotNode(string sFile, string sName)
		{
			this.SelectedNode = this.Nodes.Add(sFile, sName, 11, 11);
			this.SelectedNode.ToolTipText = sFile;
		}
		
		void RaiseFileSelected(object sender, FileSelectedEventArgs e)
		{
			if (OnFileSelected!=null)
				OnFileSelected(sender,e);
		}
		
		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{			
			if (File.Exists(this.SelectedNode.FullPath))
			    {
			    	RaiseFileSelected(this,new FileSelectedEventArgs{Filename=this.SelectedNode.FullPath});
			    }
			
			base.OnMouseDoubleClick(e);
		}
		
		protected override void OnAfterSelect(TreeViewEventArgs e)
		{
			if (SelectedNode!=null)
			{
				if (File.Exists(this.SelectedNode.FullPath))
				    SelectedFile=this.SelectedNode.FullPath;	
				else
					SelectedFile = String.Empty;
				if (Directory.Exists(this.SelectedNode.FullPath))
					SelectedDirectory=this.SelectedNode.FullPath;
				else
					SelectedDirectory=String.Empty;
			}
			base.OnAfterSelect(e);
		}
		
		
		public void ShowTree()
		{
			this.Nodes.Clear();
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
		
		private TreeNode AddNode(string name, int unselected,int selected)
		{
			var node = new TreeNode(name,unselected,selected);			
			this.Nodes.Add(node);
			node.Tag=name;
			node.Nodes.Add(String.Empty);
			return node;
		}
	
		public void ShowTree(string Path, bool bArchiveRoot = false, string sRobName = "", bool bSelect = false)
		{
			int num = 5;
			string text = Path;
			if (bArchiveRoot)
			{
				num = 11;
				text = sRobName;
			}
			
			TreeNode treeNode = new TreeNode(text, num, num);
			
			
			if (bArchiveRoot)
			{
				treeNode.Tag = Path;
				this.Nodes.Add(treeNode);
			}
			else
			{
				this.Nodes.Add(treeNode);
			}
			this.FillTreeNode(treeNode, "");
			if (bSelect)
			{
				this.SelectedNode = treeNode;
			}
		}
	
		public void ShowTree(DriveType DriveType)
		{
			DriveInfo[] drives = DriveInfo.GetDrives();
			checked
			{
				for (int i = 0; i < drives.Length; i++)
				{
					var driveInfo = drives[i];
					if (driveInfo.DriveType == DriveType)
					{
						this.ShowTree(driveInfo.Name, false, "", false);
					}
				}
			}
		}
	
		public void FillTreeNode(TreeNode dNode, string sRoot = "")
		{
			checked
			{
				try
				{
					this.Cursor = Cursors.WaitCursor;
					var text = dNode.FullPath;
					if (String.Compare(text,"\\",false)==0)
					{
						text = dNode.ToString();
					}
					else
					{
						if (String.Compare(text.Substring(1,1),":",false)!=0)
						{
							
							sRoot = dNode.Text;
							text = sRoot + text.Substring(text.IndexOf("\\"));
						}
					}
					var directoryInfo = new DirectoryInfo(text);
					var directories = directoryInfo.GetDirectories();
					var comparer = new Comparer();
					Array.Sort(directories, comparer);					
					foreach (DirectoryInfo d in directories)
					{						
						var treeNode = new TreeNode(d.Name, 0, 1);
						treeNode.Tag = dNode.Tag.ToString();
						dNode.Nodes.Add(treeNode);
						treeNode.Nodes.Add("");
					}
					
					var files = Directory.GetFiles(text, FileExplorerControl.Instance.Filter);
					Array.Sort<string>(files);
					var array2 = files;
					foreach (string f in array2)
					{
						var fn = Path.GetFileName(f);
						var treeNode2 = new TreeNode(Path.GetFileName(f));
						treeNode2.Tag = dNode.Tag.ToString();
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
							
						
						dNode.Nodes.Add(treeNode2);
					}
					this.Cursor = Cursors.Default;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				//TODO	MainWindow.Instance.Output.Add("FileExplorerClass",ex.ToString());
					this.Cursor = Cursors.Default;
				}
			}
		}
	
		protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
		{
			var node = e.Node;
			this.BeginUpdate();
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
			return string.Compare(name, name2);
		}
	}
	public class FileSelectedEventArgs:EventArgs
	{
		public string Filename{get;set;}
	}
}
