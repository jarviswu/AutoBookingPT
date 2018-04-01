using System;
using System.Collections.Generic;
using System.Text;

namespace BookingDomainService
{
    /// <summary>
    /// Booking Service,do all booking and crawler operation
    /// </summary>
    public class BookingService
    {
        private static BookingService _singletonBookingService;
        private static object _lock = new object();

        private BookingService() { }

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

        public DateTime BookingTime { get; set; }
        public bool Enable { get; set; }

        public bool BookNow()
        {
            SystemLogDBContext db = new SystemLogDBContext();
            var bookingModel = db.BookingModel.Select(p => p).FirstOrDefault(p => p.ID == 1);
            return true;
        }

        public bool BookWithTimer()
        {
            return true;
        }

        private bool book()
        {
            return true;
        }


    }
}
