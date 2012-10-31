/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 10/30/2012
 * Time: 12:37
 * 
 */
namespace miRobotEditor.Controls.ExplorerControl
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
		private void InitializeComponent()
		{
			this.panel1 = new System.Windows.Forms.Panel();
			this.cmbFilter = new System.Windows.Forms.ComboBox();
			this.explorerClass1 = new miRobotEditor.Controls.ExplorerControl.ExplorerClass();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.cmbFilter);
			this.panel1.Controls.Add(this.explorerClass1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(150, 150);
			this.panel1.TabIndex = 1;
			// 
			// cmbFilter
			// 
			this.cmbFilter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cmbFilter.FormattingEnabled = true;
			this.cmbFilter.Location = new System.Drawing.Point(0, 0);
			this.cmbFilter.Name = "cmbFilter";
			this.cmbFilter.Size = new System.Drawing.Size(150, 21);
			this.cmbFilter.TabIndex = 5;
			// 
			// explorerClass1
			// 
			this.explorerClass1.HideSelection = false;
			this.explorerClass1.Location = new System.Drawing.Point(0, 27);
			this.explorerClass1.Name = "explorerClass1";
			this.explorerClass1.Size = new System.Drawing.Size(150, 123);
			this.explorerClass1.TabIndex = 1;
			this.explorerClass1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.RaiseAfterSelect);
			this.explorerClass1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.RaiseMouseClick);
			this.explorerClass1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.RaiseDoubleClick);
			this.explorerClass1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.RaiseKeyUp);
			// 
			// FileExplorerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Name = "FileExplorerControl";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private miRobotEditor.Controls.ExplorerControl.ExplorerClass explorerClass1;
		private System.Windows.Forms.ComboBox cmbFilter;
		private System.Windows.Forms.Panel panel1;
	}
}
