using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoBooking2.Helper;
using AutoBooking2.Models;
using Newtonsoft.Json;
using BookingDomainService;

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

        public ActionResult Save(string traner, string owner, DateTime classDate, bool enable, string cookies)
        {
            try
            {
                var bookingModel = new BookingInfo { ID = 1, Traner = traner, Owner = owner, ClassDate = classDate, Cookies = cookies };
                db.Entry(bookingModel).State = EntityState.Modified;
                db.SaveChangesAsync();
                LogHelper.WriteLog("Save", JsonConvert.SerializeObject(bookingModel), LogType.Info);

                BookingService.GetInstance().SetBookingDate(); 
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Booking Error", ex.Message, LogType.Error);
                return Json(new { Message = ex.Message });
            }
            return Json(new { Message = "保存成功" });
        }

        public async Task<ActionResult> Booking(string traner, string owner, DateTime classTime, string cookies)
        {
            try
            {
                var bookingModel = new BookingInfo { ID = 1, Traner = traner, Owner = owner, ClassDate = classTime, Cookies = cookies };
                db.Entry(bookingModel).State = EntityState.Modified;
                db.SaveChangesAsync();

                BookingService.GetInstance().Book();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Booking Error", ex.Message, LogType.Error);
                return Json(new { Message = ex.Message });
            }
            return Json(new { Message = "预定成功" });
        }
    }
}