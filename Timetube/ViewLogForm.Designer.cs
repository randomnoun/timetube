namespace Timetube {
    partial class ViewLogForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lstProcess = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.calDate = new System.Windows.Forms.MonthCalendar();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatusBar = new System.Windows.Forms.ToolStripStatusLabel();
            this.pbxScreen = new System.Windows.Forms.PictureBox();
            this.trkTime = new System.Windows.Forms.TrackBar();
            this.lblStartTime = new System.Windows.Forms.Label();
            this.lblStopTime = new System.Windows.Forms.Label();
            this.shadePanel = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxScreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkTime)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lstProcess);
            this.groupBox1.Location = new System.Drawing.Point(496, 179);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(178, 171);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Processes and windows";
            // 
            // lstProcess
            // 
            this.lstProcess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstProcess.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lstProcess.FullRowSelect = true;
            this.lstProcess.GridLines = true;
            this.lstProcess.Location = new System.Drawing.Point(6, 19);
            this.lstProcess.Name = "lstProcess";
            this.lstProcess.Size = new System.Drawing.Size(166, 146);
            this.lstProcess.TabIndex = 0;
            this.lstProcess.UseCompatibleStateImageBehavior = false;
            this.lstProcess.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Windows";
            this.columnHeader1.Width = 140;
            // 
            // calDate
            // 
            this.calDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.calDate.Location = new System.Drawing.Point(496, 12);
            this.calDate.MaxSelectionCount = 1;
            this.calDate.Name = "calDate";
            this.calDate.TabIndex = 3;
            this.calDate.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.calDate_DateChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatusBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 353);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(692, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatusBar
            // 
            this.lblStatusBar.Name = "lblStatusBar";
            this.lblStatusBar.Size = new System.Drawing.Size(80, 17);
            this.lblStatusBar.Text = "Status bar text";
            // 
            // pbxScreen
            // 
            this.pbxScreen.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pbxScreen.BackColor = System.Drawing.Color.Black;
            this.pbxScreen.Location = new System.Drawing.Point(12, 12);
            this.pbxScreen.Name = "pbxScreen";
            this.pbxScreen.Size = new System.Drawing.Size(472, 287);
            this.pbxScreen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxScreen.TabIndex = 0;
            this.pbxScreen.TabStop = false;
            // 
            // trkTime
            // 
            this.trkTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trkTime.Location = new System.Drawing.Point(12, 305);
            this.trkTime.Name = "trkTime";
            this.trkTime.Size = new System.Drawing.Size(472, 45);
            this.trkTime.TabIndex = 5;
            this.trkTime.Scroll += new System.EventHandler(this.trkTime_Scroll);
            // 
            // lblStartTime
            // 
            this.lblStartTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStartTime.BackColor = System.Drawing.Color.Transparent;
            this.lblStartTime.Location = new System.Drawing.Point(12, 337);
            this.lblStartTime.Name = "lblStartTime";
            this.lblStartTime.Size = new System.Drawing.Size(100, 13);
            this.lblStartTime.TabIndex = 6;
            this.lblStartTime.Text = "lblStartTime";
            // 
            // lblStopTime
            // 
            this.lblStopTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStopTime.BackColor = System.Drawing.Color.Transparent;
            this.lblStopTime.Location = new System.Drawing.Point(384, 337);
            this.lblStopTime.Name = "lblStopTime";
            this.lblStopTime.Size = new System.Drawing.Size(100, 13);
            this.lblStopTime.TabIndex = 7;
            this.lblStopTime.Text = "lblStopTime";
            this.lblStopTime.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // shadePanel
            // 
            this.shadePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.shadePanel.Location = new System.Drawing.Point(12, 345);
            this.shadePanel.Name = "shadePanel";
            this.shadePanel.Size = new System.Drawing.Size(475, 8);
            this.shadePanel.TabIndex = 8;
            this.shadePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.shadePanel_Paint);
            // 
            // ViewLogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 375);
            this.Controls.Add(this.lblStartTime);
            this.Controls.Add(this.lblStopTime);
            this.Controls.Add(this.shadePanel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.calDate);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pbxScreen);
            this.Controls.Add(this.trkTime);
            this.Name = "ViewLogForm";
            this.Text = "ViewLogForm";
            this.groupBox1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxScreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trkTime)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbxScreen;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.MonthCalendar calDate;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusBar;
        public System.Windows.Forms.TrackBar trkTime;
        private System.Windows.Forms.Label lblStartTime;
        private System.Windows.Forms.Label lblStopTime;
        private System.Windows.Forms.ListView lstProcess;
        private System.Windows.Forms.Panel shadePanel;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}