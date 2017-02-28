namespace ICLRead
{
    partial class BootLoader
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
            this.TransferParametersPanel = new System.Windows.Forms.Panel();
            this.FileSizeLabel = new System.Windows.Forms.Label();
            this.FileSizeTb = new System.Windows.Forms.TextBox();
            this.FilePathTb = new System.Windows.Forms.TextBox();
            this.CloseFileBtn = new System.Windows.Forms.Button();
            this.OpenFileBtn = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.JumpBtLdrBtn = new System.Windows.Forms.Button();
            this.ExtJumpBtLdrBtn = new System.Windows.Forms.Button();
            this.NPTModeBtn = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ModuleStatusTb = new System.Windows.Forms.TextBox();
            this.ModeTb = new System.Windows.Forms.TextBox();
            this.DIDTb = new System.Windows.Forms.TextBox();
            this.SIDTb = new System.Windows.Forms.TextBox();
            this.ModuleStrSelectBox = new System.Windows.Forms.DomainUpDown();
            this.panel3 = new System.Windows.Forms.Panel();
            this.CloseBtn = new System.Windows.Forms.Button();
            this.StartDownloadBtn = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.TransferParametersPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // TransferParametersPanel
            // 
            this.TransferParametersPanel.BackColor = System.Drawing.Color.White;
            this.TransferParametersPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TransferParametersPanel.Controls.Add(this.FileSizeLabel);
            this.TransferParametersPanel.Controls.Add(this.FileSizeTb);
            this.TransferParametersPanel.Controls.Add(this.FilePathTb);
            this.TransferParametersPanel.Controls.Add(this.CloseFileBtn);
            this.TransferParametersPanel.Controls.Add(this.OpenFileBtn);
            this.TransferParametersPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.TransferParametersPanel.Location = new System.Drawing.Point(10, 11);
            this.TransferParametersPanel.Margin = new System.Windows.Forms.Padding(2);
            this.TransferParametersPanel.Name = "TransferParametersPanel";
            this.TransferParametersPanel.Size = new System.Drawing.Size(478, 69);
            this.TransferParametersPanel.TabIndex = 0;
            this.TransferParametersPanel.Tag = "";
            // 
            // FileSizeLabel
            // 
            this.FileSizeLabel.AutoSize = true;
            this.FileSizeLabel.Location = new System.Drawing.Point(320, 41);
            this.FileSizeLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.FileSizeLabel.Name = "FileSizeLabel";
            this.FileSizeLabel.Size = new System.Drawing.Size(49, 13);
            this.FileSizeLabel.TabIndex = 19;
            this.FileSizeLabel.Text = "File Size:";
            // 
            // FileSizeTb
            // 
            this.FileSizeTb.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.FileSizeTb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FileSizeTb.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.FileSizeTb.Location = new System.Drawing.Point(374, 39);
            this.FileSizeTb.Margin = new System.Windows.Forms.Padding(2);
            this.FileSizeTb.Name = "FileSizeTb";
            this.FileSizeTb.ReadOnly = true;
            this.FileSizeTb.Size = new System.Drawing.Size(76, 20);
            this.FileSizeTb.TabIndex = 18;
            // 
            // FilePathTb
            // 
            this.FilePathTb.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.FilePathTb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FilePathTb.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.FilePathTb.Location = new System.Drawing.Point(104, 11);
            this.FilePathTb.Margin = new System.Windows.Forms.Padding(2);
            this.FilePathTb.Name = "FilePathTb";
            this.FilePathTb.ReadOnly = true;
            this.FilePathTb.Size = new System.Drawing.Size(346, 20);
            this.FilePathTb.TabIndex = 17;
            // 
            // CloseFileBtn
            // 
            this.CloseFileBtn.AutoSize = true;
            this.CloseFileBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(223)))), ((int)(((byte)(255)))));
            this.CloseFileBtn.Enabled = false;
            this.CloseFileBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CloseFileBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.CloseFileBtn.Location = new System.Drawing.Point(20, 36);
            this.CloseFileBtn.Margin = new System.Windows.Forms.Padding(2);
            this.CloseFileBtn.Name = "CloseFileBtn";
            this.CloseFileBtn.Size = new System.Drawing.Size(80, 25);
            this.CloseFileBtn.TabIndex = 14;
            this.CloseFileBtn.Text = "Close File";
            this.CloseFileBtn.UseVisualStyleBackColor = false;
            this.CloseFileBtn.Click += new System.EventHandler(this.CloseFileBtn_Click);
            // 
            // OpenFileBtn
            // 
            this.OpenFileBtn.AutoSize = true;
            this.OpenFileBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(223)))), ((int)(((byte)(255)))));
            this.OpenFileBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OpenFileBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.OpenFileBtn.Location = new System.Drawing.Point(20, 7);
            this.OpenFileBtn.Margin = new System.Windows.Forms.Padding(2);
            this.OpenFileBtn.Name = "OpenFileBtn";
            this.OpenFileBtn.Size = new System.Drawing.Size(80, 25);
            this.OpenFileBtn.TabIndex = 13;
            this.OpenFileBtn.Text = "Open File";
            this.OpenFileBtn.UseVisualStyleBackColor = false;
            this.OpenFileBtn.Click += new System.EventHandler(this.OpenFileBtn_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.JumpBtLdrBtn);
            this.panel2.Controls.Add(this.ExtJumpBtLdrBtn);
            this.panel2.Controls.Add(this.NPTModeBtn);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.ModuleStatusTb);
            this.panel2.Controls.Add(this.ModeTb);
            this.panel2.Controls.Add(this.DIDTb);
            this.panel2.Controls.Add(this.SIDTb);
            this.panel2.Controls.Add(this.ModuleStrSelectBox);
            this.panel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel2.Location = new System.Drawing.Point(10, 84);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(478, 52);
            this.panel2.TabIndex = 1;
            // 
            // JumpBtLdrBtn
            // 
            this.JumpBtLdrBtn.AutoSize = true;
            this.JumpBtLdrBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(223)))), ((int)(((byte)(255)))));
            this.JumpBtLdrBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.JumpBtLdrBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.JumpBtLdrBtn.Location = new System.Drawing.Point(344, 15);
            this.JumpBtLdrBtn.Margin = new System.Windows.Forms.Padding(2);
            this.JumpBtLdrBtn.Name = "JumpBtLdrBtn";
            this.JumpBtLdrBtn.Size = new System.Drawing.Size(44, 25);
            this.JumpBtLdrBtn.TabIndex = 29;
            this.JumpBtLdrBtn.Text = "Jump";
            this.JumpBtLdrBtn.UseVisualStyleBackColor = false;
            this.JumpBtLdrBtn.Click += new System.EventHandler(this.JumpBtLdrBtn_Click);
            // 
            // ExtJumpBtLdrBtn
            // 
            this.ExtJumpBtLdrBtn.AutoSize = true;
            this.ExtJumpBtLdrBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(223)))), ((int)(((byte)(255)))));
            this.ExtJumpBtLdrBtn.Enabled = false;
            this.ExtJumpBtLdrBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ExtJumpBtLdrBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ExtJumpBtLdrBtn.Location = new System.Drawing.Point(382, 15);
            this.ExtJumpBtLdrBtn.Margin = new System.Windows.Forms.Padding(2);
            this.ExtJumpBtLdrBtn.Name = "ExtJumpBtLdrBtn";
            this.ExtJumpBtLdrBtn.Size = new System.Drawing.Size(34, 25);
            this.ExtJumpBtLdrBtn.TabIndex = 30;
            this.ExtJumpBtLdrBtn.Text = "Ext";
            this.ExtJumpBtLdrBtn.UseVisualStyleBackColor = false;
            // 
            // NPTModeBtn
            // 
            this.NPTModeBtn.AutoSize = true;
            this.NPTModeBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(223)))), ((int)(((byte)(255)))));
            this.NPTModeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.NPTModeBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.NPTModeBtn.Location = new System.Drawing.Point(304, 15);
            this.NPTModeBtn.Margin = new System.Windows.Forms.Padding(2);
            this.NPTModeBtn.Name = "NPTModeBtn";
            this.NPTModeBtn.Size = new System.Drawing.Size(41, 25);
            this.NPTModeBtn.TabIndex = 28;
            this.NPTModeBtn.Text = "NPT";
            this.NPTModeBtn.UseVisualStyleBackColor = false;
            this.NPTModeBtn.Click += new System.EventHandler(this.NPTModeBtn_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(222, 5);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 13);
            this.label5.TabIndex = 27;
            this.label5.Text = "Status:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(178, 5);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 26;
            this.label4.Text = "Mode:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(140, 5);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = "DID:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(101, 5);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "SID:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Module:";
            // 
            // ModuleStatusTb
            // 
            this.ModuleStatusTb.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ModuleStatusTb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ModuleStatusTb.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ModuleStatusTb.Location = new System.Drawing.Point(224, 21);
            this.ModuleStatusTb.Margin = new System.Windows.Forms.Padding(2);
            this.ModuleStatusTb.Name = "ModuleStatusTb";
            this.ModuleStatusTb.Size = new System.Drawing.Size(76, 20);
            this.ModuleStatusTb.TabIndex = 22;
            // 
            // ModeTb
            // 
            this.ModeTb.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ModeTb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ModeTb.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ModeTb.Location = new System.Drawing.Point(180, 21);
            this.ModeTb.Margin = new System.Windows.Forms.Padding(2);
            this.ModeTb.Name = "ModeTb";
            this.ModeTb.Size = new System.Drawing.Size(40, 20);
            this.ModeTb.TabIndex = 21;
            // 
            // DIDTb
            // 
            this.DIDTb.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.DIDTb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DIDTb.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.DIDTb.Location = new System.Drawing.Point(142, 21);
            this.DIDTb.Margin = new System.Windows.Forms.Padding(2);
            this.DIDTb.Name = "DIDTb";
            this.DIDTb.Size = new System.Drawing.Size(34, 20);
            this.DIDTb.TabIndex = 20;
            // 
            // SIDTb
            // 
            this.SIDTb.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.SIDTb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SIDTb.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.SIDTb.Location = new System.Drawing.Point(104, 21);
            this.SIDTb.Margin = new System.Windows.Forms.Padding(2);
            this.SIDTb.Name = "SIDTb";
            this.SIDTb.Size = new System.Drawing.Size(34, 20);
            this.SIDTb.TabIndex = 19;
            // 
            // ModuleStrSelectBox
            // 
            this.ModuleStrSelectBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ModuleStrSelectBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ModuleStrSelectBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ModuleStrSelectBox.Items.Add("AWS");
            this.ModuleStrSelectBox.Items.Add("DET");
            this.ModuleStrSelectBox.Items.Add("GCB");
            this.ModuleStrSelectBox.Items.Add("GEN");
            this.ModuleStrSelectBox.Items.Add("THD");
            this.ModuleStrSelectBox.Items.Add("VTA");
            this.ModuleStrSelectBox.Items.Add("BKY");
            this.ModuleStrSelectBox.Items.Add("CARM");
            this.ModuleStrSelectBox.Items.Add("BGM");
            this.ModuleStrSelectBox.Items.Add("ACC");
            this.ModuleStrSelectBox.Items.Add("EXT");
            this.ModuleStrSelectBox.Items.Add("PMC");
            this.ModuleStrSelectBox.Items.Add("CDI");
            this.ModuleStrSelectBox.Items.Add("BCM");
            this.ModuleStrSelectBox.Items.Add("AIB");
            this.ModuleStrSelectBox.Items.Add("DTC");
            this.ModuleStrSelectBox.Location = new System.Drawing.Point(20, 21);
            this.ModuleStrSelectBox.Margin = new System.Windows.Forms.Padding(2);
            this.ModuleStrSelectBox.Name = "ModuleStrSelectBox";
            this.ModuleStrSelectBox.Size = new System.Drawing.Size(80, 20);
            this.ModuleStrSelectBox.TabIndex = 0;
            this.ModuleStrSelectBox.SelectedItemChanged += new System.EventHandler(this.ModuleStrSelectBox_SelectedItemChanged);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.CloseBtn);
            this.panel3.Controls.Add(this.StartDownloadBtn);
            this.panel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel3.Location = new System.Drawing.Point(10, 141);
            this.panel3.Margin = new System.Windows.Forms.Padding(2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(478, 69);
            this.panel3.TabIndex = 2;
            // 
            // CloseBtn
            // 
            this.CloseBtn.AutoSize = true;
            this.CloseBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(223)))), ((int)(((byte)(255)))));
            this.CloseBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CloseBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.CloseBtn.Location = new System.Drawing.Point(408, 40);
            this.CloseBtn.Margin = new System.Windows.Forms.Padding(2);
            this.CloseBtn.Name = "CloseBtn";
            this.CloseBtn.Size = new System.Drawing.Size(66, 25);
            this.CloseBtn.TabIndex = 31;
            this.CloseBtn.Text = "Close";
            this.CloseBtn.UseVisualStyleBackColor = false;
            this.CloseBtn.Click += new System.EventHandler(this.CloseBtn_Click);
            // 
            // StartDownloadBtn
            // 
            this.StartDownloadBtn.AutoSize = true;
            this.StartDownloadBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(223)))), ((int)(((byte)(255)))));
            this.StartDownloadBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StartDownloadBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.StartDownloadBtn.Location = new System.Drawing.Point(20, 22);
            this.StartDownloadBtn.Margin = new System.Windows.Forms.Padding(2);
            this.StartDownloadBtn.Name = "StartDownloadBtn";
            this.StartDownloadBtn.Size = new System.Drawing.Size(92, 25);
            this.StartDownloadBtn.TabIndex = 30;
            this.StartDownloadBtn.Text = "Start Download";
            this.StartDownloadBtn.UseVisualStyleBackColor = false;
            this.StartDownloadBtn.Click += new System.EventHandler(this.StartDownloadBtn_Click);
            // 
            // BootLoader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(499, 223);
            this.ControlBox = false;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.TransferParametersPanel);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BootLoader";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "BootLoader";
            this.TopMost = true;
            this.TransferParametersPanel.ResumeLayout(false);
            this.TransferParametersPanel.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel TransferParametersPanel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button CloseFileBtn;
        private System.Windows.Forms.Button OpenFileBtn;
        private System.Windows.Forms.Label FileSizeLabel;
        private System.Windows.Forms.TextBox FileSizeTb;
        private System.Windows.Forms.TextBox FilePathTb;
        private System.Windows.Forms.DomainUpDown ModuleStrSelectBox;
        private System.Windows.Forms.TextBox ModuleStatusTb;
        private System.Windows.Forms.TextBox ModeTb;
        private System.Windows.Forms.TextBox DIDTb;
        private System.Windows.Forms.TextBox SIDTb;
        private System.Windows.Forms.Button JumpBtLdrBtn;
        private System.Windows.Forms.Button ExtJumpBtLdrBtn;
        private System.Windows.Forms.Button NPTModeBtn;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button StartDownloadBtn;
        private System.Windows.Forms.Button CloseBtn;
    }
}