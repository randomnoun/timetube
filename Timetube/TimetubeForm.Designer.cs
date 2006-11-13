namespace Timetube {
    partial class TimetubeForm {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimetubeForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmdStartLogging = new System.Windows.Forms.ToolStripMenuItem();
            this.cmdStopLogging = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cmdLogSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cmdExit = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmdAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.picPreview = new System.Windows.Forms.PictureBox();
            this.txtSaveDirectory = new System.Windows.Forms.TextBox();
            this.lblSaveDirectory = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.chkSaveWindowList = new System.Windows.Forms.CheckBox();
            this.chkSaveProcessList = new System.Windows.Forms.CheckBox();
            this.chkSaveDesktop = new System.Windows.Forms.CheckBox();
            this.lblFormat = new System.Windows.Forms.Label();
            this.cmbFormat = new System.Windows.Forms.ComboBox();
            this.trkQuality = new System.Windows.Forms.TrackBar();
            this.lblQuality = new System.Windows.Forms.Label();
            this.lblInterval = new System.Windows.Forms.Label();
            this.txtInterval = new System.Windows.Forms.TextBox();
            this.lblPreview = new System.Windows.Forms.Label();
            this.notifyMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmdNotifyOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.cmdNotifyExit = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowser1 = new FolderBrowser();
            this.notifyIcon1 = new MattGriffith.Windows.Forms.NotifyIcon(this.components);
            this.picStatus = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdViewLog = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            this.grpOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkQuality)).BeginInit();
            this.notifyMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(589, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmdStartLogging,
            this.cmdStopLogging,
            this.toolStripSeparator1,
            this.cmdViewLog,
            this.toolStripSeparator3,
            this.cmdLogSettings,
            this.toolStripSeparator2,
            this.cmdExit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // cmdStartLogging
            // 
            this.cmdStartLogging.Name = "cmdStartLogging";
            this.cmdStartLogging.Size = new System.Drawing.Size(179, 22);
            this.cmdStartLogging.Text = "Start logging";
            this.cmdStartLogging.Click += new System.EventHandler(this.cmdStartLogging_Click);
            // 
            // cmdStopLogging
            // 
            this.cmdStopLogging.Name = "cmdStopLogging";
            this.cmdStopLogging.Size = new System.Drawing.Size(179, 22);
            this.cmdStopLogging.Text = "Stop logging";
            this.cmdStopLogging.Click += new System.EventHandler(this.cmdStopLogging_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(176, 6);
            // 
            // cmdLogSettings
            // 
            this.cmdLogSettings.Name = "cmdLogSettings";
            this.cmdLogSettings.Size = new System.Drawing.Size(179, 22);
            this.cmdLogSettings.Text = "Log settings...";
            this.cmdLogSettings.Click += new System.EventHandler(this.cmdLogSettings_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(176, 6);
            // 
            // cmdExit
            // 
            this.cmdExit.Name = "cmdExit";
            this.cmdExit.Size = new System.Drawing.Size(179, 22);
            this.cmdExit.Text = "Exit";
            this.cmdExit.Click += new System.EventHandler(this.cmdExit_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmdAbout});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // cmdAbout
            // 
            this.cmdAbout.Name = "cmdAbout";
            this.cmdAbout.Size = new System.Drawing.Size(126, 22);
            this.cmdAbout.Text = "About...";
            this.cmdAbout.Click += new System.EventHandler(this.cmdAbout_Click);
            // 
            // picPreview
            // 
            this.picPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.picPreview.BackColor = System.Drawing.SystemColors.Desktop;
            this.picPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picPreview.Location = new System.Drawing.Point(12, 125);
            this.picPreview.Name = "picPreview";
            this.picPreview.Size = new System.Drawing.Size(568, 243);
            this.picPreview.TabIndex = 1;
            this.picPreview.TabStop = false;
            // 
            // txtSaveDirectory
            // 
            this.txtSaveDirectory.Location = new System.Drawing.Point(93, 41);
            this.txtSaveDirectory.Name = "txtSaveDirectory";
            this.txtSaveDirectory.Size = new System.Drawing.Size(150, 20);
            this.txtSaveDirectory.TabIndex = 2;
            this.txtSaveDirectory.TextChanged += new System.EventHandler(this.txtSaveDirectory_TextChanged);
            // 
            // lblSaveDirectory
            // 
            this.lblSaveDirectory.AutoSize = true;
            this.lblSaveDirectory.Location = new System.Drawing.Point(9, 44);
            this.lblSaveDirectory.Name = "lblSaveDirectory";
            this.lblSaveDirectory.Size = new System.Drawing.Size(78, 13);
            this.lblSaveDirectory.TabIndex = 3;
            this.lblSaveDirectory.Text = "Save directory:";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(249, 39);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 4;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // grpOptions
            // 
            this.grpOptions.Controls.Add(this.chkSaveWindowList);
            this.grpOptions.Controls.Add(this.chkSaveProcessList);
            this.grpOptions.Controls.Add(this.chkSaveDesktop);
            this.grpOptions.Location = new System.Drawing.Point(331, 32);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(208, 87);
            this.grpOptions.TabIndex = 5;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Options";
            // 
            // chkSaveWindowList
            // 
            this.chkSaveWindowList.AutoSize = true;
            this.chkSaveWindowList.Checked = true;
            this.chkSaveWindowList.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSaveWindowList.Location = new System.Drawing.Point(7, 66);
            this.chkSaveWindowList.Name = "chkSaveWindowList";
            this.chkSaveWindowList.Size = new System.Drawing.Size(180, 17);
            this.chkSaveWindowList.TabIndex = 2;
            this.chkSaveWindowList.Text = "Save window locations and titles";
            this.chkSaveWindowList.UseVisualStyleBackColor = true;
            // 
            // chkSaveProcessList
            // 
            this.chkSaveProcessList.AutoSize = true;
            this.chkSaveProcessList.Checked = true;
            this.chkSaveProcessList.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSaveProcessList.Location = new System.Drawing.Point(7, 43);
            this.chkSaveProcessList.Name = "chkSaveProcessList";
            this.chkSaveProcessList.Size = new System.Drawing.Size(155, 17);
            this.chkSaveProcessList.TabIndex = 1;
            this.chkSaveProcessList.Text = "Save process list and icons";
            this.chkSaveProcessList.UseVisualStyleBackColor = true;
            // 
            // chkSaveDesktop
            // 
            this.chkSaveDesktop.AutoSize = true;
            this.chkSaveDesktop.Checked = true;
            this.chkSaveDesktop.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSaveDesktop.Location = new System.Drawing.Point(7, 20);
            this.chkSaveDesktop.Name = "chkSaveDesktop";
            this.chkSaveDesktop.Size = new System.Drawing.Size(123, 17);
            this.chkSaveDesktop.TabIndex = 0;
            this.chkSaveDesktop.Text = "Save desktop image";
            this.chkSaveDesktop.UseVisualStyleBackColor = true;
            // 
            // lblFormat
            // 
            this.lblFormat.AutoSize = true;
            this.lblFormat.Location = new System.Drawing.Point(9, 70);
            this.lblFormat.Name = "lblFormat";
            this.lblFormat.Size = new System.Drawing.Size(85, 13);
            this.lblFormat.TabIndex = 6;
            this.lblFormat.Text = "Format / Quality:";
            // 
            // cmbFormat
            // 
            this.cmbFormat.FormattingEnabled = true;
            this.cmbFormat.Items.AddRange(new object[] {
            "PNG",
            "GIF",
            "JPEG",
            "BMP"});
            this.cmbFormat.Location = new System.Drawing.Point(93, 67);
            this.cmbFormat.Name = "cmbFormat";
            this.cmbFormat.Size = new System.Drawing.Size(64, 21);
            this.cmbFormat.TabIndex = 7;
            this.cmbFormat.SelectedIndexChanged += new System.EventHandler(this.cmbFormat_SelectedIndexChanged);
            // 
            // trkQuality
            // 
            this.trkQuality.Enabled = false;
            this.trkQuality.LargeChange = 10;
            this.trkQuality.Location = new System.Drawing.Point(163, 67);
            this.trkQuality.Maximum = 100;
            this.trkQuality.Name = "trkQuality";
            this.trkQuality.Size = new System.Drawing.Size(122, 45);
            this.trkQuality.TabIndex = 8;
            this.trkQuality.TickFrequency = 10;
            this.trkQuality.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trkQuality.Scroll += new System.EventHandler(this.trkQuality_Scroll);
            // 
            // lblQuality
            // 
            this.lblQuality.AutoSize = true;
            this.lblQuality.Location = new System.Drawing.Point(291, 70);
            this.lblQuality.Name = "lblQuality";
            this.lblQuality.Size = new System.Drawing.Size(33, 13);
            this.lblQuality.TabIndex = 9;
            this.lblQuality.Text = "100%";
            // 
            // lblInterval
            // 
            this.lblInterval.AutoSize = true;
            this.lblInterval.Location = new System.Drawing.Point(9, 97);
            this.lblInterval.Name = "lblInterval";
            this.lblInterval.Size = new System.Drawing.Size(71, 13);
            this.lblInterval.TabIndex = 11;
            this.lblInterval.Text = "Interval (sec):";
            // 
            // txtInterval
            // 
            this.txtInterval.Location = new System.Drawing.Point(93, 94);
            this.txtInterval.Name = "txtInterval";
            this.txtInterval.Size = new System.Drawing.Size(64, 20);
            this.txtInterval.TabIndex = 10;
            this.txtInterval.Text = "10";
            this.txtInterval.TextChanged += new System.EventHandler(this.txtInterval_TextChanged);
            // 
            // lblPreview
            // 
            this.lblPreview.AutoSize = true;
            this.lblPreview.Location = new System.Drawing.Point(163, 102);
            this.lblPreview.Name = "lblPreview";
            this.lblPreview.Size = new System.Drawing.Size(137, 13);
            this.lblPreview.TabIndex = 12;
            this.lblPreview.Text = "Preview: 256K (1.2GB/day)";
            // 
            // notifyMenu
            // 
            this.notifyMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmdNotifyOptions,
            this.cmdNotifyExit});
            this.notifyMenu.Name = "notifyMenu";
            this.notifyMenu.Size = new System.Drawing.Size(135, 48);
            // 
            // cmdNotifyOptions
            // 
            this.cmdNotifyOptions.Name = "cmdNotifyOptions";
            this.cmdNotifyOptions.Size = new System.Drawing.Size(134, 22);
            this.cmdNotifyOptions.Text = "Options...";
            // 
            // cmdNotifyExit
            // 
            this.cmdNotifyExit.Name = "cmdNotifyExit";
            this.cmdNotifyExit.Size = new System.Drawing.Size(134, 22);
            this.cmdNotifyExit.Text = "Exit";
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenu = null;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // picStatus
            // 
            this.picStatus.Image = global::Timetube.Properties.Resources.traffic_red;
            this.picStatus.Location = new System.Drawing.Point(548, 52);
            this.picStatus.Name = "picStatus";
            this.picStatus.Size = new System.Drawing.Size(32, 32);
            this.picStatus.TabIndex = 13;
            this.picStatus.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(545, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Status:";
            // 
            // cmdViewLog
            // 
            this.cmdViewLog.Name = "cmdViewLog";
            this.cmdViewLog.Size = new System.Drawing.Size(179, 22);
            this.cmdViewLog.Text = "View log";
            this.cmdViewLog.Click += new System.EventHandler(this.cmdViewLog_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 6);
            // 
            // TimetubeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 380);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.picStatus);
            this.Controls.Add(this.picPreview);
            this.Controls.Add(this.lblSaveDirectory);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.lblPreview);
            this.Controls.Add(this.lblInterval);
            this.Controls.Add(this.txtInterval);
            this.Controls.Add(this.lblQuality);
            this.Controls.Add(this.trkQuality);
            this.Controls.Add(this.cmbFormat);
            this.Controls.Add(this.lblFormat);
            this.Controls.Add(this.grpOptions);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtSaveDirectory);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "TimetubeForm";
            this.Text = "TimetubeForm";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
            this.grpOptions.ResumeLayout(false);
            this.grpOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trkQuality)).EndInit();
            this.notifyMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cmdStartLogging;
        private System.Windows.Forms.ToolStripMenuItem cmdStopLogging;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem cmdLogSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem cmdExit;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cmdAbout;
        private System.Windows.Forms.PictureBox picPreview;
        private System.Windows.Forms.TextBox txtSaveDirectory;
        private System.Windows.Forms.Label lblSaveDirectory;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.CheckBox chkSaveProcessList;
        private System.Windows.Forms.CheckBox chkSaveDesktop;
        private System.Windows.Forms.Label lblFormat;
        private System.Windows.Forms.ComboBox cmbFormat;
        private System.Windows.Forms.CheckBox chkSaveWindowList;
        private System.Windows.Forms.TrackBar trkQuality;
        private System.Windows.Forms.Label lblQuality;
        private System.Windows.Forms.Label lblInterval;
        private System.Windows.Forms.TextBox txtInterval;
        private System.Windows.Forms.Label lblPreview;
        private FolderBrowser folderBrowser1;
        private MattGriffith.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip notifyMenu;
        private System.Windows.Forms.ToolStripMenuItem cmdNotifyOptions;
        private System.Windows.Forms.ToolStripMenuItem cmdNotifyExit;
        private System.Windows.Forms.PictureBox picStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem cmdViewLog;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    }
}