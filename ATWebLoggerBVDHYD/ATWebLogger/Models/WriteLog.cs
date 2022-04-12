using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ATWebLogger.Models
{
    public class WriteLog
    {
        public string DateTime { get; set; }
        public string Action { get; set; }
        public string Result { get; set; }
    }
}