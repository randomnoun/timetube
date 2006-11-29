using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Timetube {
    public partial class DesktopViewControl : UserControl {

        Image image = null;
        bool hasHighlight = false;
        Rectangle highlight;

        public void setImage(Image image) {
            this.image = image;
            this.Refresh();
        }

        public void setHighlight(Rectangle highlight) {
            this.highlight = highlight;
            this.hasHighlight = !highlight.Equals(Rectangle.Empty);
            this.Refresh();
        }

        public DesktopViewControl() {
            InitializeComponent();
        }

        private void panelImg_Resize(object sender, EventArgs e) {
            // this.Refresh();
        }

        private void panelImg_Paint(object sender, PaintEventArgs e) {
            Graphics g = e.Graphics;
            if (image != null) {
                g.DrawImage(image, 0, 0, panelImg.Width, panelImg.Height);
            }
            if (hasHighlight) {
                Pen p = new Pen(new SolidBrush(Color.FromArgb(128, 255, 0, 0)), 3);
                g.DrawRectangle(p, highlight.X * panelImg.Width / image.Width,
                      highlight.Y * panelImg.Width / image.Width,
                      highlight.Width * panelImg.Width / image.Width,
                      highlight.Height * panelImg.Width / image.Width);
                p.Dispose();
            }
        }
     
    }
}
