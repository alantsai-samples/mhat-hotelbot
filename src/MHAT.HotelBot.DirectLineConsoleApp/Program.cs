using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHAT.HotelBot.DirectLineConsoleApp
{
    class Program
    {
        private static string directLineSecret = 
            ConfigurationManager.AppSettings["DirectLineSecret"];

        private static string botId = ConfigurationManager.AppSettings["BotId"];

        private static string fromUser = "DirectLineSampleClientUser";

        static void Main(string[] args)
        {
        }
    }
}
