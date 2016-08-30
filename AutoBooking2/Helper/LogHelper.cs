using AutoBooking2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoBooking2.Helper
{
    public class LogHelper
    {
        private static SystemLogDBContext db = new SystemLogDBContext();

        public static void WriteLog(string title, string message, LogType type, string stackTrace = "")
        {
            db.Log.Add(new SystemLog { Title = title, Type = type.ToString(), Message = message, StackTrace = stackTrace });
            db.SaveChanges();
        }

        public static async void WriteLogAsync(string title, string message, LogType type, string stackTrace = "")
        {
            db.Log.Add(new SystemLog { Title = title, Type = type.ToString(), Message = message, StackTrace = stackTrace });
            db.SaveChangesAsync();
        }
    }

    public enum LogType
    {
        Info,
        Error
    }
}