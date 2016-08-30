using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoBooking.Models;
using AutoBooking2.Helper;
using AutoBooking2.Models;
using Newtonsoft.Json;

namespace AutoBooking2.Controllers
{
    public class HomeController : Controller
    {
        private SystemLogDBContext db = new SystemLogDBContext();

        public async Task<ActionResult> Index()
        {
            return View(await db.BookingModel.FirstOrDefaultAsync());
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

        public ActionResult Save(string traner, string owner, DateTime classTime, bool enable, string cookies)
        {
            try
            {
                LogHelper.WriteLog("Save", "s", LogType.Info);
                var bookingModel = new BookingInfo { ID = 1, Traner = traner, Owner = owner, ClassDate = classTime, Cookies = cookies };
                BookingModel.BookingTime = DateTime.Now.AddDays(1).Date;
                //BookingModel.BookingTime = DateTime.Now.AddMinutes(1);
                db.Entry(bookingModel).State = EntityState.Modified;
                db.SaveChangesAsync();
                BookingModel.Enable = true;
                BookingModel.Timer = new Timer(BookingModel.PorcessBooking, null, 0, 1000);

                LogHelper.WriteLog("Save", JsonConvert.SerializeObject(bookingModel), LogType.Info);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Booking Error", ex.Message, LogType.Error);
            }
            LogHelper.WriteLog("Save", "b", LogType.Info); 
            return Json(new { Message = "保存成功" });
        }

        public async Task<ActionResult> Booking(string traner, string owner, DateTime classTime, string cookies)
        {
            try
            {
                var bookingModel = new BookingInfo { ID = 1, Traner = traner, Owner = owner, ClassDate = classTime, Cookies = cookies };
                BookingModel.BookingTime = DateTime.Now.AddDays(-1).Date;
                BookingModel.Enable = true;
                db.Entry(bookingModel).State = EntityState.Modified;
                db.SaveChangesAsync();

                BookingModel.PorcessBooking(new object());
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Booking Error", ex.Message, LogType.Error);
                //BookingModel.Message = ex.Message;
            }
            return Json(new { BookingModel.Message });
        }
    }
}