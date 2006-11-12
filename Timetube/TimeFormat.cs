using System;
using System.Collections.Generic;
using System.Text;

namespace Timetube {
    public class TimeFormat {

        // @TODO obvious problems with date/time during day rollover
        public static string GetDate() {
            string dateString = "";
            DateTime now = DateTime.Now;
            int day = now.Day;
            int month = now.Month;
            int year = now.Year;
            dateString = year + "-" + ((month < 10) ? "0" + month.ToString() : month.ToString());
            dateString += "-" + ((day < 10) ? "0" + day.ToString() : day.ToString());
            return dateString;
        }

        public static string GetTime() {
            string timeString = "";
            DateTime now = DateTime.Now;
            int hour = now.Hour;
            int min = now.Minute;
            int sec = now.Second;
            timeString = (hour < 10) ? "0" + hour.ToString() : hour.ToString();
            timeString += "." + ((min < 10) ? "0" + min.ToString() : min.ToString());
            timeString += "." + ((sec < 10) ? "0" + sec.ToString() : sec.ToString());
            timeString += ".1234"; // for compatibility with timesnapper. not sure what this is :)
            return timeString;
        }
    }
}
