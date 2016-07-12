using AutoBooking.Models;
using AutoBooking2.Helper;
using AutoBooking2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AutoBooking2.Controllers
{
    public class HomeController : Controller
    {
        private SystemLogDBContext db = new SystemLogDBContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public async System.Threading.Tasks.Task<ActionResult> Save(string traner, string owner, DateTime classTime, bool enable, string cookies)
        {
            try
            {
                var bookingModel = new BookingInfo { ID = 1, Traner = traner, Owner = owner, ClassDate = classTime, Cookies = cookies };
                BookingModel.BookingTime = DateTime.Now.AddDays(1).Date;
                db.Entry(bookingModel).State = EntityState.Modified;
                await db.SaveChangesAsync();
                BookingModel.Enable = true;
                BookingModel.Timer = new System.Threading.Timer(BookingModel.PorcessBooking, null, 0, 1000);

                LogHelper.WriteLog("Save", Newtonsoft.Json.JsonConvert.SerializeObject(bookingModel), LogType.Info);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Booking Error", ex.Message, LogType.Error);
            }
            return Json(new { Message = "保存成功" });
        }

        public async System.Threading.Tasks.Task<ActionResult> Booking(string traner, string owner, DateTime classTime, string cookies)
        {
            try
            {
                var bookingModel = new BookingInfo { ID = 1, Traner = traner, Owner = owner, ClassDate = classTime, Cookies = cookies };
                BookingModel.BookingTime = DateTime.Now.AddDays(-1).Date;
                BookingModel.Enable = true;
                db.Entry(bookingModel).State = EntityState.Modified;
                await db.SaveChangesAsync();

                BookingModel.PorcessBooking(new object());
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Booking Error", ex.Message, LogType.Error);
            }
            return Json(new { Message = BookingModel.Message });
        }
    }
}