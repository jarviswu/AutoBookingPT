using AutoBooking2.Helper;
using AutoBooking2.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Timers;
using System.Web.Services.Description;

namespace BookingDomainService
{
    /// <summary>
    /// Booking Service,do all booking and crawler operation
    /// </summary>
    public class BookingService
    {
        private static BookingService _singletonBookingService;
        private static object _lock = new object();
        private static Timer _bookingTimer;
        private const double intervel60Sec =1000;
        private DateTime _bookingDate;

        private BookingService() {
            _bookingTimer = new Timer(intervel60Sec);
            _bookingTimer.Enabled = true;
            _bookingTimer.Elapsed += checkForBookingDate_Elapsed;
        }

        public static BookingService GetInstance()
        {
            if (_singletonBookingService == null)
            {
                lock (_lock)
                {
                    if (_singletonBookingService == null)
                    {
                        _singletonBookingService = new BookingService();
                    }
                }
            }

            return _singletonBookingService;
        }

        /// <summary>
        /// set booking date and start
        /// </summary>
        /// <param name="BookingDate"></param>
        public void SetBookingDate()
        {
            // Auto booking at tomorrow 00:00
            _bookingDate= DateTime.Now.AddDays(1).Date;
            _bookingTimer.Enabled = true;
        }

        /// <summary>
        /// Booking Now
        /// </summary>
        public void Book()
        {
            //get booking info
            BookingInfo bookingModel = GetBookingInfo();

            //start booking
            double timeFromStamp = CalulateTimeFromStamp(bookingModel);
            double timeToStamp = CalulateTimeToStamp(timeFromStamp);

            var result = string.Empty;
            using (var client = new WebClient())
            {
                NameValueCollection request = ConstructRequest(bookingModel, timeFromStamp, timeToStamp);
                SetHeaders(bookingModel, client);

                //submit booking info
                var response = client.UploadValues("http://www.styd.cn/m/384378/course/order_confirm", request);

                // log
                result = Encoding.UTF8.GetString(response);
                var respBpdy = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseBody>(result);
                LogHelper.WriteLogAsync("End Booking", respBpdy.msg, LogType.Info);
            }

            _bookingTimer.Enabled = false;
        }

        private static BookingInfo GetBookingInfo()
        {
            SystemLogDBContext db = new SystemLogDBContext();
            var bookingModel = db.BookingModel.Select(p => p).FirstOrDefault(p => p.ID == 1);
            LogHelper.WriteLogAsync("Begin Booking", Newtonsoft.Json.JsonConvert.SerializeObject(bookingModel),
                        LogType.Info);
            return bookingModel;
        }

        private static void SetHeaders(BookingInfo bookingModel, WebClient client)
        {
            client.Headers["Referer"] = "http://www.styd.cn/m/384378/user/bind";
            client.Headers["User-Agent"] = "Mozilla/5.0";
            client.Headers["Cookie"] = bookingModel.Cookies;
        }

        private NameValueCollection ConstructRequest(BookingInfo bookingModel, double timeFromStamp, double timeToStamp)
        {
            var values = new NameValueCollection();
            values["member_card_id"] = GetMemberCardID(bookingModel.Owner);
            values["card_cat_id"] = GetCardCatID(bookingModel.Owner);
            values["course_id"] = GetCourseID(bookingModel.Traner); //todo:待定
            values["class_id"] = GetClassID(bookingModel.ClassDate, bookingModel.Traner);
            values["time_from_stamp"] = timeFromStamp.ToString();
            values["time_to_stamp"] = timeToStamp.ToString();
            values["quantity"] = "1";
            return values;
        }

        private static double CalulateTimeToStamp(double timeFromStamp)
        {
            return timeFromStamp + 3600;
        }

        private void checkForBookingDate_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (timeIsReady())
            {
                Book();
            }
        }

        private bool timeIsReady()
        {
            if (DateTime.Now < _bookingDate)
            {
                return false;
            }
            return true;
        }

        private double CalulateTimeFromStamp(BookingInfo bookingModel)
        {
            var standardDate = DateTime.Parse("2016-06-09 16:00");
            var standardTicks = 1465459200;
            var difTicks = (bookingModel.ClassDate - standardDate).TotalSeconds;

            var timeFromStamp = standardTicks + difTicks;
            return timeFromStamp;
        }

        private string GetClassID(DateTime date, string trainer)
        {
            var strDate = date.ToString("yyyy-MM-dd");
            var url = $"http://www.styd.cn/m/384378/default/index?date={strDate}&shop_id=1001049&type=2";

            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            var xpath = "//p[text()[contains(.,'" + trainer + "常规课程')]]/../..";
            var selectSingleNode = doc.DocumentNode.SelectSingleNode(xpath);
            var href = selectSingleNode.Attributes["href"].Value;

            var courseID = href.Split('/').Last();

            return courseID;
        }

        private string GetCourseID(string trainer)
        {
            if (trainer == "Alan")
                return "1022198";
            if (trainer == "Vito")
                return "1022193";
            throw new Exception("Traner Error:" + trainer);
        }

        private string GetMemberCardID(string owner)
        {
            if (owner == "W")
                return "1048977";
            throw new Exception("Owner Error:" + owner);
        }

        private string GetCardCatID(string owner)
        {
            if (owner == "W")
                return "1003298";
            throw new Exception("Owner Error:" + owner);
        }
    }
}
