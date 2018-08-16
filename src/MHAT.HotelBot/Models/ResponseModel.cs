using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MHAT.HotelBot.Models
{
    public class ResponseModel
    {
        public string type { get; set; }
        public string id { get; set; }
        public string recognition { get; set; }
        public string translation { get; set; }
    }
}