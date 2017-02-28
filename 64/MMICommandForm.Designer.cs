namespace ICLRead
{
    partial class MMICommandForm
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
            this.LoadProfileBtn = new System.Windows.Forms.Button();
            this.SendProfileBtn = new System.Windows.Forms.Button();
            this.ExecuteProfileBtn = new System.Windows.Forms.Button();
            this.UnknownTb = new System.Windows.Forms.TextBox();
            this.ProfileParamsTb = new System.Windows.Forms.TextBox();
            this.CloseBtn = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.AlignMotorBtn = new System.Windows.Forms.Button();
            this.NPTModeBtn = new System.Windows.Forms.Button();
            this.PosZeroBtn = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LoadProfileBtn
            // 
            this.LoadProfileBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(223)))), ((int)(((byte)(255)))));
            this.LoadProfileBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LoadProfileBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.LoadProfileBtn.Location = new System.Drawing.Point(15, 10);
            this.LoadProfileBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.LoadProfileBtn.Name = "LoadProfileBtn";
            this.LoadProfileBtn.Size = new System.Drawing.Size(125, 25);
            this.LoadProfileBtn.TabIndex = 0;
            this.LoadProfileBtn.Text = "LoadProfile";
            this.LoadProfileBtn.UseVisualStyleBackColor = false;
            this.LoadProfileBtn.Click += new System.EventHandler(this.LoadProfileBtn_Click);
            // 
            // SendProfileBtn
            // 
            this.SendProfileBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(223)))), ((int)(((byte)(255)))));
            this.SendProfileBtn.Enabled = false;
            this.SendProfileBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SendProfileBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.SendProfileBtn.Location = new System.Drawing.Point(15, 39);
            this.SendProfileBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SendProfileBtn.Name = "SendProfileBtn";
            this.SendProfileBtn.Size = new System.Drawing.Size(125, 25);
            this.SendProfileBtn.TabIndex = 1;
            this.SendProfileBtn.Text = "Send Profile";
            this.SendProfileBtn.UseVisualStyleBackColor = false;
            this.SendProfileBtn.Click += new System.EventHandler(this.SendProfileBtn_Click);
            // 
            // ExecuteProfileBtn
            // 
            this.ExecuteProfileBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(223)))), ((int)(((byte)(255)))));
            this.ExecuteProfileBtn.Enabled = false;
            this.ExecuteProfileBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ExecuteProfileBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ExecuteProfileBtn.Location = new System.Drawing.Point(15, 68);
            this.ExecuteProfileBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ExecuteProfileBtn.Name = "ExecuteProfileBtn";
            this.ExecuteProfileBtn.Size = new System.Drawing.Size(125, 25);
            this.ExecuteProfileBtn.TabIndex = 2;
            this.ExecuteProfileBtn.Text = "Execute Profile";
            this.ExecuteProfileBtn.UseVisualStyleBackColor = false;
            this.ExecuteProfileBtn.Click += new System.EventHandler(this.ExecuteProfileBtn_Click);
            // 
            // UnknownTb
            // 
            this.UnknownTb.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.UnknownTb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.UnknownTb.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.UnknownTb.Location = new System.Drawing.Point(146, 12);
            this.UnknownTb.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.UnknownTb.Name = "UnknownTb";
            this.UnknownTb.Size = new System.Drawing.Size(175, 22);
            this.UnknownTb.TabIndex = 3;
            // 
            // ProfileParamsTb
            // 
            this.ProfileParamsTb.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.ProfileParamsTb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ProfileParamsTb.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ProfileParamsTb.Location = new System.Drawing.Point(146, 39);
            this.ProfileParamsTb.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ProfileParamsTb.Multiline = true;
            this.ProfileParamsTb.Name = "ProfileParamsTb";
            this.ProfileParamsTb.ReadOnly = true;
            this.ProfileParamsTb.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.ProfileParamsTb.Size = new System.Drawing.Size(423, 141);
            this.ProfileParamsTb.TabIndex = 4;
            this.ProfileParamsTb.Text = "Profile Parameters";
            // 
            // CloseBtn
            // 
            this.CloseBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(223)))), ((int)(((byte)(255)))));
            this.CloseBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CloseBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.CloseBtn.Location = new System.Drawing.Point(494, 184);
            this.CloseBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.CloseBtn.Name = "CloseBtn";
            this.CloseBtn.Size = new System.Drawing.Size(75, 25);
            this.CloseBtn.TabIndex = 6;
            this.CloseBtn.Text = "Close";
            this.CloseBtn.UseVisualStyleBackColor = false;
            this.CloseBtn.Click += new System.EventHandler(this.CloseBtn_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // AlignMotorBtn
            // 
            this.AlignMotorBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(223)))), ((int)(((byte)(255)))));
            this.AlignMotorBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AlignMotorBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.AlignMotorBtn.Location = new System.Drawing.Point(15, 97);
            this.AlignMotorBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.AlignMotorBtn.Name = "AlignMotorBtn";
            this.AlignMotorBtn.Size = new System.Drawing.Size(125, 25);
            this.AlignMotorBtn.TabIndex = 7;
            this.AlignMotorBtn.Text = "Align Motor";
            this.AlignMotorBtn.UseVisualStyleBackColor = false;
            this.AlignMotorBtn.Click += new System.EventHandler(this.AlignMotorBtn_Click);
            // 
            // NPTModeBtn
            // 
            this.NPTModeBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(223)))), ((int)(((byte)(255)))));
            this.NPTModeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.NPTModeBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.NPTModeBtn.Location = new System.Drawing.Point(15, 126);
            this.NPTModeBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.NPTModeBtn.Name = "NPTModeBtn";
            this.NPTModeBtn.Size = new System.Drawing.Size(125, 25);
            this.NPTModeBtn.TabIndex = 8;
            this.NPTModeBtn.Text = "NPT Mode";
            this.NPTModeBtn.UseVisualStyleBackColor = false;
            this.NPTModeBtn.Click += new System.EventHandler(this.NPTModeBtn_Click);
            // 
            // PosZeroBtn
            // 
            this.PosZeroBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(223)))), ((int)(((byte)(255)))));
            this.PosZeroBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PosZeroBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.PosZeroBtn.Location = new System.Drawing.Point(15, 155);
            this.PosZeroBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.PosZeroBtn.Name = "PosZeroBtn";
            this.PosZeroBtn.Size = new System.Drawing.Size(125, 25);
            this.PosZeroBtn.TabIndex = 9;
            this.PosZeroBtn.Text = "Zero Position";
            this.PosZeroBtn.UseVisualStyleBackColor = false;
            this.PosZeroBtn.Click += new System.EventHandler(this.PosZeroBtn_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(223)))), ((int)(((byte)(255)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button1.Location = new System.Drawing.Point(335, 10);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(125, 25);
            this.button1.TabIndex = 10;
            this.button1.Text = "Send Profile";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.LoadProfileBtn);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.SendProfileBtn);
            this.panel1.Controls.Add(this.UnknownTb);
            this.panel1.Controls.Add(this.ProfileParamsTb);
            this.panel1.Controls.Add(this.CloseBtn);
            this.panel1.Controls.Add(this.PosZeroBtn);
            this.panel1.Controls.Add(this.ExecuteProfileBtn);
            this.panel1.Controls.Add(this.NPTModeBtn);
            this.panel1.Controls.Add(this.AlignMotorBtn);
            this.panel1.Location = new System.Drawing.Point(4, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(574, 214);
            this.panel1.TabIndex = 11;
            // 
            // MMICommandForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(580, 219);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MMICommandForm";
            this.Text = "MMI Command";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button LoadProfileBtn;
        private System.Windows.Forms.Button SendProfileBtn;
        private System.Windows.Forms.Button ExecuteProfileBtn;
        private System.Windows.Forms.TextBox UnknownTb;
        private System.Windows.Forms.TextBox ProfileParamsTb;
        private System.Windows.Forms.Button CloseBtn;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button AlignMotorBtn;
        private System.Windows.Forms.Button NPTModeBtn;
        private System.Windows.Forms.Button PosZeroBtn;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel1;
    }
}