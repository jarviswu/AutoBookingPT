using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AutoBooking2.Models;

namespace AutoBooking2.Controllers
{
    public class SystemLogsController : Controller
    {
        private SystemLogDBContext db = new SystemLogDBContext();

        // GET: SystemLogs
        public async Task<ActionResult> Index(string title,DateTime? timeFrom,DateTime? timeTo,string Types)
        {
            var logs = db.Log.Select(p => p);
            if (timeFrom.HasValue && timeTo.HasValue)
            { logs = logs.Where(p => p.DataChange_lasttime >= timeFrom && p.DataChange_lasttime <= timeTo); }
            
            if (!string.IsNullOrEmpty(title))
            {
                logs = db.Log.Where(p => p.Title == title);
            }
            if (!string.IsNullOrEmpty(Types))
            {
                logs = db.Log.Where(p => p.Type == Types);
            }
            ViewBag.Types = new SelectList(db.Log.Select(p => p.Type).Distinct());

            return View(await logs.ToListAsync());
        }

        // GET: SystemLogs/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemLog systemLog = await db.Log.FindAsync(id);
            if (systemLog == null)
            {
                return HttpNotFound();
            }
            return View(systemLog);
        }

        // GET: SystemLogs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SystemLogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Title,Message,StackTrace,DataChange_lasttime")] SystemLog systemLog)
        {
            if (ModelState.IsValid)
            {
                db.Log.Add(systemLog);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(systemLog);
        }

        // GET: SystemLogs/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemLog systemLog = await db.Log.FindAsync(id);
            if (systemLog == null)
            {
                return HttpNotFound();
            }
            return View(systemLog);
        }

        // POST: SystemLogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Title,Message,StackTrace,DataChange_lasttime")] SystemLog systemLog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(systemLog).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(systemLog);
        }

        // GET: SystemLogs/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemLog systemLog = await db.Log.FindAsync(id);
            if (systemLog == null)
            {
                return HttpNotFound();
            }
            return View(systemLog);
        }

        // POST: SystemLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            SystemLog systemLog = await db.Log.FindAsync(id);
            db.Log.Remove(systemLog);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
