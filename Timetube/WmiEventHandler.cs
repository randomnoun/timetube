using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Runtime.InteropServices; // DllImport
using System.Collections;
using System.Management;
using System.ComponentModel;
using System.Threading;

using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography;


namespace Timetube {
    public class WmiEventHandler {

        public void Win32ProcCreated(object sender, EventArrivedEventArgs e) {
            List<string> eventList = Timetube.getInstance().eventList;
            if (Timetube.getInstance().eventList != null) {
                foreach (PropertyData pd in e.NewEvent.Properties) {
                    ManagementBaseObject mbo = null;
                    if ((mbo = pd.Value as ManagementBaseObject) != null) {
                        lock (eventList) {
                            eventList.Add(String.Format("{0} Start process {1} exe='{2}' cl='{3}'", TimeFormat.GetTime(),
                                mbo.Properties["ProcessId"].Value,
                                mbo.Properties["ExecutablePath"].Value, mbo.Properties["CommandLine"].Value));
                        }
                    }
                }
            }

            /*
            foreach (PropertyData pd in e.NewEvent.Properties) {
                ManagementBaseObject mbo = null;
                if ((mbo = pd.Value as ManagementBaseObject) != null) {
                    Console.WriteLine("--------------Properties------------------");
                    foreach (PropertyData prop in mbo.Properties)
                        Console.WriteLine("{0} - {1}", prop.Name, prop.Value);
                }
            }
            */
        }

        public void Win32ProcDeleted(object sender, EventArrivedEventArgs e) {
            List<string> eventList = Timetube.getInstance().eventList;
            if (eventList != null) {
                foreach (PropertyData pd in e.NewEvent.Properties) {
                    ManagementBaseObject mbo = null;
                    if ((mbo = pd.Value as ManagementBaseObject) != null) {
                        lock (eventList) {
                            eventList.Add(String.Format("{0} Start process {1} exe='{2}' cl='{3}'", TimeFormat.GetTime(),
                                mbo.Properties["ProcessId"].Value,
                                mbo.Properties["ExecutablePath"].Value, mbo.Properties["CommandLine"].Value));
                        }
                    }
                }
            }
            /*
            //lock (Timetube.eventList) {
            if (Timetube.eventList != null) {
                Timetube.eventList.Add(String.Format("{0} End process {1} exe='{2}' cl='{3}'",
                    TimeFormat.GetTime(), e.NewEvent.Properties["ProcessId"]));
            }
            //}
            Console.WriteLine("evc={0}",Timetube.eventList.Count);
             */
        }

        public static ManagementEventWatcher GetWatcher(string WatcherType) {
            string wql = WatcherType;
            WqlEventQuery EventQuery = new WqlEventQuery(wql,
              new TimeSpan(0, 0, 3), "TargetInstance ISA 'Win32_Process'"); // NOTE that
              // short running Process events may be lost
            ManagementEventWatcher watcher = new ManagementEventWatcher(EventQuery);
            return watcher;
        }
    
    }



}
