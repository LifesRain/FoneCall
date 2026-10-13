namespace FoneCall
{
    partial class RunOptMngr
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RunOptMngr));
            this.DGV = new System.Windows.Forms.DataGridView();
            this.rowNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Role = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Arguments = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Explain = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SaveBTN = new System.Windows.Forms.Button();
            this.CloseBTN = new System.Windows.Forms.Button();
            this.TextPNL = new System.Windows.Forms.Panel();
            this.ReleasesLINK = new System.Windows.Forms.LinkLabel();
            this.InstructLBL = new System.Windows.Forms.Label();
            this.ArgHdrLBL = new System.Windows.Forms.Label();
            this.DGV_PNL = new System.Windows.Forms.Panel();
            this.LoadingLBL = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).BeginInit();
            this.TextPNL.SuspendLayout();
            this.DGV_PNL.SuspendLayout();
            this.SuspendLayout();
            // 
            // DGV
            // 
            this.DGV.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.DGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.rowNum,
            this.Role,
            this.Arguments,
            this.Explain});
            this.DGV.Location = new System.Drawing.Point(12, 3);
            this.DGV.Name = "DGV";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            this.DGV.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.DGV.RowTemplate.Height = 18;
            this.DGV.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.DGV.Size = new System.Drawing.Size(450, 150);
            this.DGV.TabIndex = 2;
            // 
            // rowNum
            // 
            this.rowNum.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.rowNum.HeaderText = "";
            this.rowNum.Name = "rowNum";
            this.rowNum.ReadOnly = true;
            this.rowNum.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.rowNum.Width = 30;
            // 
            // Role
            // 
            this.Role.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Role.HeaderText = "Role";
            this.Role.Name = "Role";
            this.Role.ReadOnly = true;
            this.Role.Width = 54;
            // 
            // Arguments
            // 
            this.Arguments.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Arguments.HeaderText = "Arguments";
            this.Arguments.MinimumWidth = 100;
            this.Arguments.Name = "Arguments";
            // 
            // Explain
            // 
            this.Explain.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Explain.HeaderText = "Description";
            this.Explain.MinimumWidth = 250;
            this.Explain.Name = "Explain";
            this.Explain.Width = 250;
            // 
            // SaveBTN
            // 
            this.SaveBTN.AutoSize = true;
            this.SaveBTN.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SaveBTN.BackColor = System.Drawing.Color.WhiteSmoke;
            this.SaveBTN.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SaveBTN.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.SaveBTN.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveBTN.Location = new System.Drawing.Point(25, 171);
            this.SaveBTN.Name = "SaveBTN";
            this.SaveBTN.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.SaveBTN.Size = new System.Drawing.Size(41, 25);
            this.SaveBTN.TabIndex = 4;
            this.SaveBTN.Text = "Save";
            this.SaveBTN.UseVisualStyleBackColor = false;
            // 
            // CloseBTN
            // 
            this.CloseBTN.AutoSize = true;
            this.CloseBTN.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CloseBTN.BackColor = System.Drawing.Color.WhiteSmoke;
            this.CloseBTN.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CloseBTN.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.CloseBTN.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CloseBTN.Location = new System.Drawing.Point(409, 171);
            this.CloseBTN.Name = "CloseBTN";
            this.CloseBTN.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.CloseBTN.Size = new System.Drawing.Size(53, 25);
            this.CloseBTN.TabIndex = 5;
            this.CloseBTN.Text = "Cancel";
            this.CloseBTN.UseVisualStyleBackColor = false;
            // 
            // TextPNL
            // 
            this.TextPNL.AutoSize = true;
            this.TextPNL.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TextPNL.Controls.Add(this.ReleasesLINK);
            this.TextPNL.Controls.Add(this.InstructLBL);
            this.TextPNL.Controls.Add(this.ArgHdrLBL);
            this.TextPNL.Location = new System.Drawing.Point(22, 18);
            this.TextPNL.MaximumSize = new System.Drawing.Size(720, 0);
            this.TextPNL.Name = "TextPNL";
            this.TextPNL.Size = new System.Drawing.Size(448, 117);
            this.TextPNL.TabIndex = 6;
            // 
            // ReleasesLINK
            // 
            this.ReleasesLINK.AutoSize = true;
            this.ReleasesLINK.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReleasesLINK.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.ReleasesLINK.LinkArea = new System.Windows.Forms.LinkArea(33, 14);
            this.ReleasesLINK.LinkColor = System.Drawing.Color.WhiteSmoke;
            this.ReleasesLINK.Location = new System.Drawing.Point(65, 96);
            this.ReleasesLINK.Name = "ReleasesLINK";
            this.ReleasesLINK.Size = new System.Drawing.Size(316, 21);
            this.ReleasesLINK.TabIndex = 6;
            this.ReleasesLINK.TabStop = true;
            this.ReleasesLINK.Text = "To find new Arguments conult the scrcpy Releaes page.\r\n";
            this.ReleasesLINK.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ReleasesLINK.UseCompatibleTextRendering = true;
            this.ReleasesLINK.VisitedLinkColor = System.Drawing.Color.WhiteSmoke;
            // 
            // InstructLBL
            // 
            this.InstructLBL.AutoSize = true;
            this.InstructLBL.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InstructLBL.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.InstructLBL.Location = new System.Drawing.Point(20, 51);
            this.InstructLBL.MaximumSize = new System.Drawing.Size(443, 0);
            this.InstructLBL.Name = "InstructLBL";
            this.InstructLBL.Size = new System.Drawing.Size(406, 30);
            this.InstructLBL.TabIndex = 5;
            this.InstructLBL.Text = "To add Argguments scroll to the bottom of the listing and paste or type the addit" +
    "ional Arguments starting in the firt empty row";
            // 
            // ArgHdrLBL
            // 
            this.ArgHdrLBL.AutoSize = true;
            this.ArgHdrLBL.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ArgHdrLBL.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.ArgHdrLBL.Location = new System.Drawing.Point(2, -3);
            this.ArgHdrLBL.MaximumSize = new System.Drawing.Size(716, 0);
            this.ArgHdrLBL.Name = "ArgHdrLBL";
            this.ArgHdrLBL.Size = new System.Drawing.Size(443, 60);
            this.ArgHdrLBL.TabIndex = 4;
            this.ArgHdrLBL.Text = resources.GetString("ArgHdrLBL.Text");
            // 
            // DGV_PNL
            // 
            this.DGV_PNL.AutoSize = true;
            this.DGV_PNL.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.DGV_PNL.Controls.Add(this.DGV);
            this.DGV_PNL.Controls.Add(this.SaveBTN);
            this.DGV_PNL.Controls.Add(this.CloseBTN);
            this.DGV_PNL.Location = new System.Drawing.Point(22, 173);
            this.DGV_PNL.Name = "DGV_PNL";
            this.DGV_PNL.Size = new System.Drawing.Size(465, 199);
            this.DGV_PNL.TabIndex = 7;
            this.DGV_PNL.Visible = false;
            // 
            // LoadingLBL
            // 
            this.LoadingLBL.AutoSize = true;
            this.LoadingLBL.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoadingLBL.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.LoadingLBL.Location = new System.Drawing.Point(289, 245);
            this.LoadingLBL.Name = "LoadingLBL";
            this.LoadingLBL.Padding = new System.Windows.Forms.Padding(5);
            this.LoadingLBL.Size = new System.Drawing.Size(229, 35);
            this.LoadingLBL.TabIndex = 8;
            this.LoadingLBL.Text = "LOADING  ARGUMENTS";
            // 
            // RunOptMngr
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(87)))), ((int)(((byte)(79)))));
            this.ClientSize = new System.Drawing.Size(807, 450);
            this.Controls.Add(this.LoadingLBL);
            this.Controls.Add(this.DGV_PNL);
            this.Controls.Add(this.TextPNL);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RunOptMngr";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RunOptMngr";
            ((System.ComponentModel.ISupportInitialize)(this.DGV)).EndInit();
            this.TextPNL.ResumeLayout(false);
            this.TextPNL.PerformLayout();
            this.DGV_PNL.ResumeLayout(false);
            this.DGV_PNL.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView DGV;
        private System.Windows.Forms.Button SaveBTN;
        private System.Windows.Forms.Button CloseBTN;
        private System.Windows.Forms.Panel TextPNL;
        private System.Windows.Forms.LinkLabel ReleasesLINK;
        private System.Windows.Forms.Label InstructLBL;
        private System.Windows.Forms.Label ArgHdrLBL;
        private System.Windows.Forms.Panel DGV_PNL;
        private System.Windows.Forms.Label LoadingLBL;
        private System.Windows.Forms.DataGridViewTextBoxColumn rowNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn Role;
        private System.Windows.Forms.DataGridViewTextBoxColumn Arguments;
        private System.Windows.Forms.DataGridViewTextBoxColumn Explain;
    }
}