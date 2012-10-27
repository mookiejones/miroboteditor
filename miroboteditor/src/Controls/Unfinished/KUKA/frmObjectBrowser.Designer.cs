namespace miRobotEditor.Controls.Unfinished.KUKA
{
    partial class frmObjectBrowser
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
            tabControl1 = new System.Windows.Forms.TabControl();
            tpFunctions = new System.Windows.Forms.TabPage();
            Variables = new System.Windows.Forms.TabPage();
            lv2 = new System.Windows.Forms.ListView();
            cName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            cType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            cPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            cGlobal = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            cInfo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            cBlank = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            tabControl1.SuspendLayout();
            tpFunctions.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tpFunctions);
            tabControl1.Controls.Add(Variables);
            tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl1.Location = new System.Drawing.Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(683, 292);
            tabControl1.TabIndex = 0;
            // 
            // tpFunctions
            // 
            tpFunctions.Controls.Add(lv2);
            tpFunctions.Location = new System.Drawing.Point(4, 22);
            tpFunctions.Name = "tpFunctions";
            tpFunctions.Padding = new System.Windows.Forms.Padding(3);
            tpFunctions.Size = new System.Drawing.Size(675, 266);
            tpFunctions.TabIndex = 0;
            tpFunctions.Text = "Functions";
            tpFunctions.UseVisualStyleBackColor = true;
            // 
            // _variables
            // 
            Variables.Location = new System.Drawing.Point(4, 22);
            Variables.Name = "_variables";
            Variables.Padding = new System.Windows.Forms.Padding(3);
            Variables.Size = new System.Drawing.Size(675, 266);
            Variables.TabIndex = 1;
            Variables.Text = "tpVariables";
            Variables.UseVisualStyleBackColor = true;
            // 
            // lv2
            // 
            lv2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            cName,
            cType,
            cPath,
            cGlobal,
            cInfo,
            cBlank});
            lv2.Dock = System.Windows.Forms.DockStyle.Fill;
            lv2.FullRowSelect = true;
            lv2.GridLines = true;
            lv2.Location = new System.Drawing.Point(3, 3);
            lv2.Name = "lv2";
            lv2.ShowGroups = false;
            lv2.Size = new System.Drawing.Size(669, 260);
            lv2.TabIndex = 0;
            lv2.UseCompatibleStateImageBehavior = false;
            lv2.View = System.Windows.Forms.View.Details;
            // 
            // cName
            // 
            cName.Text = "Name";
            cName.Width = 150;
            // 
            // cType
            // 
            cType.Text = "Type";
            cType.Width = 50;
            // 
            // cPath
            // 
            cPath.Text = "Path";
            cPath.Width = 200;
            // 
            // cGlobal
            // 
            cGlobal.Text = "Global";
            cGlobal.Width = 45;
            // 
            // cInfo
            // 
            cInfo.Text = "Info";
            cInfo.Width = 220;
            // 
            // cBlank
            // 
            cBlank.Text = "";
            cBlank.Width = 0;
            // 
            // frmObjectBrowser
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(683, 292);
            Controls.Add(tabControl1);
            Name = "frmObjectBrowser";
            Text = "frmObjectBrowser";
            tabControl1.ResumeLayout(false);
            tpFunctions.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpFunctions;
        private System.Windows.Forms.TabPage Variables;
        private System.Windows.Forms.ListView lv2;
        private System.Windows.Forms.ColumnHeader cName;
        private System.Windows.Forms.ColumnHeader cType;
        private System.Windows.Forms.ColumnHeader cPath;
        private System.Windows.Forms.ColumnHeader cGlobal;
        private System.Windows.Forms.ColumnHeader cInfo;
        private System.Windows.Forms.ColumnHeader cBlank;
    }
}