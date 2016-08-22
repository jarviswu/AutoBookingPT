using AutoBooking2.Helper;
using AutoBooking2.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;

namespace AutoBooking.Models
{
    public class BookingModel
    {
        //public static string Cookies;
        //public static bool Enable;
        //public static DateTime ClassDate;
        //public static string Traner;
        //public static string Owner;

        public static DateTime BookingTime = DateTime.Now.AddDays(1).Date;
        public static string Message;
        public static Timer Timer;
        public static bool Enable = true;
        public static object locker = new object();

        private static SystemLogDBContext db = new SystemLogDBContext();

        public BookingModel()
        {
            LogHelper.WriteLog("我他妈被回收了", "我他妈被回收了", LogType.Info);
        }

        public static async void PorcessBooking(object o)
        {
            lock (locker)
            {
                try
                {
                    if (!Enable || DateTime.Now < BookingTime)
                    {
                        Debug.WriteLine("No booking");
                        return;
                    }
                    Debug.WriteLine("Begin booking");
                    SystemLogDBContext db = new SystemLogDBContext();
                    var bookingModel = db.BookingModel.Select(p => p).FirstOrDefault(p => p.ID == 1);
                    LogHelper.WriteLog("Begin Booking", Newtonsoft.Json.JsonConvert.SerializeObject(bookingModel),
                        LogType.Info);

                    var standardDate = DateTime.Parse("2016-06-09 16:00");
                    var standardTicks = 1465459200;
                    var difTicks = (bookingModel.ClassDate - standardDate).TotalSeconds;

                    var timeFromStamp = standardTicks + difTicks;
                    var timeToStamp = timeFromStamp + 3600;

                    var result = string.Empty;
                    using (var client = new WebClient())
                    {
                        var values = new NameValueCollection();
                        values["member_card_id"] = GetMemberCardID(bookingModel.Owner);
                        values["card_cat_id"] = GetCardCatID(bookingModel.Owner);
                        values["course_id"] = GetCourseID(bookingModel.Traner); //todo:待定
                        values["class_id"] = GetClassID(bookingModel.ClassDate, bookingModel.Traner);
                        values["time_from_stamp"] = timeFromStamp.ToString();
                        values["time_to_stamp"] = timeToStamp.ToString();
                        values["quantity"] = "1";

                        client.Headers["Referer"] = "http://www.styd.cn/m/384378/user/bind";
                        client.Headers["User-Agent"] = "Mozilla/5.0";
                        client.Headers["Cookie"] = bookingModel.Cookies;

                        var response = client.UploadValues("http://www.styd.cn/m/384378/course/order_confirm", values);
                        result = Encoding.UTF8.GetString(response);
                        var respBpdy = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseBody>(result);
                        Message = respBpdy.msg;
                        Debug.WriteLine(Message);
                    }
                    Enable = false;
                    LogHelper.WriteLog("End Booking", Message, LogType.Info);
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("Booking Error", ex.Message, LogType.Error, ex.StackTrace);
                }
            }
        }

        private static string GetClassID(DateTime date, string trainer)
        {
            var strDate = date.ToShortDateString();
            var url = $"http://www.styd.cn/m/384378/default/index?date={strDate}&shop_id=1001049&type=2";

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            var href = doc.DocumentNode.SelectSingleNode("//p[text()='" + trainer + "常规课程']/../..").Attributes["href"].Value;
            var courseID = href.Split('/').Last();

            return courseID;

        }

        private static string GetCourseID(string trainer)
        {
            if (trainer == "Alan")
                return "1022198";
            if (trainer == "Vito")
                return "1022193";
            throw new Exception("Traner Error:" + trainer);
        }

        private static string GetMemberCardID(string owner)
        {
            if (owner == "W")
                return "1048977";
            throw new Exception("Owner Error:" + owner);
        }

        private static string GetCardCatID(string owner)
        {
            if (owner == "W")
                return "1003298";
            throw new Exception("Owner Error:" + owner);
        }
    }


}

