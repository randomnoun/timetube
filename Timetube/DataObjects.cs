using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace Timetube {

    // this file contains the ProcessDetails and WindowDetails value objects


    /// <summary>
    /// Stores the details for a single process
    /// </summary>
    class ProcessDetails {
        
        public int id;
        public string processName;
        public string processExe;
        public string dimensions;
        public string mainWindowTitle;

        public override String ToString() {
            return String.Format("{0}\t{1}\t{2}\t{3}\t{4}", id, processName, processExe, dimensions, mainWindowTitle);
        }
        public override bool Equals(Object other) {
            if (!(other is ProcessDetails)) { return false; }
            ProcessDetails o = (ProcessDetails)other;
            return (o.processName.Equals(processName) &&
                o.processExe.Equals(processExe) &&  // -- we could comment this out
                o.dimensions.Equals(dimensions) &&
                o.mainWindowTitle.Equals(mainWindowTitle));
        }

        public override int GetHashCode() {
            return id;
        }

        public static ProcessDetails ParseString(string s) {
            // Console.WriteLine("Parsing p '" + s + "'");
            Match m = Regex.Match(s, "^([0-9]+)\t(.*)\t(.*)\t(.*)$");
            if (m.Success) {
                ProcessDetails pd = new ProcessDetails();
                pd.id = Convert.ToInt32(m.Groups[1].Value);
                pd.processName = m.Groups[2].Value;
                pd.processExe = m.Groups[3].Value;
                pd.dimensions = m.Groups[4].Value;
                pd.mainWindowTitle = m.Groups[5].Value;
                return pd;
            } else {
                return null;
            }
        }
    }

    /// <summary>
    /// Stores the details for a single window
    /// </summary>
    class WindowDetails {
        public int pid;
        public int hwnd;
        public string dimensions;  // @TODO use rc
        public Rectangle dimensionsRect = new Rectangle(0, 0, 0, 0);

        public string module;
        public string title;
        // public string path;
        public int iconId;


        public override string ToString() {
            return String.Format("{0} {1} [{2}] {3} '{4}' '{5}'", pid, hwnd, iconId, dimensions, module, title);
        }
        public override bool Equals(object other) {
            if (!(other is WindowDetails)) { return false; }
            WindowDetails o = (WindowDetails)other;
            return (o.pid == pid && o.hwnd == hwnd &&
                o.dimensions.Equals(dimensions) &&
                ((o.module == null && module == null) || o.module.Equals(module)) &
                ((o.title == null && title == null) || o.title.Equals(title)));
        }
        public override int GetHashCode() {
            return pid + hwnd;
        }

        public static WindowDetails ParseString(string s) {
            // Console.WriteLine("Parsing w '" + s + "'");
            // was \\(([-0-9]*),([-0-9]*)\\)-\\(([-0-9]*),([-0-9]*)\\)
            // was Match m = Regex.Match(s, "^([0-9]+) ([0-9]+) \\[([0-9]+)\\] (.*) '(.*)' '(.*)'$");
            Match m = Regex.Match(s, "^([0-9]+) ([0-9]+) \\[([0-9]+)\\] \\(([-0-9]*),([-0-9]*)\\)-\\(([-0-9]*),([-0-9]*)\\) '(.*)' '(.*)'$");
            if (m.Success) {
                WindowDetails wd = new WindowDetails();
                wd.pid = Convert.ToInt32(m.Groups[1].Value);
                wd.hwnd = Convert.ToInt32(m.Groups[2].Value);
                wd.iconId = Convert.ToInt32(m.Groups[3].Value);
                // was  "(" + m.Groups[4].Value + "," + m.Groups[5].Value + ")-(" + m.Groups[6].Value + "," + m.Groups[7].Value + ")";
                // wd.dimensions = m.Groups[4].Value;
                wd.dimensions = "(" + m.Groups[4].Value + "," + m.Groups[5].Value + ")-(" + m.Groups[6].Value + "," + m.Groups[7].Value + ")";
                wd.dimensionsRect = new Rectangle(
                    Convert.ToInt32(m.Groups[4].Value),
                    Convert.ToInt32(m.Groups[5].Value),
                    Convert.ToInt32(m.Groups[6].Value) - Convert.ToInt32(m.Groups[4].Value),
                    Convert.ToInt32(m.Groups[7].Value) - Convert.ToInt32(m.Groups[5].Value));


                wd.module = m.Groups[8].Value;
                wd.title = m.Groups[9].Value;
                return wd;
            } else {
                return null;
            }

        }
    }

}
