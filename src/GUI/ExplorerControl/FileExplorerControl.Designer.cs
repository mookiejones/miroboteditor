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
			this.panel1 = new System.Windows.Forms.Panel();
			this.explorer = new miRobotEditor.GUI.ExplorerControl.ExplorerClass();
			this.cmbFilter = new System.Windows.Forms.ComboBox();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.explorer);
			this.panel1.Controls.Add(this.cmbFilter);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(150, 150);
			this.panel1.TabIndex = 1;
			// 
			// explorer
			// 
			this.explorer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.explorer.HideSelection = false;
			this.explorer.Location = new System.Drawing.Point(0, 21);
			this.explorer.Name = "explorer";
			this.explorer.Size = new System.Drawing.Size(150, 129);
			this.explorer.TabIndex = 6;
			// 
			// cmbFilter
			// 
			this.cmbFilter.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbFilter.FormattingEnabled = true;
			this.cmbFilter.Location = new System.Drawing.Point(0, 0);
			this.cmbFilter.Name = "cmbFilter";
			this.cmbFilter.Size = new System.Drawing.Size(150, 21);
			this.cmbFilter.TabIndex = 5;
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
		private miRobotEditor.GUI.ExplorerControl.ExplorerClass explorer;
		private System.Windows.Forms.ComboBox cmbFilter;
		private System.Windows.Forms.Panel panel1;
	}
}
