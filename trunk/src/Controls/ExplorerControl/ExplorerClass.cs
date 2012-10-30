/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 10/30/2012
 * Time: 12:37
 * 
 */
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Collections;
namespace miRobotEditor.Controls.ExplorerControl
{
	/// <summary>
	/// Description of ExplorerClass.
	/// </summary>
	public class ExplorerClass:TreeView,IComparable
	{
		public int CompareTo(object obj)
		{
			DirectoryInfo directoryInfo = (DirectoryInfo)obj;
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
		public void ShowTree()
		{
			this.Nodes.Clear();
			checked
			{
				try
				{
					DriveInfo[] drives = DriveInfo.GetDrives();
					for (int i = 0; i < drives.Length; i++)
					{
						DriveInfo driveInfo = drives[i];
						switch (driveInfo.DriveType)
						{
						case DriveType.Unknown:
						{
							TreeNode treeNode = new TreeNode(driveInfo.Name, 7, 7);
							this.Nodes.Add(treeNode);
							treeNode.Nodes.Add("");
							break;
						}
						case DriveType.Removable:
								switch (driveInfo.Name)
								{
									case "A:\\":
									case "B:\\":
										TreeNode treeNode2 = new TreeNode(driveInfo.Name, 6, 6);
										this.Nodes.Add(treeNode2);
										treeNode2.Nodes.Add("");
										break;
									default:
										TreeNode treeNode3 = new TreeNode(driveInfo.Name, 9, 9);
										treeNode3.ToolTipText = driveInfo.VolumeLabel;
										this.Nodes.Add(treeNode3);
										treeNode3.Nodes.Add("");
										break;
								}							
							break;
						case DriveType.Fixed:
						{
							TreeNode treeNode4 = new TreeNode(driveInfo.Name, 7, 7);
							treeNode4.ToolTipText = driveInfo.VolumeLabel;
							this.Nodes.Add(treeNode4);
							treeNode4.Nodes.Add("");
							break;
						}
						case DriveType.Network:
						{
							TreeNode treeNode5 = new TreeNode(driveInfo.Name, 10, 10);
							this.Nodes.Add(treeNode5);
							treeNode5.Nodes.Add("");
							break;
						}
						case DriveType.CDRom:
						{
							TreeNode treeNode6 = new TreeNode(driveInfo.Name, 8, 8);
							this.Nodes.Add(treeNode6);
							treeNode6.Nodes.Add("");
							break;
						}
						}
					}
				}
				catch (Exception ex)
				{
					MainWindow.Instance.Output.Add("ExplorerClass",ex.ToString());
				}
			}
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
					DriveInfo driveInfo = drives[i];
					if (driveInfo.DriveType == DriveType)
					{
						this.ShowTree(driveInfo.Name, false, "", false);
					}
				}
			}
		}
		private void FillTreeNode(TreeNode dNode, string sRoot = "")
		{
			checked
			{
				try
				{
					this.Cursor = Cursors.WaitCursor;
					string text = dNode.FullPath;
					if (String.Compare(text,"\\",false)==0)
					{
						text = dNode.ToString();
					}
					else
					{
						if (String.Compare(text.Substring(2,1),":",false)!=0)
						{
							
							sRoot = dNode.Tag.ToString();
							text = sRoot + text.Substring(text.IndexOf("\\"));
						}
					}
					DirectoryInfo directoryInfo = new DirectoryInfo(text);
					DirectoryInfo[] directories = directoryInfo.GetDirectories();
					Comparer comparer = new Comparer();
					Array.Sort(directories, comparer);
					DirectoryInfo[] array = directories;
					for (int i = 0; i < array.Length; i++)
					{
						DirectoryInfo directoryInfo2 = array[i];
						TreeNode treeNode = new TreeNode(directoryInfo2.Name, 5, 5);
						treeNode.Tag = RuntimeHelpers.GetObjectValue(dNode.Tag);
						dNode.Nodes.Add(treeNode);
						treeNode.Nodes.Add("");
					}
					string[] files = Directory.GetFiles(text, FileExplorerControl.Instance.Filter);
					Array.Sort<string>(files);
					string[] array2 = files;
					for (int j = 0; j < array2.Length; j++)
					{
						string path = array2[j];
						TreeNode treeNode2 = new TreeNode(Path.GetFileName(path));
						treeNode2.Tag = RuntimeHelpers.GetObjectValue(dNode.Tag);
						string left = Path.GetExtension(path).ToLower();
						switch (left)
						{
							case ".src":								
							treeNode2.SelectedImageIndex = 1;
							treeNode2.ImageIndex = 1;
							break;
						case ".dat":
							
								treeNode2.SelectedImageIndex = 2;
								treeNode2.ImageIndex = 2;
								break;
							case ".sub":
									treeNode2.SelectedImageIndex = 3;
									treeNode2.ImageIndex = 3;
									break;
								case ".zip":
										treeNode2.SelectedImageIndex = 4;
										treeNode2.ImageIndex = 4;
										break;
									default:
										treeNode2.SelectedImageIndex = 0;
										treeNode2.ImageIndex = 0;
										break;
								}
							
						
						dNode.Nodes.Add(treeNode2);
					}
					this.Cursor = Cursors.Default;
				}
				catch (Exception ex)
				{
					MainWindow.Instance.Output.Add("FileExplorerClass",ex.ToString());
					this.Cursor = Cursors.Default;
				}
			}
		}
		protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
		{
			TreeNode node = e.Node;
			this.BeginUpdate();
			node.Nodes.Clear();
			string sRoot = e.Node.Tag.ToString();
			this.FillTreeNode(node, sRoot);
			this.EndUpdate();
			base.OnBeforeExpand(e);
		}
	}
	class Comparer:IComparer
	{
		public int Compare(object x, object y)
		{
			DirectoryInfo directoryInfo = (DirectoryInfo)x;
			DirectoryInfo directoryInfo2 = (DirectoryInfo)y;
			string name = directoryInfo.Name;
			string name2 = directoryInfo2.Name;
			return string.Compare(name, name2);
		}
	}
}
