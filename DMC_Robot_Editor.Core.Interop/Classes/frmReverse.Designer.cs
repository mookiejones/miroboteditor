namespace DMC_Robot_Editor.Classes
{
    partial class frmReverse
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.layout = new System.Windows.Forms.TableLayoutPanel();
            this.ckOK = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // layout
            // 
            this.layout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.layout.ColumnCount = 2;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layout.Controls.Add(this.btnCancel, 0, 0);
            this.layout.Controls.Add(this.ckOK, 1, 0);
            this.layout.Location = new System.Drawing.Point(200, 38);
            this.layout.Name = "layout";
            this.layout.RowCount = 1;
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.layout.Size = new System.Drawing.Size(146, 29);
            this.layout.TabIndex = 0;
            // 
            // ckOK
            // 
            this.ckOK.AutoSize = true;
            this.ckOK.Location = new System.Drawing.Point(126, 185);
            this.ckOK.Name = "ckOK";
            this.ckOK.Size = new System.Drawing.Size(73, 17);
            this.ckOK.TabIndex = 1;
            this.ckOK.Text = "&Ok button";
            this.ckOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(101, 217);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // frmReverse
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.ckOK);
            this.Controls.Add(this.layout);
            this.Name = "frmReverse";
            this.Text = "frmReverse";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel layout;
        private System.Windows.Forms.CheckBox ckOK;
        private System.Windows.Forms.Button btnCancel;
    }
}