using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using StringTok;
using System.Collections;

namespace Timetube {
    public partial class ViewLogForm : Form {
        string dataDir;
        List<DateTime> snapDates;
        DateTime minDate;
        DateTime maxDate;
        DateTime shownDate;
        Image backImage = null;
        Hashtable icons;

        public ViewLogForm(string dataDir) {
            InitializeComponent();
            this.dataDir = dataDir;

            lblStartTime.Text = "";
            lblStopTime.Text = "";
            lblStatusBar.Text = "";
            setBoldMonthDates();
            loadIcons();
            setTrackBarValues();
        }

        
        public void setBoldMonthDates() {
            // @TODO just restrict to those dates visible in the current month
            List<DateTime> list = new List<DateTime>();
            foreach (string dir in Directory.GetDirectories(dataDir)) {
                string lastBit = dir.Substring(dir.LastIndexOf('\\') + 1);
                Match m = Regex.Match(lastBit, "^([0-9]{4})-([0-9]{2})-([0-9]{2})$");
                if (m.Success) {
                    Console.WriteLine("Found " + dir);
                    DateTime dtm = new DateTime(Convert.ToInt32(m.Groups[1].Value),
                        Convert.ToInt32(m.Groups[2].Value),
                        Convert.ToInt32(m.Groups[3].Value));
                    list.Add(dtm);
                } else {
                    Console.WriteLine("Ignored " + dir);
                    // ignore non-dated countries
                }
            }
            DateTime[] monthlyBoldedDates = list.ToArray();
            calDate.MonthlyBoldedDates = monthlyBoldedDates;
        }

        public void setTrackBarValues() {
            // @TODO update image under trackbar
            List<DateTime> list = new List<DateTime>();
            int year = calDate.SelectionStart.Year;
            int month = calDate.SelectionStart.Month;
            int day = calDate.SelectionStart.Day;
            string dir = dataDir + "\\" + calDate.SelectionStart.ToString("yyyy-MM-dd");
            if (Directory.Exists(dir)) {
                foreach (string filename in Directory.GetFiles(dir, "*.jpg")) {
                    string lastBit = filename.Substring(filename.LastIndexOf('\\') + 1);
                    Match m = Regex.Match(lastBit, "^desktop-([0-9]{2}).([0-9]{2}).([0-9]{2}).1234.jpg$");
                    if (m.Success) {
                        Console.WriteLine("Found time " + lastBit);
                        DateTime dtm = new DateTime(year, month, day, Convert.ToInt32(m.Groups[1].Value),
                            Convert.ToInt32(m.Groups[2].Value),
                            Convert.ToInt32(m.Groups[3].Value));
                        list.Add(dtm);
                    } else {
                        Console.WriteLine("Ignore time " + lastBit);
                    }
                }
                list.Sort();
            } else {
                Console.WriteLine("could not find dir " + dir);
            }
            if (list.Count > 0) {
                snapDates = list;

                minDate = list[0];
                maxDate = list[list.Count - 1];
                DateTime actualMinDate = minDate;
                
                // round off to nearest 10 minutes
                DateTime baseDate = new DateTime(minDate.Year, minDate.Month, minDate.Day, 0, 0, 0);
                minDate = new DateTime(minDate.Year, minDate.Month, minDate.Day, minDate.Hour,
                    (minDate.Minute / 10) * 10, 0);
                // there's almost certainly a better way of doing this. 
                // @TODO will break at 23:50 on the last day of the month :)
                if (minDate.Minute >= 50) {
                    if (minDate.Hour == 23) {
                        maxDate = new DateTime(minDate.Year, minDate.Month, minDate.Day + 1, 0, 0, 0);
                    } else {
                        maxDate = new DateTime(minDate.Year, minDate.Month, minDate.Day, minDate.Hour + 1, 0, 0);
                    }
                } else {
                    maxDate = new DateTime(minDate.Year, minDate.Month, minDate.Day, minDate.Hour,
                        ((minDate.Minute / 10) + 1) * 10, 0);
                }
                lblStartTime.Text = minDate.ToString("hh:mm:ss");
                lblStopTime.Text = maxDate.ToString("hh:mm:ss");
                trkTime.Enabled = true;
                
                // convert from ticks (1 tick = 100ns) to seconds
                trkTime.Minimum = (int) ((minDate.Ticks - baseDate.Ticks) / 10000000);
                trkTime.Maximum = (int) ((maxDate.Ticks - baseDate.Ticks) / 10000000);
                int span = trkTime.Maximum - trkTime.Minimum;
                if (span < 60) { // less than 1 minute, tick 10 secs
                    trkTime.TickFrequency = 10;
                } else if (span <= 600) { // less than 10 min, tick min
                    trkTime.TickFrequency = 60;
                } else if (span <= 30 * 60) { // less than 30 minutes, tick 5 minutes 
                    trkTime.TickFrequency = 5 * 60;
                } else if (span <= 60 * 60) { // less than 1 hour, tick 10 minutes 
                    trkTime.TickFrequency = 10 * 60;
                } else if (span <= 3 * 60 * 60) { // less then 3 hours, tick 30 minutes
                    trkTime.TickFrequency = 30 * 60;
                } else { // tick every hour
                    trkTime.TickFrequency = 60 * 60;
                }
                trkTime.LargeChange = trkTime.TickFrequency;

                // @TODO set trkTime to proper date
                showTime(actualMinDate);

            } else {
                trkTime.Enabled = false;
                lblStartTime.Text = "";
                lblStopTime.Text = "";
            }

        }

        private void calDate_DateChanged(object sender, DateRangeEventArgs e) {
            setTrackBarValues();
        }

        private void loadIcons() {
            ImageList il = new ImageList();
            icons = new Hashtable();
            foreach (string file in Directory.GetFiles(dataDir + "\\iconCache", "*.png")) {
                Image img = Image.FromFile(file);
                Match m = Regex.Match(file, "^.*?([0-9-.]+).png$");
                if (m.Success) {
                    int thisIconId = Convert.ToInt32(m.Groups[1].Value);
                    Console.WriteLine("adding [" + thisIconId + "]" + file);
                    icons[thisIconId] = img;
                    il.Images.Add(Convert.ToString(thisIconId), img);
                } else {
                    Console.WriteLine("Cannot parse filename '" + file + "'");
                }
            }
            
            lstProcess.SmallImageList = il;


        }

        private void trkTime_Scroll(object sender, EventArgs e) {
            // find closest time
            // Console.WriteLine("Value is " + trkTime.Value);
            // TODO could keep this as a local var
            DateTime baseDate = new DateTime(calDate.SelectionStart.Year, calDate.SelectionStart.Month, calDate.SelectionStart.Day, 0, 0, 0);
            long ticks = ((long)trkTime.Value) * 10000000 + baseDate.Ticks;
            DateTime dtmTrack = new DateTime(ticks);
            Console.WriteLine("Which is " + dtmTrack.ToString("HH:mm:ss"));

            int idx = snapDates.BinarySearch(dtmTrack);
            if (idx < 0) {
                idx = (~idx);
                if (idx > 0) { idx--; }
            }
            DateTime closest = snapDates[idx];
            lblStatusBar.Text = "Selected: " + dtmTrack.ToString("HH:mm:ss") + "; closest snapshot: " +
                closest.ToString("hh:mm:ss");
            if (!closest.Equals(shownDate)) {
                showTime(closest);
            }
        }

        private void showTime(DateTime dateTime) {

            // get desktop
            string jpgFile = dataDir + "\\" + dateTime.ToString("yyyy-MM-dd") + "\\desktop-" + 
                dateTime.ToString("HH.mm.ss") + ".1234.jpg";
            backImage = Image.FromFile(jpgFile);

            desktopView.setImage(backImage);
            // pbxScreen.Invalidate();
            
            shownDate = dateTime;

            // get process list / windows
            // Open the file and read it back.
            List<ProcessDetails> pds = new List<ProcessDetails>();
            List<WindowDetails> wds = new List<WindowDetails>();
            string datafile = dataDir + "\\" + dateTime.ToString("yyyy-MM-dd") + "\\windowList.log";
            using (System.IO.StreamReader sr = System.IO.File.OpenText(datafile)) {
                string s = "";
                string country = sr.ReadLine();
                DateTime parsingDate = new DateTime();
                Boolean eof = false;

                while ((!eof) && (s = sr.ReadLine()) != null) {
                    // s = s.Trim();
                    if (s.StartsWith("*** Begin snapshot ")) {
                        string d = s.Substring(19, 8);
                        CultureInfo cultureInfo = CultureInfo.CurrentCulture;
                        parsingDate = DateTime.ParseExact(d, "HH.mm.ss", cultureInfo );
                        parsingDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
                            parsingDate.Hour, parsingDate.Minute, parsingDate.Second);
                    } else if (s.StartsWith("P + ")) {
                        ProcessDetails pd = ProcessDetails.ParseString(s.Substring(4));
                        if (pd != null) {
                            pds.Add(pd);
                        } else {
                            Console.WriteLine("Could not parse P+: " + s);
                        }

                    } else if (s.StartsWith("W + ")) {
                        WindowDetails wd = WindowDetails.ParseString(s.Substring(4));
                        if (wd != null) {
                            wds.Add(wd);
                        } else {
                            Console.WriteLine("Could not parse w+: " + s);
                        }

                    } else if (s.StartsWith("WO ! ")) {
                        List<int> newOrder = new List<int>();
                        StringTokenizer st = new StringTokenizer(s.Substring(5), " ");
                        while (st.HasMoreTokens()) {
                            newOrder.Add(Convert.ToInt32(st.NextToken()));
                        }
                    }

                    // read next line
                    s = sr.ReadLine();
                }
            }

            lstProcess.BeginUpdate();
            lstProcess.Items.Clear();
            foreach (WindowDetails wd in wds) {
                // @TODO check if window is visible
                Console.WriteLine("Adding wd '" + wd.title + "'");
                ListViewItem lvi = new ListViewItem(new string[] { wd.title }, Convert.ToString(wd.iconId));
                lvi.Text = wd.title;
                lvi.Tag = wd;
                // lvi.ImageIndex = lstProcess.SmallImageList.Images.IndexOfKey();
                lstProcess.Items.Add(lvi);
            }
            lstProcess.EndUpdate();

        }

        private void shadePanel_Paint(object sender, PaintEventArgs e) {
            Graphics g = e.Graphics;
            g.FillRectangle(Brushes.DarkGray, g.ClipBounds);
        }

        private void lstProcess_SelectedIndexChanged(object sender, EventArgs e) {
            if (lstProcess.SelectedItems.Count > 0) {
                ListViewItem lvi = lstProcess.SelectedItems[0];
                WindowDetails wd = (WindowDetails) lvi.Tag;
                lblStatusBar.Text = "Window " + wd.title + ": dimensions = " + wd.dimensions;
                desktopView.setHighlight(wd.dimensionsRect);
            } else {
                lblStatusBar.Text = "No window selected";
            }
        }

    }
}