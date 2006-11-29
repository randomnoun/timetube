namespace Timetube {
    partial class DesktopViewControl {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.panelImg = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panelImg
            // 
            this.panelImg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelImg.Location = new System.Drawing.Point(0, 1);
            this.panelImg.Name = "panelImg";
            this.panelImg.Size = new System.Drawing.Size(149, 148);
            this.panelImg.TabIndex = 0;
            this.panelImg.Resize += new System.EventHandler(this.panelImg_Resize);
            this.panelImg.Paint += new System.Windows.Forms.PaintEventHandler(this.panelImg_Paint);
            // 
            // DesktopViewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelImg);
            this.Name = "DesktopViewControl";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelImg;
    }
}
