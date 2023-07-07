using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForSight.TimeZonesService.Core.Config
{
    public class PermittedReadAllTimeZonesClients
    {
        private const string TimeZones = "TimeZones";        

        public static readonly List<string> ClientIds = new()
        {
            TimeZones
        };
    }
}
