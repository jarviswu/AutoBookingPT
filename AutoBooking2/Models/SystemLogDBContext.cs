using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AutoBooking2.Models
{
    public class SystemLogDBContext: DbContext
    {
        public DbSet<SystemLog> Log { get; set; }
        public DbSet<BookingInfo> BookingModel { get; set; }

        public SystemLogDBContext()
        {
            Database.SetInitializer<SystemLogDBContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();
        }
    }

    public class SystemLog
    {
        public long ID { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime DataChange_lasttime { get; set; }

        public SystemLog()
        {
            DataChange_lasttime = DateTime.Now;
        }
    }

    public class BookingInfo
    {
        public int ID { get; set; }
        public string Cookies { get; set; }
        public DateTime ClassDate { get; set; }
        public string Traner { get; set; }
        public string Owner { get; set; }
    }
}
