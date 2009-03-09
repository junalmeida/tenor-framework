namespace TenorTemplate
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.lblStep = new System.Windows.Forms.Label();
            this.grpStep1 = new System.Windows.Forms.GroupBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtCatalog = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtConnection = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbProvider = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.lblVersion = new System.Windows.Forms.Label();
            this.grpStep2 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tree = new System.Windows.Forms.TreeView();
            this.grpStep3 = new System.Windows.Forms.GroupBox();
            this.btnChooseFile = new System.Windows.Forms.Button();
            this.txtTargetFolder = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbLanguage = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.grpStep4 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.listProgress = new System.Windows.Forms.ListBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.grpStep1.SuspendLayout();
            this.grpStep2.SuspendLayout();
            this.grpStep3.SuspendLayout();
            this.grpStep4.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblStep
            // 
            this.lblStep.AutoSize = true;
            this.lblStep.BackColor = System.Drawing.Color.Transparent;
            this.lblStep.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStep.Location = new System.Drawing.Point(176, 9);
            this.lblStep.Name = "lblStep";
            this.lblStep.Size = new System.Drawing.Size(85, 18);
            this.lblStep.TabIndex = 0;
            this.lblStep.Text = "Introduction";
            // 
            // grpStep1
            // 
            this.grpStep1.BackColor = System.Drawing.Color.Transparent;
            this.grpStep1.Controls.Add(this.txtPassword);
            this.grpStep1.Controls.Add(this.txtCatalog);
            this.grpStep1.Controls.Add(this.label6);
            this.grpStep1.Controls.Add(this.label4);
            this.grpStep1.Controls.Add(this.txtConnection);
            this.grpStep1.Controls.Add(this.txtUsername);
            this.grpStep1.Controls.Add(this.label7);
            this.grpStep1.Controls.Add(this.label5);
            this.grpStep1.Controls.Add(this.txtServer);
            this.grpStep1.Controls.Add(this.label3);
            this.grpStep1.Controls.Add(this.label2);
            this.grpStep1.Controls.Add(this.cmbProvider);
            this.grpStep1.Controls.Add(this.label1);
            this.grpStep1.Location = new System.Drawing.Point(179, 30);
            this.grpStep1.Name = "grpStep1";
            this.grpStep1.Size = new System.Drawing.Size(401, 348);
            this.grpStep1.TabIndex = 1;
            this.grpStep1.TabStop = false;
            this.grpStep1.Tag = "Introduction";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(222, 221);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(155, 20);
            this.txtPassword.TabIndex = 9;
            this.txtPassword.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // txtCatalog
            // 
            this.txtCatalog.Location = new System.Drawing.Point(222, 171);
            this.txtCatalog.Name = "txtCatalog";
            this.txtCatalog.Size = new System.Drawing.Size(155, 20);
            this.txtCatalog.TabIndex = 5;
            this.txtCatalog.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(219, 205);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "P&assword:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(219, 155);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "&Catalog:";
            // 
            // txtConnection
            // 
            this.txtConnection.Location = new System.Drawing.Point(21, 272);
            this.txtConnection.Multiline = true;
            this.txtConnection.Name = "txtConnection";
            this.txtConnection.Size = new System.Drawing.Size(356, 56);
            this.txtConnection.TabIndex = 11;
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(21, 221);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(155, 20);
            this.txtUsername.TabIndex = 7;
            this.txtUsername.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 256);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(94, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "C&onnection String:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 205);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "&Username:";
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(21, 171);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(155, 20);
            this.txtServer.TabIndex = 3;
            this.txtServer.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 155);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "&Server:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 105);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "&Provider:";
            // 
            // cmbProvider
            // 
            this.cmbProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProvider.FormattingEnabled = true;
            this.cmbProvider.Location = new System.Drawing.Point(21, 121);
            this.cmbProvider.Name = "cmbProvider";
            this.cmbProvider.Size = new System.Drawing.Size(356, 21);
            this.cmbProvider.TabIndex = 1;
            this.cmbProvider.SelectedIndexChanged += new System.EventHandler(this.cmbProvider_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(18, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(377, 61);
            this.label1.TabIndex = 0;
            this.label1.Text = "The Tenor Template will let you create entity classes based on your database mode" +
                "l.\r\n\r\nFill in your database settings below.";
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(505, 384);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 12;
            this.btnNext.Text = "&Next >";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnBack
            // 
            this.btnBack.Enabled = false;
            this.btnBack.Location = new System.Drawing.Point(424, 384);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 23);
            this.btnBack.TabIndex = 13;
            this.btnBack.Text = "< &Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblVersion.Location = new System.Drawing.Point(12, 382);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(82, 26);
            this.lblVersion.TabIndex = 4;
            this.lblVersion.Text = "Tenor Template\r\nVersion {0}";
            // 
            // grpStep2
            // 
            this.grpStep2.BackColor = System.Drawing.Color.Transparent;
            this.grpStep2.Controls.Add(this.label8);
            this.grpStep2.Controls.Add(this.tree);
            this.grpStep2.Location = new System.Drawing.Point(179, 24);
            this.grpStep2.Name = "grpStep2";
            this.grpStep2.Size = new System.Drawing.Size(401, 348);
            this.grpStep2.TabIndex = 5;
            this.grpStep2.TabStop = false;
            this.grpStep2.Tag = "Step 2 of 4 - Choose Tables and/or Views";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(18, 26);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(343, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "At this step you can choose database objects to generate your classes.";
            // 
            // tree
            // 
            this.tree.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tree.CheckBoxes = true;
            this.tree.Location = new System.Drawing.Point(21, 45);
            this.tree.Name = "tree";
            this.tree.Size = new System.Drawing.Size(356, 283);
            this.tree.TabIndex = 0;
            this.tree.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tree_AfterCheck);
            // 
            // grpStep3
            // 
            this.grpStep3.BackColor = System.Drawing.Color.Transparent;
            this.grpStep3.Controls.Add(this.btnChooseFile);
            this.grpStep3.Controls.Add(this.txtTargetFolder);
            this.grpStep3.Controls.Add(this.label11);
            this.grpStep3.Controls.Add(this.cmbLanguage);
            this.grpStep3.Controls.Add(this.label10);
            this.grpStep3.Controls.Add(this.label9);
            this.grpStep3.Location = new System.Drawing.Point(200, 17);
            this.grpStep3.Name = "grpStep3";
            this.grpStep3.Size = new System.Drawing.Size(390, 341);
            this.grpStep3.TabIndex = 15;
            this.grpStep3.TabStop = false;
            this.grpStep3.Tag = "Step 3 of 4 - Choose language and target";
            // 
            // btnChooseFile
            // 
            this.btnChooseFile.Image = global::TenorTemplate.Properties.Resources.openfolderHS;
            this.btnChooseFile.Location = new System.Drawing.Point(325, 132);
            this.btnChooseFile.Name = "btnChooseFile";
            this.btnChooseFile.Size = new System.Drawing.Size(31, 23);
            this.btnChooseFile.TabIndex = 5;
            this.btnChooseFile.UseVisualStyleBackColor = true;
            this.btnChooseFile.Click += new System.EventHandler(this.btnChooseFile_Click);
            // 
            // txtTargetFolder
            // 
            this.txtTargetFolder.Location = new System.Drawing.Point(23, 134);
            this.txtTargetFolder.Name = "txtTargetFolder";
            this.txtTargetFolder.ReadOnly = true;
            this.txtTargetFolder.Size = new System.Drawing.Size(296, 20);
            this.txtTargetFolder.TabIndex = 4;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(20, 118);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(70, 13);
            this.label11.TabIndex = 3;
            this.label11.Text = "&Target folder:";
            // 
            // cmbLanguage
            // 
            this.cmbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLanguage.FormattingEnabled = true;
            this.cmbLanguage.Items.AddRange(new object[] {
            "C#",
            "VB .Net"});
            this.cmbLanguage.Location = new System.Drawing.Point(23, 76);
            this.cmbLanguage.Name = "cmbLanguage";
            this.cmbLanguage.Size = new System.Drawing.Size(333, 21);
            this.cmbLanguage.TabIndex = 2;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(20, 60);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(58, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "&Language:";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(20, 23);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(364, 37);
            this.label9.TabIndex = 0;
            this.label9.Text = "Please choose the desired language and the target folder to generate your classes" +
                ".";
            // 
            // grpStep4
            // 
            this.grpStep4.BackColor = System.Drawing.Color.Transparent;
            this.grpStep4.Controls.Add(this.progressBar);
            this.grpStep4.Controls.Add(this.listProgress);
            this.grpStep4.Controls.Add(this.label12);
            this.grpStep4.Location = new System.Drawing.Point(142, 24);
            this.grpStep4.Name = "grpStep4";
            this.grpStep4.Size = new System.Drawing.Size(398, 342);
            this.grpStep4.TabIndex = 16;
            this.grpStep4.TabStop = false;
            this.grpStep4.Tag = "Step 4 of 4 - Let\'s generate";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(11, 18);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(231, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "Please wait while your code is being generated.";
            // 
            // listProgress
            // 
            this.listProgress.FormattingEnabled = true;
            this.listProgress.Location = new System.Drawing.Point(16, 52);
            this.listProgress.Name = "listProgress";
            this.listProgress.Size = new System.Drawing.Size(368, 238);
            this.listProgress.TabIndex = 1;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(16, 305);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(368, 23);
            this.progressBar.TabIndex = 2;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.BackgroundImage = global::TenorTemplate.Properties.Resources.tenortemplate1;
            this.ClientSize = new System.Drawing.Size(592, 419);
            this.Controls.Add(this.grpStep4);
            this.Controls.Add(this.grpStep3);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.lblStep);
            this.Controls.Add(this.grpStep2);
            this.Controls.Add(this.grpStep1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.Text = "Tenor Template";
            this.grpStep1.ResumeLayout(false);
            this.grpStep1.PerformLayout();
            this.grpStep2.ResumeLayout(false);
            this.grpStep2.PerformLayout();
            this.grpStep3.ResumeLayout(false);
            this.grpStep3.PerformLayout();
            this.grpStep4.ResumeLayout(false);
            this.grpStep4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStep;
        private System.Windows.Forms.GroupBox grpStep1;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbProvider;
        private System.Windows.Forms.TextBox txtCatalog;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtConnection;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox grpStep2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TreeView tree;
        private System.Windows.Forms.GroupBox grpStep3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnChooseFile;
        private System.Windows.Forms.TextBox txtTargetFolder;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbLanguage;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.FolderBrowserDialog folderBrowser;
        private System.Windows.Forms.GroupBox grpStep4;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.ListBox listProgress;

    }
}

