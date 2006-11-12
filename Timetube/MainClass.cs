using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Timetube {
    class MainClass {

        public static void Main(String[] args) {
            /*
            if (args.Length > 0 && args[0].Equals("--quiet")) {
             */

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            TimetubeForm timetubeForm = new TimetubeForm();
            Application.Run(timetubeForm);
        }
    }


}
