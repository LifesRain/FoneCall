namespace FoneCall
{
    partial class Welcome
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Welcome));
            this.VerLBL = new System.Windows.Forms.Label();
            this.WinCtrlPNL = new System.Windows.Forms.Panel();
            this.CloseWinPB = new System.Windows.Forms.PictureBox();
            this.MinWinPB = new System.Windows.Forms.PictureBox();
            this.MoveWinPB = new System.Windows.Forms.PictureBox();
            this.LogLBL = new System.Windows.Forms.Label();
            this.CloseBTN = new System.Windows.Forms.Button();
            this.RTBX = new CertAdmin.RTB__FLB();
            this.WinCtrlPNL.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CloseWinPB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinWinPB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MoveWinPB)).BeginInit();
            this.SuspendLayout();
            // 
            // VerLBL
            // 
            this.VerLBL.AutoSize = true;
            this.VerLBL.Font = new System.Drawing.Font("Segoe UI Semibold", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VerLBL.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.VerLBL.Location = new System.Drawing.Point(15, 346);
            this.VerLBL.MaximumSize = new System.Drawing.Size(328, 0);
            this.VerLBL.Name = "VerLBL";
            this.VerLBL.Size = new System.Drawing.Size(55, 12);
            this.VerLBL.TabIndex = 3;
            this.VerLBL.Text = "win64-v4.1";
            // 
            // WinCtrlPNL
            // 
            this.WinCtrlPNL.BackColor = System.Drawing.Color.Transparent;
            this.WinCtrlPNL.Controls.Add(this.CloseWinPB);
            this.WinCtrlPNL.Controls.Add(this.MinWinPB);
            this.WinCtrlPNL.Controls.Add(this.MoveWinPB);
            this.WinCtrlPNL.Location = new System.Drawing.Point(267, 5);
            this.WinCtrlPNL.Name = "WinCtrlPNL";
            this.WinCtrlPNL.Size = new System.Drawing.Size(98, 30);
            this.WinCtrlPNL.TabIndex = 1200;
            // 
            // CloseWinPB
            // 
            this.CloseWinPB.BackgroundImage = global::FoneCall.Properties.Resources.Close;
            this.CloseWinPB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CloseWinPB.Location = new System.Drawing.Point(58, 7);
            this.CloseWinPB.Margin = new System.Windows.Forms.Padding(5);
            this.CloseWinPB.Name = "CloseWinPB";
            this.CloseWinPB.Size = new System.Drawing.Size(18, 20);
            this.CloseWinPB.TabIndex = 1201;
            this.CloseWinPB.TabStop = false;
            // 
            // MinWinPB
            // 
            this.MinWinPB.BackgroundImage = global::FoneCall.Properties.Resources.Minimize;
            this.MinWinPB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.MinWinPB.Cursor = System.Windows.Forms.Cursors.Hand;
            this.MinWinPB.Location = new System.Drawing.Point(30, 17);
            this.MinWinPB.Name = "MinWinPB";
            this.MinWinPB.Size = new System.Drawing.Size(18, 10);
            this.MinWinPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.MinWinPB.TabIndex = 1200;
            this.MinWinPB.TabStop = false;
            // 
            // MoveWinPB
            // 
            this.MoveWinPB.BackgroundImage = global::FoneCall.Properties.Resources.moveWin;
            this.MoveWinPB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.MoveWinPB.Location = new System.Drawing.Point(1, 7);
            this.MoveWinPB.Name = "MoveWinPB";
            this.MoveWinPB.Size = new System.Drawing.Size(20, 20);
            this.MoveWinPB.TabIndex = 1199;
            this.MoveWinPB.TabStop = false;
            // 
            // LogLBL
            // 
            this.LogLBL.AutoSize = true;
            this.LogLBL.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LogLBL.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.LogLBL.Location = new System.Drawing.Point(22, 301);
            this.LogLBL.MaximumSize = new System.Drawing.Size(328, 0);
            this.LogLBL.Name = "LogLBL";
            this.LogLBL.Size = new System.Drawing.Size(232, 15);
            this.LogLBL.TabIndex = 1206;
            this.LogLBL.Text = "C:\\Users\\Yo*urID\\AppData\\Local\\FoneCall";
            // 
            // CloseBTN
            // 
            this.CloseBTN.AutoSize = true;
            this.CloseBTN.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CloseBTN.BackColor = System.Drawing.Color.WhiteSmoke;
            this.CloseBTN.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CloseBTN.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.CloseBTN.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CloseBTN.Location = new System.Drawing.Point(185, 319);
            this.CloseBTN.Name = "CloseBTN";
            this.CloseBTN.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.CloseBTN.Size = new System.Drawing.Size(33, 25);
            this.CloseBTN.TabIndex = 1201;
            this.CloseBTN.Text = "OK";
            this.CloseBTN.UseVisualStyleBackColor = false;
            // 
            // RTBX
            // 
            this.RTBX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(87)))), ((int)(((byte)(79)))));
            this.RTBX.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RTBX.Cursor = System.Windows.Forms.Cursors.Default;
            this.RTBX.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RTBX.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.RTBX.Location = new System.Drawing.Point(20, 50);
            this.RTBX.Name = "RTBX";
            this.RTBX.ReadOnly = true;
            this.RTBX.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.RTBX.Size = new System.Drawing.Size(365, 283);
            this.RTBX.TabIndex = 1207;
            this.RTBX.TabStop = false;
            this.RTBX.Text = "";
            // 
            // Welcome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(87)))), ((int)(((byte)(79)))));
            this.ClientSize = new System.Drawing.Size(384, 364);
            this.Controls.Add(this.CloseBTN);
            this.Controls.Add(this.LogLBL);
            this.Controls.Add(this.WinCtrlPNL);
            this.Controls.Add(this.VerLBL);
            this.Controls.Add(this.RTBX);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Welcome";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "FoneCall";
            this.WinCtrlPNL.ResumeLayout(false);
            this.WinCtrlPNL.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CloseWinPB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinWinPB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MoveWinPB)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label VerLBL;
        private System.Windows.Forms.Panel WinCtrlPNL;
        private System.Windows.Forms.PictureBox CloseWinPB;
        private System.Windows.Forms.PictureBox MinWinPB;
        private System.Windows.Forms.PictureBox MoveWinPB;
        private System.Windows.Forms.Label LogLBL;
        private System.Windows.Forms.Button CloseBTN;
        private CertAdmin.RTB__FLB RTBX;
    }
}

