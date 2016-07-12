using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoBooking2.Models
{
    public class ResponseBody
    {
        public string code { get; set; }
        public List<string> data { get; set; }
        public string msg { get; set; }
        public string req_id { get; set; }
    }
}