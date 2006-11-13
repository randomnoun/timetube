using System;
using System.IO;
using System.Runtime.InteropServices; // DllImport
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.ComponentModel;
using System.Threading;

using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

// perf notes: if time is set to fire every 5secs, this app uses 1.2% of CPU 
// (on my laptop, at least, with not too many processes or windows running)

namespace Timetube {

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
            ProcessDetails o = (ProcessDetails) other;
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

    class WindowDetails {
        public int pid;
        public int hwnd;
        public string dimensions;  // @TODO use rc
        public string module;
        public string title;
        // public string path;
        public int iconId;


        public override string ToString() {
            return String.Format("{0} {1} [{2}] {3} '{4}' '{5}'", pid, hwnd, iconId, dimensions, module, title);
        }
        public override bool Equals(object other) {
            if (!(other is WindowDetails)) { return false; }
            WindowDetails o = (WindowDetails) other;
            return (o.pid == pid && o.hwnd == hwnd &&
                o.dimensions.Equals(dimensions) &&
                ((o.module==null && module==null) || o.module.Equals(module)) &
                ((o.title==null && title==null) || o.title.Equals(title)));
        }
        public override int GetHashCode() {
            return pid + hwnd;
        }

        public static WindowDetails ParseString(string s) {
            // Console.WriteLine("Parsing w '" + s + "'");
            // was \\(([-0-9]*),([-0-9]*)\\)-\\(([-0-9]*),([-0-9]*)\\)
            Match m = Regex.Match(s, "^([0-9]+) ([0-9]+) \\[([0-9]+)\\] (.*) '(.*)' '(.*)'$");
            if (m.Success) {
                WindowDetails wd = new WindowDetails();
                wd.pid = Convert.ToInt32(m.Groups[1].Value);
                wd.hwnd = Convert.ToInt32(m.Groups[2].Value);
                wd.iconId = Convert.ToInt32(m.Groups[3].Value);
                // was  "(" + m.Groups[4].Value + "," + m.Groups[5].Value + ")-(" + m.Groups[6].Value + "," + m.Groups[7].Value + ")";
                wd.dimensions = m.Groups[4].Value;
                wd.module = m.Groups[5].Value;
                wd.title = m.Groups[6].Value;
                return wd;
            } else {
                return null;
            }

        }
    }

    class Timetube {

        static Timetube singleton = null;

        System.Timers.Timer timer = null;
        EventArrivedEventHandler eventArrivedEventHandler;
        ManagementEventWatcher win32ProcCreatedWatcher;
        ManagementEventWatcher win32ProcDeletedWatcher;

        Hashtable processMap = new Hashtable();
        Hashtable windowMap = new Hashtable();
        Hashtable iconMap = new Hashtable();
        public List<String> eventList = new List<String>();
        List<int> hwndOrder = new List<int>();

        bool   working = false;  // guard around OnTimer method
        public int iconNum = 0;  // number of icons cached so far

        public static Timetube getInstance() {
            if (singleton == null) {
                singleton = new Timetube();
            }
            return singleton;
        }

        // public properties
        private string _SaveDirectory;
        public string SaveDirectory {
            get { return _SaveDirectory;  }
            set { _SaveDirectory = value; }
        }
        private int _Interval;
        public int Interval {
            get { return _Interval; }
            set { _Interval = value; }
        }
        private bool _IsRunning = false;
        public bool IsRunning {
            get { return _IsRunning; }
        }
        private string _SaveFormat = "JPEG";
        public string SaveFormat {
            get { return _SaveFormat; }
            set { _SaveFormat = SaveFormat; }
        }
        private int _SaveQuality = 80;
        public int SaveQuality {
            get { return _SaveQuality; }
            set { _SaveQuality = SaveQuality; }
        }


        public Hashtable getProcessMap() {
            Hashtable newMap = new Hashtable();
            System.Diagnostics.Process[] myProcesses = System.Diagnostics.Process.GetProcesses();
            for (int i = 0; i < myProcesses.Length; i++) {
                ProcessDetails pd = new ProcessDetails();
                newMap.Add(myProcesses[i].Id, pd);
                System.Diagnostics.Process p = myProcesses[i];
                pd.id = myProcesses[i].Id;
                // cache process name ?
                pd.processName = p.ProcessName;
                if (!processMap.ContainsKey(pd.id)) {
                    try {
                        pd.processExe = p.MainModule.FileName;
                    } catch (System.ComponentModel.Win32Exception) {
                        pd.processExe = "";
                    }
                } else {
                    // just copy the exe across from previous lookup to save time
                    pd.processExe = ((ProcessDetails)processMap[pd.id]).processExe;
                }
                
                pd.dimensions = "";
                pd.mainWindowTitle = "";
                

                // this takes 2 secs to iterate; replaced with GetWindowText() below
                // pd.mainWindowTitle = p.MainWindowTitle;  

                // this takes 0.78 secs; replaced with window enumeration below
                /*
                try {
                    Win32.RECT rc = new Win32.RECT();
                    IntPtr hwnd = p.MainWindowHandle;
                    Win32.GetWindowRect(hwnd, ref rc);
                    pd.dimensions = String.Format("({0},{1})-({2},{3})", rc.left, rc.top, rc.right, rc.bottom);
                } catch (System.ComponentModel.Win32Exception we) {
                }
                 */

                // this takes 2 seconds to process for all processes on my laptop (arg!);
                // replaced with WMI + getWindowModuleFilename() below
                /*
                try {
                    System.Diagnostics.ProcessModuleCollection pcm = p.Modules;
                    if (pcm.Count > 0) {
                        System.Diagnostics.ProcessModule pm = pcm[0];
                        pd.processExe = pm.FileName;
                    }
                } catch (System.ComponentModel.Win32Exception we) {
                } catch (InvalidOperationException ioe) {
                    // occurs if process is closed whilst process list is being iterated
                }
                */
                // Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", myProcesses[i].Id, processName, processExe, dimensions, p.MainWindowTitle);
            }
            return newMap;
        }

        public void mergeProcessMap(StreamWriter sw, Hashtable newMap) {
            foreach (int pid in processMap.Keys) {
                if (!newMap.ContainsKey(pid)) {
                    sw.WriteLine("P - " + pid);
                } else if (!newMap[pid].Equals(processMap[pid])) {
                    sw.WriteLine("P ! " + newMap[pid].ToString());
                }
            }
            foreach (int pid in newMap.Keys) {
                if (!processMap.ContainsKey(pid)) {
                    sw.WriteLine("P + " + newMap[pid].ToString());
                }
            }
            processMap = newMap;
        }

        public void mergeWindowMap(StreamWriter sw, Hashtable newMap) {
            foreach (string handle in windowMap.Keys) {
                if (!newMap.ContainsKey(handle)) {
                    sw.WriteLine("W - " + handle);
                } else if (!newMap[handle].Equals(windowMap[handle])) {
                    sw.WriteLine("W ! " + newMap[handle].ToString());
                }
            }
            foreach (string handle in newMap.Keys) {
                if (!windowMap.ContainsKey(handle)) {
                    sw.WriteLine("W + " + newMap[handle].ToString());
                }
            }
            windowMap = newMap;
        }

        public void mergeHwndOrder(StreamWriter sw, List<int> newHwndOrder) {
            // work out the bottom-most window that's unchanged
            int unchanged = hwndOrder.Count-1;
            int scan = newHwndOrder.Count-1;
            while (unchanged > 0 && scan > 0 && (hwndOrder[unchanged] == newHwndOrder[scan])) {
                unchanged--;
                scan--;
            }
            // List from unchanged->hwndOrder.Count == scan->newHwndOrder.Count, just display 0->scan
            if (scan > 0) {
                sw.Write("WO ! ");
                for (int i = 0; i <= scan; i++) {
                    sw.Write(newHwndOrder[i] + " ");
                }
                sw.WriteLine();
            }
            hwndOrder = newHwndOrder;
        }

        public Hashtable getWindowMap(List<int> newHwndOrder) {
            Hashtable newMap = new Hashtable();
            Win32.RECT rc = new Win32.RECT();
            IntPtr hwnd = Win32.GetDesktopWindow();
            hwnd = Win32.GetTopWindow(hwnd);  // get first window

            int pid;
            if (IntPtr.Zero == hwnd) { return null; }
            StringBuilder title = new StringBuilder(1000);
            StringBuilder module = new StringBuilder(1000);
            //StringBuilder path = new StringBuilder(1000);
            while (IntPtr.Zero != hwnd) {
                IntPtr threadId = Win32.GetWindowThreadProcessId(hwnd, out pid);
                if (Win32.IsWindowVisible(hwnd) != 0) {
                    newHwndOrder.Add(hwnd.ToInt32());
                    IntPtr processHwnd = Win32.OpenProcess(0, false, pid);  // pid ?
                    
                    //path.Length = 0;
                    module.Length = 0;
                    title.Length = 0;
                    // Win32.GetModuleFileName(processHwnd, path, 1000); // just gets this .exe's filename
                    Win32.GetWindowModuleFileName(hwnd, module, 1000);   // seems to be empty most of the time ?
                    Win32.GetWindowText(hwnd, title, 1000);
                    Win32.GetWindowRect(hwnd, ref rc);

                    // Console.WriteLine("{0} {1} ({2},{3})-({4},{5}) '{6}' '{7}'", pid, hwnd.ToInt32(), rc.left, rc.top, rc.right, rc.bottom, module, title);
                    WindowDetails wd = new WindowDetails();
                    wd.pid = pid;
                    wd.hwnd = hwnd.ToInt32();
                    wd.dimensions = String.Format("({0},{1})-({2},{3})", rc.left, rc.top, rc.right, rc.bottom);
                    wd.title = title.ToString();
                    wd.module = module.ToString();
                    // wd.path = path.ToString();
                    newMap.Add(wd.pid + ":" + wd.hwnd, wd);

                    /* skip icon saving; check http://www.kennyandkarin.com/Kenny/CodeCorner/Tools/IconBrowser/
                     * later for some ideas
                     * - adderss space problems ?
                    IntPtr hIcon = (IntPtr)Win32.SendMessage(hwnd, 0x007f, 0, 0);   // WM_GETICON
                    if (IntPtr.Zero != hIcon) {
                        Icon ic = Icon.FromHandle(hwnd);
                        Bitmap b = ic.ToBitmap();
                        ic.Dispose();
                        FileStream fs = new FileStream(String.Format(@"c:\temp\icons\{0}.ico", iconNum), FileMode.Create, FileAccess.Write);
                        // ic.Save(fs);
                        b.Save(fs, System.Drawing.Imaging.ImageFormat.Icon);
                        fs.Close();
                        iconNum++;
                    }
                     */

                    // other code from  http://www.codeproject.com/csharp/taskbarsorter.asp 
                    UInt32 hIcon = 0;
                    if (hIcon == 0) hIcon = Win32.SendMessage(hwnd, WM.GETICON, ICON.SMALL2, 0);
                    if (hIcon == 0) hIcon = Win32.GetClassLong(hwnd, GCL.HICONSM);
                    if (hIcon == 0) hIcon = Win32.GetClassLong(hwnd, GCL.HICON);
                    if (hIcon != 0) {
                        Bitmap bitmap = null;
                        try {
                            Int32 hIcon2 = unchecked((Int32)hIcon);
                            bitmap = Bitmap.FromHicon(new IntPtr(hIcon2));
                        } catch (ArgumentException) { continue; }
                        if (bitmap != null) {
                            // hash bitmap; if it's new, save it.
                            string hash = getBitmapHash(bitmap);
                            if (iconMap.ContainsKey(hash)) {
                                // exists
                                wd.iconId = (int) iconMap[hash];
                            } else {
                                wd.iconId = iconNum;
                                iconMap[hash] = iconNum;
                                if (!Directory.Exists(SaveDirectory + "\\iconCache")) {
                                    Directory.CreateDirectory(SaveDirectory + "\\iconCache");
                                }
                                FileStream fs = new FileStream(String.Format(SaveDirectory + "\\iconCache\\{0}.png", iconNum), FileMode.Create, FileAccess.Write);
                                bitmap.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                                bitmap.Dispose();
                                iconNum++;
                            }
                        }
                    }



                }
                hwnd = Win32.GetNextWindow(hwnd, Win32.GW_HWNDNEXT);
            }
            return newMap;
        }

        public string getBitmapHash(Bitmap bitmap) {
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), 
                ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            byte[] bytes = new byte[bitmap.Width * bitmap.Height * 3];
            int byteIndex = 0;
            int stride = data.Stride;
            System.IntPtr Scan0 = data.Scan0;
            // convert to flattened format to prepare for hash
            unsafe {
                byte nVal;
                byte* p = (byte*)(void*)Scan0;
                int nOffset = stride - bitmap.Width * 3;
                int nWidth = bitmap.Width * 3;
                for (int y = 0; y < bitmap.Height; ++y) {
                    for (int x = 0; x < nWidth; ++x) {
                        nVal = (byte)(p[0]);
                        bytes[byteIndex++] = (byte)(nVal);

                        //if (nVal < 0) nVal = 0;
                        //if (nVal > 255) nVal = 255;
                        //p[0] = (byte)nVal;
                        ++p;
                    }
                    p += nOffset;
                }
            }
            bitmap.UnlockBits(data);

            MD5 md = new MD5CryptoServiceProvider();
            string hash = Convert.ToBase64String(md.ComputeHash(bytes));
            return hash;
        }
        
        // modified from http://blogs.msdn.com/brada/archive/2004/03/04/84069.aspx
        public static IEnumerable<string> GetFiles(string path, string glob) {
            foreach (string s in Directory.GetFiles(path, glob)) {
                yield return s;
            }
            foreach (string s in Directory.GetDirectories(path)) {
                foreach (string s1 in GetFiles(s, glob)) {
                    yield return s1;
                }
            }
        }

        public void loadIconMap() {
            Console.WriteLine("woot");
            foreach (string file in GetFiles(SaveDirectory + "\\iconCache", "*.png")) {
                Console.WriteLine("* " + file);
                Bitmap bm = new Bitmap(Bitmap.FromFile(file)); // cast ?
                string hash = getBitmapHash(bm);
                if (iconMap.ContainsKey(hash)) {
                    // exists
                    Console.WriteLine("Duplicating hash found in icon cache");
                } else {
                    int thisIconId = 0;
                    Match m = Regex.Match(file, "^.*?([0-9-.]+).png$");
                    if (m.Success) {
                        thisIconId = Convert.ToInt32(m.Groups[1].Value);
                    } else {
                        Console.WriteLine("Cannot parse filename '" + file + "'");
                    }
                    Console.WriteLine("adding [" + thisIconId + "]" + file);
                    iconMap[hash] = thisIconId;

                    if (thisIconId > iconNum) { iconNum = thisIconId; }
                }
            }
            Console.WriteLine("Icon cachesize=" + iconNum);
        }

        [MTAThread]
        public static int Main2(string[] args) {
            Timetube tt = Timetube.getInstance();
            tt.SaveDirectory = "c:\\temp\\newDir";
            tt.Interval = 10;

            Console.WriteLine("*** Started " + TimeFormat.GetDate() + " " + TimeFormat.GetTime());

            // initialise icon cache
            tt.loadIconMap();
            tt.BeginCapture();

            // spin wheels
            Console.WriteLine("press <enter> to stop...");
            Console.ReadLine();

            // shut down timer & event watchers
            tt.EndCapture();
            return 0;
        }

        public void BeginCapture() {
            // add timer
            if (!IsRunning) {
                _IsRunning = true;
                timer = new System.Timers.Timer(Interval * 1000);
                timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimer);
                timer.Start();

                WmiEventHandler myHandler = new WmiEventHandler();
                eventArrivedEventHandler = new EventArrivedEventHandler(myHandler.Win32ProcCreated);
                win32ProcCreatedWatcher = WmiEventHandler.GetWatcher("__InstanceCreationEvent");
                win32ProcCreatedWatcher.EventArrived += eventArrivedEventHandler;
                win32ProcCreatedWatcher.Start();

                eventArrivedEventHandler = new EventArrivedEventHandler(myHandler.Win32ProcDeleted);
                win32ProcDeletedWatcher = WmiEventHandler.GetWatcher("__InstanceDeletionEvent");
                win32ProcDeletedWatcher.EventArrived += eventArrivedEventHandler;
                win32ProcDeletedWatcher.Start();
            }
        }

        public void EndCapture() {
            if (IsRunning) {
                _IsRunning = false;
                timer.Stop();
                win32ProcCreatedWatcher.Stop();
                win32ProcCreatedWatcher.EventArrived -= eventArrivedEventHandler;
                win32ProcDeletedWatcher.Stop();
                win32ProcDeletedWatcher.EventArrived -= eventArrivedEventHandler;
            }
        }

        private void OnTimer(object sender, System.Timers.ElapsedEventArgs e) {
            if (working) {
                Console.WriteLine("(skipping)");
            } else {
                working = true;

                // Console.WriteLine("Working.");
                HiPerfTimer pt = new HiPerfTimer();     // create a new PerfTimer object
                pt.Start();                             // start the timer

                // capturing screen takes about 0.2 secs 
                
                string ext;
                if (SaveFormat.Equals("JPEG")) { ext = ".jpg"; }
                else if (SaveFormat.Equals("GIF")) { ext = ".gif"; }
                else if (SaveFormat.Equals("PNG")) { ext = ".png"; } 
                else if (SaveFormat.Equals("BMP")) { ext = ".bmp"; } 
                else {
                    ext = ".unknown";
                }

                String dir = SaveDirectory + "\\";
                String thisDate = TimeFormat.GetDate();
                String thisTime = TimeFormat.GetTime();
                String imgFilename = dir + thisDate + "\\desktop-" + thisTime + ext;
                String logFilename = dir + thisDate + "\\windowList.log";

                if (!Directory.Exists(dir + thisDate)) {
                    // make the directory
                    Directory.CreateDirectory(dir + thisDate);
                }
                ScreenCapture sc = new ScreenCapture();
                sc.CaptureScreenToFile(imgFilename, /*System.Drawing.Imaging.ImageFormat.Jpeg*/
                    SaveFormat, SaveQuality);

                StreamWriter sw = new StreamWriter(logFilename, true); // true = append
                sw.WriteLine("*** Begin snapshot " + thisTime);
                Hashtable newProcessMap = getProcessMap();
                List<int> newHwndOrder = new List<int>();
                Hashtable newWindowList = getWindowMap(newHwndOrder);
                mergeProcessMap(sw, newProcessMap);
                mergeWindowMap(sw, newWindowList);
                mergeHwndOrder(sw, newHwndOrder);

                lock (eventList) {
                    // Console.WriteLine("numevents:" + eventList.Count);
                    if (eventList.Count > 0) {
                        String plogFilename = dir + thisDate + "\\processList.log";
                        StreamWriter sw2 = new StreamWriter(plogFilename, true); // true = append
                        foreach (String s in eventList) {
                            sw2.WriteLine(s);
                        }
                        sw2.Close();
                        eventList.Clear();
                    }
                }

                pt.Stop();                              // stop the timer
                sw.WriteLine("*** Snapshot time " + pt.Duration);
                sw.Close();
                working = false;
            }
        }
    
    }
}



