using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATWebLogger.Models
{
    public class ServerModel
    {
        public string WebLoggerId { get; set; }
        public string ServerIp { get; set; }
        public int LogTimeRate { get; set; }
        public string LogType { get; set; }
        public string ServerIpDisplay { get; set; }
    }
}