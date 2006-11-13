using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

using Microsoft.Win32;

namespace Timetube {
    public partial class TimetubeForm : Form {
        bool IsInitialised = false;
        Image screenImage;
        ScreenCapture sc;
        Timetube timeTube;

        public TimetubeForm() {
            InitializeComponent();

            // Attempt to open the key; create it if it doesn't exist
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Randomnoun\\Timetube");
            if (key == null) {
                key = Registry.CurrentUser.CreateSubKey("Software\\Randomnoun\\Timetube");
            }
            txtSaveDirectory.Text = (string) key.GetValue("SaveDirectory", "C:\\docs");
            txtInterval.Text = (string)key.GetValue("Interval", "10");
            cmbFormat.Text = (string)key.GetValue("Format", "JPEG");
            trkQuality.Value = (int) key.GetValue("Quality", 80);
            key.Close();
            
            /* load from registry */
            trkQuality.Enabled = (cmbFormat.Text.Equals("JPEG"));
            lblQuality.Enabled = (cmbFormat.Text.Equals("JPEG"));

            sc = new ScreenCapture();
            screenImage = sc.CaptureScreen();
            picPreview.Image = screenImage;
            picPreview.SizeMode = PictureBoxSizeMode.Normal;
            CalcPreview();

            timeTube = new Timetube();
            timeTube.SaveDirectory = txtSaveDirectory.Text;
            timeTube.Interval = Convert.ToInt32(txtInterval.Text);

            cmdStopLogging.Enabled = false;
            IsInitialised = true;
        }

        private void btnBrowse_Click(object sender, EventArgs e) {

            // folderBrowser1.DirectoryPath = txtSaveDirectory.Text;
            if (DialogResult.OK == folderBrowser1.ShowDialog()) {
                txtSaveDirectory.Text = folderBrowser1.DirectoryPath;
                
            }
        }

        private void cmdExit_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void cmbFormat_SelectedIndexChanged(object sender, EventArgs e) {
            SaveLogSettings();
            trkQuality.Enabled = (cmbFormat.Text.Equals("JPEG"));
            lblQuality.Enabled = (cmbFormat.Text.Equals("JPEG"));
            CalcPreview();
        }

        private void CalcPreview() {
            if (screenImage == null) {
                return;
            }

            long intInterval;
            try {
                intInterval = Convert.ToInt64(txtInterval.Text);
            } catch (FormatException) {
                // TODO: warn
                return;
            }

            // Encoder parameter for image quality 
            ImageCodecInfo imgCodec = null;
            EncoderParameters encoderParams = new EncoderParameters(0);
            if (cmbFormat.Text.Equals("JPEG")) {
                imgCodec = ScreenCapture.GetEncoderInfo("image/jpeg");
                encoderParams = new EncoderParameters(1);
                EncoderParameter qualityParam = new EncoderParameter(
                    System.Drawing.Imaging.Encoder.Quality, trkQuality.Value);
                encoderParams.Param[0] = qualityParam;
            } else if (cmbFormat.Text.Equals("GIF")) {
                imgCodec = ScreenCapture.GetEncoderInfo("image/gif");
                
            } else if (cmbFormat.Text.Equals("PNG")) {
                imgCodec = ScreenCapture.GetEncoderInfo("image/png");
                
            } else if (cmbFormat.Text.Equals("BMP")) {
                imgCodec = ScreenCapture.GetEncoderInfo("image/bmp");
            }

            if (imgCodec == null) {
                // @TODO warn about illegal codec
                return;
            }

            MemoryStream ms = new MemoryStream();
            screenImage.Save(ms, imgCodec, encoderParams);
            if (intInterval<1) { intInterval=1; }
            lblPreview.Text = FormatBytes(ms.Length) + " (" + FormatBytes(ms.Length * 86400 / intInterval) + "/day)";
            ms.Seek(0, SeekOrigin.Begin);
            picPreview.Image = Image.FromStream(ms);

            ms.Close();
        }

        private String FormatBytes(long count) {
            double countDbl = (double)count;
            if (count < 1024) {
                return String.Format("{0} bytes", count);
            } else if (count < 1048576) {
                return String.Format("{0:F2} KB", countDbl / 1024);
            } else if (count < 1073741824) {
                return String.Format("{0:F2} MB", countDbl / 1048576);
            } else if (count < 1099511627776) {
                return String.Format("{0:F2} GB", countDbl / 1073741824);
            } else {
                return String.Format("{0:F2} PB", countDbl / 1099511627776);
            }
        }

        private void trkQuality_Scroll(object sender, EventArgs e) {
            SaveLogSettings();
            lblQuality.Text = trkQuality.Value + "%";
            CalcPreview();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == MattGriffith.Interop.Win32.WM_SYSCOMMAND)
        		switch(m.WParam.ToInt32())		 
        	{
                    case MattGriffith.Interop.Win32.SC_MINIMIZE:
                    this.notifyIcon1.Text = "Double-click to view program options";
        			this.notifyIcon1.MinimizeToTray(this.Handle);
        			this.notifyIcon1.Visible = true;
                    this.notifyIcon1.ShowBalloon(MattGriffith.Interop.BalloonIconStyle.Info,
                        "Double-click this icon to return to the application.",
                        "The timetube is still running.", 10000);
        			return;
        		default:
        			break;
        	} 
        	base.WndProc(ref m);
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e) {
            this.notifyIcon1.RestoreFromTray(this.Handle);
            this.notifyIcon1.Visible = false;
        }

        private void cmdStartLogging_Click(object sender, EventArgs e) {
            // txtStatus.Text = "  Logging is enabled";
            if (IsFormValid()) {
                this.picStatus.Image = global::Timetube.Properties.Resources.traffic_green;
                cmdStartLogging.Enabled = false;
                cmdStopLogging.Enabled = true;
                EnableControls(false);

                timeTube.SaveDirectory = txtSaveDirectory.Text;
                timeTube.SaveFormat = cmbFormat.Text;
                timeTube.SaveQuality = trkQuality.Value;
                int intInterval = 10;
                try {
                    intInterval = Convert.ToInt32(txtInterval.Text);
                    if (intInterval < 1) { intInterval = 1; }
                } catch (FormatException) {
                }
                timeTube.Interval = intInterval;
                timeTube.BeginCapture();
            }            
        }

        private bool IsFormValid() {
            if (!Directory.Exists(txtSaveDirectory.Text)) {
                if (MessageBox.Show("The directory you have specified does not exist - create it ?", 
                    "Missing directory", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK) {
                    try {
                        Directory.CreateDirectory(txtSaveDirectory.Text);
                    } catch (NotSupportedException) {
                        MessageBox.Show("The directory you have entered is invalid; please enter " +
                            "a different directory name.", "Invalid directory", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                } else {
                    return false;
                }
            }

            int intInterval = 10;
            try {
                intInterval = Convert.ToInt32(txtInterval.Text);
                if (intInterval < 5) {
                    MessageBox.Show("The interval you have entered is invalid. intervals must be " +
                        "at least 5 seconds apart.", "Invalid interval", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            } catch (FormatException) {
                MessageBox.Show("The interval you have entered is invalid. Intervals must be " +
                    "recorded as a number (e.g. \"10\")", "Invalid interval", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            if (!(cmbFormat.Text.Equals("JPEG") || cmbFormat.Text.Equals("GIF") ||
                cmbFormat.Text.Equals("PNG") || cmbFormat.Text.Equals("BMP"))) {
                MessageBox.Show("The image format you have entered is invalid. The image format must be " +
                    "set to JPEG, GIF, PNG or BMP.", "Invalid image format", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            return true;
        }

        private void cmdStopLogging_Click(object sender, EventArgs e) {
            // txtStatus.Text = "  Logging is disabled";
            picStatus.Image = global::Timetube.Properties.Resources.traffic_red;
            cmdStartLogging.Enabled = true;
            cmdStopLogging.Enabled = false;
            EnableControls(true);
            timeTube.EndCapture();
        }

        private void cmdLogSettings_Click(object sender, EventArgs e) {
            MessageBox.Show("Just change them on the main form", "Not implemented");
        }


        private void EnableControls(bool enable) {
            txtSaveDirectory.Enabled = enable;
            cmbFormat.Enabled = enable;
            trkQuality.Enabled = enable && (cmbFormat.Text.Equals("JPEG"));
            txtInterval.Enabled = enable;
            chkSaveDesktop.Enabled = enable;
            chkSaveProcessList.Enabled = enable;
            chkSaveWindowList.Enabled = enable;
        }


        private void cmdAbout_Click(object sender, EventArgs e) {
            AboutBox1 ab = new AboutBox1();
            ab.ShowDialog();
        }

        private void txtSaveDirectory_TextChanged(object sender, EventArgs e) {
            SaveLogSettings();
        }

        private void txtInterval_TextChanged(object sender, EventArgs e) {
            SaveLogSettings();
            CalcPreview();
        }

        private void SaveLogSettings() {
            if (!IsInitialised) { return; }

            // Attempt to open the key
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Randomnoun\\Timetube", true);

            // If the return value is null, the key doesn't exist
            if (key == null) {
                key = Registry.CurrentUser.CreateSubKey("Software\\Randomnoun\\Timetube");
            }
            key.SetValue("SaveDirectory", txtSaveDirectory.Text);
            key.SetValue("Interval", txtInterval.Text);
            key.SetValue("Quality", trkQuality.Value);
            key.SetValue("Format", cmbFormat.Text);
            key.Close();
        }

        private void cmdViewLog_Click(object sender, EventArgs e) {
            ViewLogForm viewLogForm = new ViewLogForm(txtSaveDirectory.Text);
            viewLogForm.ShowDialog();
        }


    }
}