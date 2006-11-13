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


    /// <summary>
    /// This class is the main class responsible for creating the window, process and desktop snapshots.
    /// 
    /// Use the BeginCapture and EndCapture methods to start and stop monitoring.
    /// </summary>
    class Timetube {

        /// <summary>
        /// Singleton object (used by other objects running on other threads). There are probably
        /// better ways of doing this, but running two captures at the same time isn't likely to
        /// be that useful.
        /// </summary>
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

        // properties
        private string saveDirectory;
        private int interval;
        private bool isRunning = false;
        private string saveFormat = "JPEG";
        private int saveQuality = 80;

        public string SaveDirectory {
            get { return saveDirectory;  }
            set { saveDirectory = value; }
        }
        
        public int Interval {
            get { return interval; }
            set { interval = value; }
        }
        
        public bool IsRunning {
            get { return isRunning; }
        }
        
        public string SaveFormat {
            get { return saveFormat; }
            set { saveFormat = SaveFormat; }
        }
        
        public int SaveQuality {
            get { return saveQuality; }
            set { saveQuality = SaveQuality; }
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


        /// <summary>
        /// Create a summary of processes created/deleted since the last snapshot. Output
        /// is in the form "P + (processdetails)" for new processes, "P - (processdetails)" for
        /// removed processes. Processes are displayed one per line, in the format
        /// specified by the <see cref="ProcessDetails"/> ToString method.
        /// </summary>
        /// 
        /// <param name="sw">StreamWriter used to send output to.</param>
        /// <param name="newMap">An object mapping integer pids to ProcessDetail objects</param>
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

        /// <summary>
        /// Similar to mergeProcessMap, but creates a summary windows created/deleted since the 
        /// last snapshot. Output is in the form "W + (windowdetails)" for new windows, 
        /// "W - (windowdetails)" for old windows. Windows are displayed one per line, in the format
        /// specified by the <see cref="WindowDetails"/> ToString method.
        /// </summary>
        /// 
        /// <param name="sw">StreamWriter used to send output to.</param>
        /// <param name="newMap">An object mapping integer hwnds to WindowDetail objects</param>
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

        /// <summary>
        /// If window z-order has changed, creates a log in the supplied StreamWriter. 
        /// Format is in the form "WO ! (hwnd list)" where the first element in the hwnd list
        /// is the first (top-most) window, the second element is the second, and so on. 
        /// 
        /// Bottom-most windows whose orders are unchanged are not included in the list.
        /// </summary>
        /// 
        /// <param name="sw">StreamWriter used to send output to.</param>
        /// <param name="newMap">A list of hwnd handles, with the 1st element identifying the top-most window
        /// </param>
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

        /// <summary>
        /// Create a map of hwnd's to WindowDetails objects, as well as an ordered list of 
        /// windows.
        /// </summary>
        /// 
        /// <param name="newHwndOrder">a list which will be modified by this method to contain the
        /// window list</param>
        /// <returns>a map as described above</returns>
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
                     * - has memory addresss issues ...
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

        /// <summary>
        /// Converts a bitmap (which would be an icon in this case) into an MD5 hash. We use this
        /// to do collision-detections against our icon cache; the chances of having two icons
        /// with the same hash are about 1 in 3402823669209384634633746074317700000000, 
        /// which isn't the sort of thing I'd buy a lottery ticket in. 
        /// </summary>
        /// 
        /// <param name="bitmap">bitmap to make a hash out of</param>
        /// <returns>an MD5 hash of the thing</returns>
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
        
        /// <summary>
        /// Recursively enumerate all files in a path that match a filepath pattern.
        /// (Modified from http://blogs.msdn.com/brada/archive/2004/03/04/84069.aspx )
        /// </summary>
        /// <param name="path">path to search within</param>
        /// <param name="glob">file pattern (e.g. "*.txt")</param>
        /// <returns></returns>
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

        /// <summary>
        /// Load the existing icon cache, so that when we stop and start the program we don't
        /// rewrite out any icons we've already stored on disk.
        /// </summary>
        public void loadIconMap() {
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

        /// <summary>
        /// We don't use this main method any more (the real one is in the MainClass.cs file),
        /// but just keeping it around in case I want to do some testing later on - could be 
        /// used to create a text-only version of the program.
        /// </summary>
        /// <param name="args">command-line arguments (ignored)</param>
        /// <returns>always returns 0</returns>
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

        /// <summary>
        /// Start capturing data
        /// </summary>
        public void BeginCapture() {
            // add timer
            if (!IsRunning) {
                isRunning = true;
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

        /// <summary>
        /// Finish capturing data.
        /// </summary>
        public void EndCapture() {
            if (IsRunning) {
                isRunning = false;
                timer.Stop();
                win32ProcCreatedWatcher.Stop();
                win32ProcCreatedWatcher.EventArrived -= eventArrivedEventHandler;
                win32ProcDeletedWatcher.Stop();
                win32ProcDeletedWatcher.EventArrived -= eventArrivedEventHandler;
            }
        }

        /// <summary>
        /// Timer entry point. At each timer tick, capture the desktop image, process list
        /// and window list. If the last tick hasn't finished processing, then this method
        /// returns without performing any processing. 
        /// </summary>
        /// 
        /// <param name="sender">Not entirely sure. Probably the timer object, I guess</param>
        /// <param name="e">The timer event</param>
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



