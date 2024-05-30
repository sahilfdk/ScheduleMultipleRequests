using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleMultipleRequests
{
    internal static class ConfigurableItems
    {
        internal static void PrintWaitingMessage()
        {
            // Wait until 8:21:30 PM
            Console.WriteLine("Waiting until 08:41:54 AM...");
        }

        internal static bool IsTimeToStart()
        {
            DateTime now = DateTime.Now;
            return now.Hour == 08 && now.Minute == 41 && now.Second >= 54;
        }
    }
}
