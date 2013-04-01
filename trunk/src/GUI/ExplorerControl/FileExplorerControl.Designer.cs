/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 10/30/2012
 * Time: 12:37
 * 
 */
namespace miRobotEditor.GUI.ExplorerControl
{
	partial class FileExplorerControl
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the control.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		[System.Diagnostics.DebuggerStepThrough]
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileExplorerControl));
            this.ctxFileExplorer = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCut = new System.Windows.Forms.ToolStripMenuItem();
            this.explorer= new ExplorerClass();
            this.mnuPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.ctxFileExplorer.SuspendLayout();
            this.SuspendLayout();
            // 
            // ctxFileExplorer
            // 
            this.ctxFileExplorer.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCopy,
            this.mnuCut,
            this.mnuPaste,
            this.mnuDelete,
            this.mnuRefresh});
            this.ctxFileExplorer.Name = "contextMenuStrip1";
            this.ctxFileExplorer.Size = new System.Drawing.Size(114, 114);
            this.ctxFileExplorer.Opening += new System.ComponentModel.CancelEventHandler(this.ContextOpening);
            // 
            // mnuCopy
            // 
            this.mnuCopy.Image = ((System.Drawing.Image)(resources.GetObject("mnuCopy.Image")));
            this.mnuCopy.Name = "mnuCopy";
            this.mnuCopy.Size = new System.Drawing.Size(113, 22);
            this.mnuCopy.Text = "&Copy";
            this.mnuCopy.Click += new System.EventHandler(this.CopyFile);
            // 
            // mnuCut
            // 
            this.mnuCut.Image = ((System.Drawing.Image)(resources.GetObject("mnuCut.Image")));
            this.mnuCut.Name = "mnuCut";
            this.mnuCut.Size = new System.Drawing.Size(113, 22);
            this.mnuCut.Text = "&Cut";
            this.mnuCut.Click += new System.EventHandler(this.CutFile);
            // 
            // mnuPaste
            // 
            this.mnuPaste.Image = ((System.Drawing.Image)(resources.GetObject("mnuPaste.Image")));
            this.mnuPaste.Name = "mnuPaste";
            this.mnuPaste.Size = new System.Drawing.Size(113, 22);
            this.mnuPaste.Text = "&Paste";
            this.mnuPaste.Click += new System.EventHandler(this.PasteFile);
            // 
            // mnuDelete
            // 
            this.mnuDelete.Image = ((System.Drawing.Image)(resources.GetObject("mnuDelete.Image")));
            this.mnuDelete.Name = "mnuDelete";
            this.mnuDelete.Size = new System.Drawing.Size(113, 22);
            this.mnuDelete.Text = "&Delete";
            this.mnuDelete.Click += new System.EventHandler(this.DeleteFile);
            // 
            // mnuRefresh
            // 
            this.mnuRefresh.Name = "mnuRefresh";
            this.mnuRefresh.Size = new System.Drawing.Size(113, 22);
            this.mnuRefresh.Text = "Refresh";
            this.mnuRefresh.Click += new System.EventHandler(this.Refresh);
            // 
            // imgList
            // 
            this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            this.imgList.Images.SetKeyName(0, "folder.png");
            this.imgList.Images.SetKeyName(1, "folderOpen.png");
            this.imgList.Images.SetKeyName(2, "drive.png");
            this.imgList.Images.SetKeyName(3, "cd.png");
            this.imgList.Images.SetKeyName(4, "desktop.png");
            this.imgList.Images.SetKeyName(5, "pendrive.png");
            this.imgList.Images.SetKeyName(6, "file.png");
            // 
            // explorer
            // 
            this.explorer.ContextMenuStrip = this.ctxFileExplorer;
            this.explorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.explorer.HideSelection = false;
            this.explorer.ImageIndex = 0;
            this.explorer.ImageList = this.imgList;
            this.explorer.Location = new System.Drawing.Point(0, 0);
            this.explorer.Name = "explorer";
            this.explorer.SelectedDirectory = null;
            this.explorer.SelectedFile = null;
            this.explorer.SelectedImageIndex = 0;
            this.explorer.Size = new System.Drawing.Size(150, 150);
            this.explorer.TabIndex = 7;
            this.explorer.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.RaiseAfterSelect);
            this.explorer.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.RaiseMouseClick);
            this.explorer.KeyUp += new System.Windows.Forms.KeyEventHandler(this.RaiseKeyUp);
            // 
            // FileExplorerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.explorer);
            this.Name = "FileExplorerControl";
            this.ctxFileExplorer.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		private System.Windows.Forms.ToolStripMenuItem mnuRefresh;
		private System.Windows.Forms.ImageList imgList;
		private System.Windows.Forms.ToolStripMenuItem mnuDelete;
		private System.Windows.Forms.ToolStripMenuItem mnuPaste;
		private System.Windows.Forms.ToolStripMenuItem mnuCut;
		private System.Windows.Forms.ToolStripMenuItem mnuCopy;
		private System.Windows.Forms.ContextMenuStrip ctxFileExplorer;
		private miRobotEditor.GUI.ExplorerControl.ExplorerClass explorer = new ExplorerClass();
	}
}
